using System;
using System.Collections;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using Sourcery.EventStores;
using Sourcery.IO;
using Sourcery.IO.FileSystem;
using Sourcery.IO.ZipFileSystem;
using Sourcery.Migrations;

namespace Sourcery.Db
{
    public class SourceryDb
    {
        ConcurrentDictionary<string, ISourcerer> sourcerers = new ConcurrentDictionary<string, ISourcerer>();
        Hashtable initCommandTypes = new Hashtable();

        public void SetInitCommand<T, TInitCommand>()
        {
            initCommandTypes.Add(typeof(T), typeof(TInitCommand));
        }
        private Fs _fs;
        private MigrationsLibrary _migrationsLib;

        public SourceryDb(Type fs = null, string rootPath = null)
        {
            var appDataPath = rootPath ?? Path.Combine(GetAppDataPath(), "sourcery");
            _fs = (Fs)Activator.CreateInstance(fs ?? typeof(ZipFileSystemFs), new object[] { appDataPath });

            CreateMigrationLibrary = () => new CodeMigrationsLibrary(Assembly.GetCallingAssembly());
        }

        public Func<MigrationsLibrary> CreateMigrationLibrary { get; set; }

        public ISourcerer Get(Type t, string objectid)
        {
            var method = this.GetType().GetMethods()
                .Single(m => m.Name == "Get" && m.IsGenericMethod)
                .MakeGenericMethod(t);
            return (ISourcerer)method.Invoke(this, new object[] { objectid });
        }

        public ISourcerer<T> Get<T>(string objectid, object[] arguments = null, Action<T> onRebuild = null) where T : class
        {
            var initCommandType = initCommandTypes[typeof (T)] as Type;
            return (ISourcerer<T>)sourcerers.GetOrAdd(objectid, (s) => new Sourcerer<T>(GetEventStore(s), arguments, Migrations, onRebuild, initCommandType));
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
            return new EventStore(_fs.Root.GetDirectoryInfo(objectid));
        }

        protected MigrationsLibrary Migrations
        {
            get { return _migrationsLib ?? (_migrationsLib = CreateMigrationLibrary()); }
        }

        private static string GetAppDataPath()
        {
            return AppDomain.CurrentDomain.GetData("DataDirectory") as string ??
                   (AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "/App_Data");

        }
    }

    
}