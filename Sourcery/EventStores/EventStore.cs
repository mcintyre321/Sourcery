using System;
using System.Collections.Generic;
using System.IO;
using Sourcery.IO;
using System.Linq;
using System.Runtime.Caching;
using Newtonsoft.Json;
using Sourcery.Migrations;

namespace Sourcery.EventStores
{
    public class EventStore : IEventStore
    {
        public EventStore(IDirectory directory )
        {
            this.Directory = directory;
            Directory.Create();
        }

        public IDirectory Directory { get; set; }

        public string Id
        {
            get { throw new NotImplementedException(); }
        }

        
        public void LogEvent(string eventName, string content)
        {
            using(var session = OpenSession())
            {
                session.Write(eventName, content);
                session.Save();
            }
        }

        public EventStoreSession OpenSession()
        {
            return new EventStoreSession(this.Directory.OpenSession());
        }

        public void Delete(string name)
        {
            throw new NotImplementedException();
        }
    }
}