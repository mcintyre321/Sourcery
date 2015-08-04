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
                .OrderBy(m => m.Id).ToArray();

            var doubledUpMigrations = _migrations.GroupBy(m => m.Id).Where(g => g.Count() > 1).ToArray();
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

    public class DoubledUpMigrationsException : Exception
    {
        public DoubledUpMigrationsException(IGrouping<int, Migration>[] doubledUpMigrations)
            : base(BuildMessage(doubledUpMigrations))
        {
            DoubledMigrations = doubledUpMigrations;
        }

        public IGrouping<int, Migration>[] DoubledMigrations { get; private set; }

        private static string BuildMessage(IGrouping<int, Migration>[] doubledUpMigrations)
        {
            return "You have " + doubledUpMigrations.Count() + " migrations with the same Id:" +
                   string.Join(",", doubledUpMigrations.Select(m => m.Key + " (" + m.Count() + ")"));
        }
    }
}
