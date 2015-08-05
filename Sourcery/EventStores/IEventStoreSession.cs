using System;
using Newtonsoft.Json.Linq;

namespace Sourcery.EventStores
{
    public interface IEventStoreSession: IDisposable
    {
        
        System.Collections.Generic.IEnumerable<SourceryEvent> Events { get; }
        int Count { get; }
        void SaveChanges();
        void Write(SourceryEvent eventRecord);
    }
}
