using System;
using System.Collections.Generic;
using Sourcery.IO;

namespace Sourcery.EventStores
{
    public interface IEventStore
    {
        string Id { get; }
        void Delete();
        IEventStoreSession OpenSession();
    }

     
}