using System;
using System.Collections.Generic;
using Sourcery.IO;

namespace Sourcery.EventStores
{
    public class Event
    {
        public string Name { get; set; }
        public string Content { get; set; }
    }
    public interface IEventStore
    {
        string Id { get; }
        void Delete(string name);
        void LogEvent(string name, string content);
        EventStoreSession OpenSession();
    }

}