using System;
using System.Threading;
using Sourcery.EventStores;
using Sourcery.Migrations;

namespace Sourcery
{
    public interface ISourcedObject
    {
        object ReadModel { get; }
        IEventStore EventStore { get; }
        object ApplyCommandAndLog(CommandBase command, Func<object> apply = null);
        ReaderWriterLockSlim Lock { get; }
        string Id { get; }
        MigrationsLibrary Migrations { get; }
    }
}