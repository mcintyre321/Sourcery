using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Sourcery.EventStores;

namespace Sourcery.AzureTableStorage
{
    public class AzureTableStorageEventStoreSession : IEventStoreSession
    {
        private readonly string connectionString;
        private readonly string id;
        private readonly string tableName;

        public AzureTableStorageEventStoreSession(string connectionString, string id, string tableName)
        {
            this.connectionString = connectionString;
            this.id = id;
            this.tableName = tableName;
            events = new Lazy<Dictionary<string, SourceryEvent>>(() =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference(tableName);
                var eventEntities = table.CreateQuery<EventTableEntity>().Where(e => e.PartitionKey == id);
                
                return eventEntities.ToDictionary(e => e.RowKey, e => new SourceryEvent(int.Parse(e.RowKey)) {  Content = e.Content, Deleted = e.Deleted, Version = e.Version});
            });
        }

        void IDisposable.Dispose()
        {
        
        }

        private readonly Lazy<Dictionary<string, SourceryEvent>> events;
        private Dictionary<string, SourceryEvent> newEvents = new Dictionary<string, SourceryEvent>(); 
        public IEnumerable<SourceryEvent> Events
        {
            get { return events.Value.Select(e => e.Value); }
        }

        public int Count { get { return @events.Value.Count; }}

        public void SaveChanges()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            var entries = newEvents.Select(e => new EventTableEntity()
            {
                PartitionKey = id, 
                RowKey = e.Value.Index.ToString().PadLeft(8, '0'), 
                Content = e.Value.Content
            });
            var insert = new TableBatchOperation();
            foreach (var eventTableEntity in entries)
            {
                insert.Add(TableOperation.InsertOrReplace(eventTableEntity));
            }
            table.ExecuteBatch(insert);
            newEvents.Clear();
        }

        public void Write(SourceryEvent eventRecord)
        {
            var rk = eventRecord.Index.ToString().PadLeft(8, '0');
            events.Value[rk] = eventRecord;
            newEvents[rk] = eventRecord;
        }

    }
}