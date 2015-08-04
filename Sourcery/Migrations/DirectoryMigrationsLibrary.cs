using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Sourcery.IO;

namespace Sourcery.Migrations
{
    public class DirectoryMigrationsLibrary : IDirectoryMigrationsLibrary
    {
        private readonly IDirectory _getDirectoryInfo;
        private int? _maxId;

        public DirectoryMigrationsLibrary(IDirectory getDirectoryInfo = null)
        {
            _getDirectoryInfo = getDirectoryInfo;
            if (_getDirectoryInfo != null) _getDirectoryInfo.Create();
        }

        
        public IEnumerable<Migration> YieldedMigrations
        {
            get
            {
                if (_getDirectoryInfo == null) yield break;
                foreach (var file in _getDirectoryInfo.EnumerateFiles("*.json"))
                {
                    var customSerializerSettings = new CustomSerializerSettings();
                    var migration = JsonConvert.DeserializeObject<Migration>(file.ReadAllText(), customSerializerSettings);
                    migration.Id = int.Parse(file.Name.Substring(0, file.Name.IndexOf(' ')));
                    yield return migration;
                }
            }
        }

        public int MaxId
        {
            get
            {
                return _maxId ?? (_maxId = _getDirectoryInfo.EnumerateFiles()
                    .Where(f => f.Name.EndsWith(".json"))
                    .Select(file => int.Parse(file.Name.Substring(0, file.Name.IndexOf(' ')))).LastOrDefault()).Value;
            }
        }

        public void AddMigration(Migration migration)
        {
            migration.Id = MaxId + 1;
            _maxId++;
            using(var session = _getDirectoryInfo.OpenSession())
            {
                session.Write(migration.Id + " - " + migration.FriendlyFileName + ".json", JsonConvert.SerializeObject(migration, Formatting.Indented, new CustomSerializerSettings()));
                session.Save();
            }
        }
    }
}