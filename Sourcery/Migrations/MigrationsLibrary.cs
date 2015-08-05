using System.Collections.Generic;
using System.Linq;

namespace Sourcery.Migrations
{
    public abstract class MigrationsLibrary
    {
        public abstract IEnumerable<Migration> YieldedMigrations { get; }
        private int? _maxVersion;
        public int MaxVersion
        {
            get
            {
                return _maxVersion ?? (_maxVersion = YieldedMigrations.Select(m => m.VersionNumber).OrderByDescending(id => id).FirstOrDefault()).Value;
            }
        }
    }
}