using Newtonsoft.Json;

namespace Sourcery.EventStores
{
    public class SourceryEvent
    {
        public int Index { get; set; }

        public SourceryEvent(int index)
        {
            this.Index = index;
        }

        public string Content { get; set; }
        public bool Deleted { get; set; }
        public int Version { get; set; }

        public void SetContent(CommandBase command)
        {
            Content = JsonConvert.SerializeObject(command, new CustomSerializerSettings());
        }

        public CommandBase GetCommand()
        {
            return JsonConvert.DeserializeObject<CommandBase>(Content, new CustomSerializerSettings());
        }
    }
}