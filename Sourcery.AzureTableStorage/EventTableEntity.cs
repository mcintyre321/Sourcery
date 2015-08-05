using Microsoft.WindowsAzure.Storage.Table;

namespace Sourcery.AzureTableStorage
{
    public class EventTableEntity : TableEntity
    {
        public string Content { get; set; }
        public bool Deleted { get; set; }
        public int Version { get; set; }
    }
}