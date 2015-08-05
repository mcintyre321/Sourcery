using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Sourcery.EventStores;

namespace Sourcery.AzureTableStorage
{
    public class AzureTableStorageEventStore : IEventStore
    {
        private readonly string connectionString;
        private string tableName;

        public AzureTableStorageEventStore(string connectionString, string storeId, string tableName)
        {
            this.connectionString = connectionString;
            this.Id = storeId;
            this.tableName = tableName;
        }

        public string Id { get; private set; }

        public void Delete()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            var entries = table.CreateQuery<EventTableEntity>().Where(e => e.PartitionKey == Id).ToArray();
            var delete = new TableBatchOperation();
            foreach (var eventTableEntity in entries)
            {
                delete.Add(TableOperation.Delete(eventTableEntity));
            }
            table.ExecuteBatch(delete);
        }

        

        public IEventStoreSession OpenSession()
        {
            return new AzureTableStorageEventStoreSession(connectionString, Id, tableName);
        }
    }
}