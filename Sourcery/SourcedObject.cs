using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sourcery;
using Sourcery.EventStores;
using Sourcery.IO;
using Sourcery.Migrations;

namespace Sourcery
{

    public class SourcedObject<T> : ISourcedObject<T> where T : class
    {
        private readonly object[] _arguments;

        public SourcedObject(IEventStore eventStore, object[] arguments = null, MigrationsLibrary migrations = null, Action<T> onRebuild = null)
        {
            _arguments = arguments ?? new object[] {};
            Migrations = migrations ?? new CodeMigrationsLibrary(Assembly.GetCallingAssembly());
            OnRebuild = onRebuild;
            

            this.EventStore = eventStore;
            Lock = new ReaderWriterLockSlim();
            using (var session = EventStore.OpenSession())
            {
                if (session.Events.Any() == false)
                {
                    BuildFromActionLog(session);
                }
            }
        }

        public MigrationsLibrary Migrations { get; set; }

        public string Id
        {
            get { return EventStore.Id; }
        }

        public ReaderWriterLockSlim Lock { get; private set; }


        private T _read;

        object ISourcedObject.ReadModel
        {
            get { return ReadModel; }
        }

        public IEventStore EventStore { get; set; }

        IEventStore ISourcedObject.EventStore
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
                            t = BuildFromActionLog(session);

                        }
                    }
                    _read = t;
                }
                return _read;
            }
        }

        public Action<T> OnRebuild { get; set; }



        private T BuildFromActionLog(IEventStoreSession session)
        {
            T t = null;
            bool changesMade = false;

            foreach (var @event in session.Events)
            {
                var isFirstEvent = t == null;

                var changesMadeToObject = false;
                ApplyMigrations(@event, out changesMadeToObject, session, isInit: isFirstEvent);
                changesMade |= changesMadeToObject;

                if (isFirstEvent)
                {
                    if (@event.GetCommand() as InitCommand == null)
                    {
                        throw new InvalidOperationException(".init must be the first event - was " + @event.GetCommand().GetType().FullName);
                    }
                    t = Initialise(@event);
                    continue;
                }

                if (@event.Deleted) continue;

                CommandBase command = null;
                try
                {
                    using (Profiler.Step("Deserialising"))
                    {
                        command = @event.GetCommand();
                    }

                    using (Profiler.Step("Applying command"))
                    {

                        command.Gateway.ResultCounter = 0;
                        command.Apply(t);
                    }
                }
                catch (Exception ex)
                {
                    throw new RebuildException(ex.InnerException ?? ex, @event);
                }
            }


            if (t == null) //there must have been no events
            {
                using (new PushScope<Gateway>(Gateway.Stack, new Gateway()))
                {
                    t = (T) FormatterServices.GetSafeUninitializedObject(typeof (T));
                    t.SetSourcerer(this);
                    var ctor = typeof (T).GetConstructors().Single(c => MatchesArguments(c, _arguments));
                    ctor.Invoke(t, _arguments);
                    var initEvent = new SourceryEvent(0);
                    initEvent.SetContent(new InitCommand(Gateway.Current) {Type = typeof (T).AssemblyQualifiedName, Arguments = new JArray(_arguments ?? new object[] {})});
                    session.Write(initEvent);
                    changesMade = true;
                }
            }


            if (changesMade) session.SaveChanges();


            if (OnRebuild != null)
            {
                OnRebuild(t);
            }

            return t;
        }

        private bool MatchesArguments(ConstructorInfo constructorInfo, object[] arguments)
        {
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length == arguments.Length) //should do type check here
            {
                return true;
            }
            return false;
        }


        private T Initialise(SourceryEvent @event)
        {
            T t;


            var init = (InitCommand) @event.GetCommand();
            using (new PushScope<Gateway>(Gateway.Stack, init.Gateway))
            {
                Gateway.Current.ResultCounter = 0;

                var type = typeof (T);
                var ctorAndArgs = type.GetConstructors().Select(c => new {ctor = c, args = MakeArray(c, init.Arguments)}).Single(ca => MatchesArguments(ca.ctor, ca.args));


                t = (T) FormatterServices.GetSafeUninitializedObject(type);
                t.SetSourcerer(this);
                ctorAndArgs.ctor.Invoke(t, ctorAndArgs.args);

            }
            return t;
        }

        private object[] MakeArray(ConstructorInfo constructorInfo, JArray arguments)
        {
            var parameters = constructorInfo.GetParameters();
            if (parameters.Length != arguments.Count)
            {
                return null;
            }
            return parameters.Zip(arguments, (info, token) => token.ToObject(info.ParameterType, new CustomSerializerSettings())).ToArray();
        }

        private string ApplyMigrations(SourceryEvent @event, out bool changesMade, IEventStoreSession migrationsSession, bool isInit = false)
        {
            var json = @event.Content;
            using (Profiler.Step("Migrating"))
            {
                var eventVersion = @event.Version;
                if (Migrations.MaxVersion > eventVersion)
                {
                    foreach (var migration in Migrations.YieldedMigrations
                        .OrderBy(m => m.VersionNumber)
                        .Where(ym => ym.VersionNumber > eventVersion))
                    {
                        migration.Transform(@event, isInit);
                    }
                    @event.Content = json;
                    migrationsSession.Write(@event);
                    changesMade = true;
                }
                changesMade = false;
            }
            return json;
        }



        public object ApplyCommandAndLog(CommandBase command, Func<object> apply = null)
        {
            //apply = apply ?? (() => command.Apply(ReadModel));
            var result = command.Apply(ReadModel);
            using (Profiler.Step("Logging command"))
            {
                using (var session = EventStore.OpenSession())
                {
                    var @event = new SourceryEvent(session.Count);
                    @event.SetContent(command);
                    session.Write(@event);
                    session.SaveChanges();
                }
                
            }
            return result;
        }
         
    }


    public interface ISourcedObject<out T> : ISourcedObject
    {
        new T ReadModel { get; }
    }
}