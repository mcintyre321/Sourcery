namespace Sourcery.Migrations
{
    public interface IDirectoryMigrationsLibrary : IMigrationsLibrary
    {
        void AddMigration(Migration migration);
    }
}