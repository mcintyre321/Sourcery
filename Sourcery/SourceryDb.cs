using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Humanizer;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sourcery.EventStores;
using Sourcery.Migrations;

namespace Sourcery
{
    public class ConstructorArgs<T>
    {
        public JObject Value { get; set; }


        public ConstructorArgs(Expression<Func<T>> ctorExpr)
        {
            if (ctorExpr == null)
            {
                this.Value = new JObject();
            }
            else
            {
                var newExpression = ((NewExpression) ctorExpr.Body);
                var ctor = newExpression.Constructor;
                var paramsAndArg = ctor.GetParameters().Zip(newExpression.Arguments, Tuple.Create);
                var dict = paramsAndArg.ToDictionary(tup => tup.Item1.Name,
                    tup => ExpressionHelper.GetValueFromExpression(tup.Item2));
                this.Value = (JObject.FromObject(dict));
            }
        }
    }

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

        public ISourcedObject<T> Get<T>(string objectid = null, Expression<Func<T>> construct = null, Action<T> onRebuild = null) where T : class
        {
            objectid = objectid ?? typeof (T).Name.Dasherize().ToLowerInvariant();
            return (ISourcedObject<T>)sourcerers.GetOrAdd(objectid, (s) => new SourcedObject<T>(GetEventStore(s), new ConstructorArgs<T>(construct).Value, Migrations, onRebuild));
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