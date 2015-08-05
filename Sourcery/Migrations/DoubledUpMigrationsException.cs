using System;
using System.Linq;

namespace Sourcery.Migrations
{
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