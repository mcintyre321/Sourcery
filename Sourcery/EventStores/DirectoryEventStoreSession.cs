using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic;
using Newtonsoft.Json;
using Sourcery.IO;

namespace Sourcery.EventStores
{
    public class DirectoryEventStoreSession : IDisposable, Sourcery.EventStores.IEventStoreSession
    {
        private readonly IDirectorySession _openSession;

        public DirectoryEventStoreSession(IDirectorySession openSession)
        {
            _openSession = openSession;
        }

        public IEnumerable<SourceryEvent> Events
        {
            get
            {
                return _openSession.EnumerateFiles("*.json").Select(fi => JsonConvert.DeserializeObject<SourceryEvent>(_openSession.ReadAllText(fi)));
            }
        }

        public int Count { get {return  _openSession.Count("*.json"); }}


        public void SaveChanges()
        {
            _openSession.Save();
        }

        public void Write(SourceryEvent @event)
        {
            var json = JsonConvert.SerializeObject(@event, Formatting.Indented, new CustomSerializerSettings());
            _openSession.Write(@event.Index.ToString() + ".json", json);
        }


        public void Dispose()
        {
            _openSession.Dispose();
        }

    }
}