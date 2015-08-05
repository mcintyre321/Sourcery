using System;
using Sourcery.IO;

namespace Sourcery.EventStores
{
    public class DirectoryEventStore : IEventStore
    {
        public DirectoryEventStore(IDirectory directory )
        {
            this.Directory = directory;
            Directory.Create();
        }

        public IDirectory Directory { get; set; }

        public string Id
        {
            get { return this.Directory.Name; }
        }

        
        
        public IEventStoreSession OpenSession()
        {
            return new DirectoryEventStoreSession(this.Directory.OpenSession());
        }

        public void LogEvent(CommandBase command)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            this.Directory.Delete();
        }
    }
}