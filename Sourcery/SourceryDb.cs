using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Humanizer;
using Sourcery.EventStores;
using Sourcery.Migrations;

namespace Sourcery
{
    public class SourceryDb
    {
        readonly ConcurrentDictionary<string, ISourcedObject> sourcerers = new ConcurrentDictionary<string, ISourcedObject>();
        private readonly Func<string, IEventStore> eventStoreFactory;
        private MigrationsLibrary migrationsLib;

        public SourceryDb(Func<string, IEventStore> eventStoreFactory)
        {
            this.eventStoreFactory = eventStoreFactory;
            CreateMigrationLibrary = () => new CodeMigrationsLibrary(Assembly.GetCallingAssembly());
        }

        public Func<MigrationsLibrary> CreateMigrationLibrary { get; set; }

        ISourcedObject Get(Type t, string objectid)
        {
            var method = this.GetType().GetMethods()
                .Single(m => m.Name == "Get" && m.IsGenericMethod)
                .MakeGenericMethod(t);
            return (ISourcedObject)method.Invoke(this, new object[] { objectid });
        }

        public ISourcedObject<T> Get<T>(string objectid = null, object[] arguments = null, Action<T> onRebuild = null) where T : class
        {
            objectid = objectid ?? typeof (T).Name.Dasherize().ToLowerInvariant();
            return (ISourcedObject<T>)sourcerers.GetOrAdd(objectid, (s) => new SourcedObject<T>(GetEventStore(s), arguments, Migrations, onRebuild));
        }
        
        public bool Exists(string objectid)
        {
            using(var session = GetEventStore(objectid).OpenSession())
            {
                return session.Events.Any();
            }
        }

        private IEventStore GetEventStore(string objectid)
        {
            return eventStoreFactory(objectid);
        }

        protected MigrationsLibrary Migrations
        {
            get { return migrationsLib ?? (migrationsLib = CreateMigrationLibrary()); }
        }

        public void Delete(string objectId)
        {
            GetEventStore(objectId).Delete();
        }
    }
}