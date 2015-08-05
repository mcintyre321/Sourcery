using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sourcery.EventStores;

namespace Sourcery.Migrations
{
    public abstract class Migration
    {
        public void Transform(SourceryEvent @event, bool isInit)
        {
            var newJsonContent = TransformRawJson(@event.Content);
            var obj = JObject.Parse(newJsonContent);
            if (isInit == false)
            {
                obj = DoTransform(obj);
                obj["_previous"] = JObject.Parse(@event.Content);
            }
            @event.Version = VersionNumber;
            @event.Content = obj.ToString(Formatting.Indented);
        }
        protected virtual string TransformRawJson(string rawJson)
        {
            return rawJson;
        }
        protected virtual JObject DoTransform(JObject jsonObject)
        {
            return jsonObject;
        }

        public abstract int VersionNumber { get; }

        public virtual string FriendlyFileName
        {
            get { return this.GetType().Name; }
        }

        
    }
}
