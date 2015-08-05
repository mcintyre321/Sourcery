using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Sourcery.Migrations
{
    public class CodeMigrationsLibrary : MigrationsLibrary
    {
        private IEnumerable<Migration> _migrations;

        public CodeMigrationsLibrary(params Assembly[] assemblies)
        {
            _migrations = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => typeof (Migration).IsAssignableFrom(t) && t.IsAbstract == false)
                .Select(t => (Migration) Activator.CreateInstance(t))
                .OrderBy(m => m.VersionNumber).ToArray();

            var doubledUpMigrations = _migrations.GroupBy(m => m.VersionNumber).Where(g => g.Count() > 1).ToArray();
            if (doubledUpMigrations.Any())
            {
                throw new DoubledUpMigrationsException(doubledUpMigrations);
            }
            ;
        }

        public override IEnumerable<Migration> YieldedMigrations
        {
            get { return _migrations; }
        }

      
    }
}
