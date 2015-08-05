using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Sourcery.EventStores;

namespace Sourcery.AzureTableStorage
{
    public class AzureTableStorageEventStoreFactory
    {
        ConcurrentDictionary<string, bool> exists = new ConcurrentDictionary<string, bool>();
        private readonly string connectionString;

        public AzureTableStorageEventStoreFactory(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public IEventStore GetEventStore(string storeId, string tableName)
        {
            exists.GetOrAdd(tableName, s =>
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
                CloudTable table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
                return true;
            });
            
            return new AzureTableStorageEventStore(connectionString, storeId, tableName);
        }
    }
}
