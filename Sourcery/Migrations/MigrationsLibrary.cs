using System.Collections.Generic;
using System.Linq;

namespace Sourcery.Migrations
{
    public abstract class MigrationsLibrary
    {
        public abstract IEnumerable<Migration> YieldedMigrations { get; }
        private int? _maxId;
        public int MaxId
        {
            get
            {
                return _maxId ?? (_maxId = YieldedMigrations.Select(m => m.Id).OrderByDescending(id => id).FirstOrDefault()).Value;
            }
        }
    }
}