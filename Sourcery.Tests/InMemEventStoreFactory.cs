using System.Collections.Generic;
using Sourcery.EventStores;

namespace Sourcery.Tests
{
    internal class InMemEventStoreFactory
    {
        private Dictionary<string, IEventStore> stores = new Dictionary<string, IEventStore>();

        public IEventStore Get(string eventStoreId)
        {
            IEventStore store = null;
            if (!stores.TryGetValue(eventStoreId, out store))
            {

                store = new DirectoryEventStore(new InMemDirectory());
                stores[eventStoreId] = store;
            }
            return store;
        }
    }
}