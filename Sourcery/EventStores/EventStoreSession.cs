using System;
using System.Collections.Generic;
using System.Linq;
using Sourcery.IO;

namespace Sourcery.EventStores
{
    public class EventStoreSession : IDisposable
    {
        private readonly IDirectorySession _openSession;

        public EventStoreSession(IDirectorySession openSession)
        {
            _openSession = openSession;
        }

        public IEnumerable<Event> Events
        {
            get
            {
                return _openSession.EnumerateFiles("*.json").Select(fi => new Event()
                {
                    Name = fi.Name.Substring(0, fi.Name.Length - 5),
                    Content = _openSession.ReadAllText(fi)
                });
            }
        }

        public void Write(string name, string serializeObject)
        {
            _openSession.Write(name + ".json", serializeObject);
        }

        public void Save()
        {
            _openSession.Save();
        }

        public void Dispose()
        {
            _openSession.Dispose();
        }
    }
}