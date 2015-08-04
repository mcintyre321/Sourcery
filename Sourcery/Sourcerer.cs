using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sourcery;
using Sourcery.EventStores;
using Sourcery.IO;
using Sourcery.Migrations;
using Sourcery.Proxy;

namespace Sourcery
{

    public class Sourcerer<T> : ISourcerer<T>
        where T : class
    {
        private readonly object[] _arguments;

        public Sourcerer(IEventStore eventStore, object[] arguments = null, MigrationsLibrary migrations = null, Action<T> onRebuild = null)
        {
            _arguments = arguments;
            Migrations = migrations ?? new CodeMigrationsLibrary(Assembly.GetCallingAssembly());
            OnRebuild = onRebuild;
            DateFormat = "yyyy-MM-dd HH-mm-ss fffffff";

            this.EventStore = eventStore;
            Lock = new ReaderWriterLockSlim();
            using (var session = EventStore.OpenSession())
            {
                if (session.Events.Any() == false)
                {
                    BuildFromActionLog(session);
                }
            }
            MakeCommand = ProxyExtension.MakeCommand;
        }

        public MigrationsLibrary Migrations { get; set; }

        public string Id
        {
            get { return EventStore.Id; }
        }

        public ReaderWriterLockSlim Lock { get; private set; }
        public String DateFormat { get; set; }

        private T _read;

        object ISourcerer.ReadModel
        {
            get { return ReadModel; }
        }

        public IEventStore EventStore { get; set; }

        IEventStore ISourcerer.EventStore
        {
            get { return EventStore; }
        }


        public T ReadModel
        {
            get
            {
                if (_read == null)
                {
                    T t = null;
                    using (Profiler.Step("Updating from action log"))
                    {
                        using (var session = EventStore.OpenSession())
                        {
                            t = BuildFromActionLog(session).SetSourcerer(this);

                            if (OnRebuild != null)
                            {
                                OnRebuild(t);
                            }
                        }
                    }
                    _read = t;
                }
                return _read;
            }
        }

        public Action<T> OnRebuild { get; set; }

        private CustomSerializerSettings settings = new CustomSerializerSettings();


        private T BuildFromActionLog(EventStoreSession migrationsSession)
        {
            var commandExceptions = new List<RebuildException>();

            T t = null;
            bool changesMade = false;
            {
                foreach (var @event in migrationsSession.Events)
                {
                    if (t == null)
                    {
                        t = Initialise(@event);
                        continue;
                    }

                    var jobject = JObject.Parse(@event.Content);
                    changesMade |= ApplyMigrations(@event, jobject, migrationsSession);

                    if (jobject["_deleted"] != null && jobject["_deleted"].Value<bool>()) continue;

                    CommandBase command = null;
                    try
                    {
                        using (Profiler.Step("Deserialising"))
                        {
                            command = jobject.ToObject<CommandBase>(JsonSerializer.Create(settings));
                        }

                        using (Profiler.Step("Applying command"))
                        {
                            command.Gateway.ResultCounter = 0;
                            command.Apply(t);
                        }
                    }
                    catch (Exception ex)
                    {
                        var ce = new RebuildException(ex.InnerException ?? ex)
                        {
                            Command = command,
                            Sourcerer = this,
                            Name = @event.Name,
                            Json = @event.Content,
                            Model = t
                        };
                        commandExceptions.Add(ce);
                    }

                }



                if (t == null) //there must have been no events
                {
                    using (ThreadScoper.Use(new Gateway()))
                    {
                        t = (T)Activator.CreateInstance(typeof(T), _arguments);
                        migrationsSession.Write(".init", JsonConvert.SerializeObject(new InitCommand()
                        {
                            Gateway = Gateway.Current,
                            Type = typeof(T).AssemblyQualifiedName,
                            Arguments = new JArray(_arguments ?? new object[] { })
                        }));
                        changesMade = true;
                    }

                }


                if (commandExceptions.Any())
                {
                    throw new UpdateException(commandExceptions);
                }
                if (changesMade) migrationsSession.Save();

                return t;
            }

        }

        private T Initialise(Event @event)
        {
            T t;
            if (@event.Name != ".init")
            {
                throw new InvalidOperationException(".init must be the first event - was " + @event.Name);
            }
            var init = JsonConvert.DeserializeObject<InitCommand>(@event.Content, settings);
            using (ThreadScoper.Use(init.Gateway))
            {
                Gateway.Current.ResultCounter = 0;
                var type = Type.GetType(init.Type);
                var arguments = new object[] { };

                if (init.Arguments != null)
                {
                    arguments = typeof(T).GetConstructors()
                        .Select(c => c.GetParameters()).Single(p => p.Count() == init.Arguments.Count)
                        .Zip(init.Arguments, (pi, token) => token.ToObject(pi.ParameterType, settings))
                        .ToArray();
                }

                    t = (T)Activator.CreateInstance(type, arguments);
                if (OnRebuild != null)
                {
                    OnRebuild(t);
                }
            }
            return t;
        }

        private bool ApplyMigrations(Event @event, JObject jobject, EventStoreSession migrationsSession)
        {
            using (Profiler.Step("Migrating"))
            {
                var jobjectVersion = jobject["_version"] == null ? 0 : jobject["_version"].Value<int>();
                if (Migrations.MaxId > jobjectVersion)
                {
                    foreach (var migration in Migrations.YieldedMigrations.Where(ym => ym.Id > jobjectVersion))
                    {
                        migration.Transform(jobject);
                    }
                    migrationsSession.Write(@event.Name, JsonConvert.SerializeObject(jobject, Formatting.Indented, this.settings));
                    return true;
                }
                return false;
            }
        }

        public ProxyExtension.MakeCommandDelegate MakeCommand { get; set; }


        public object ApplyCommandAndLog(CommandBase command, Func<object> apply = null)
        {
            apply = apply ?? (() => command.Apply(ReadModel));
            var result = apply();
            using (Profiler.Step("Logging command"))
            {
                var name = command.Timestamp.ToString(DateFormat) + " " + command.ShortDescriptionForFilename;
                command._version = Migrations.MaxId;
                this.EventStore.LogEvent(name, JsonConvert.SerializeObject(command, Formatting.Indented, settings));
            }
            return result;
        }

    }

    public interface ISourcerer
    {
        object ReadModel { get; }
        IEventStore EventStore { get; }
        object ApplyCommandAndLog(CommandBase command, Func<object> apply = null);
        ReaderWriterLockSlim Lock { get; }
        string Id { get; }
        MigrationsLibrary Migrations { get; }
    }

    public interface ISourcerer<out T> : ISourcerer
    {
        T ReadModel { get; }

    }
}