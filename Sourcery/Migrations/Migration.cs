using Newtonsoft.Json.Linq;

namespace Sourcery.Migrations
{
    public abstract class Migration
    {
        public void Transform(JObject jsonObject)
        {
            var original = jsonObject.DeepClone();
            DoTransform(jsonObject);
            jsonObject["_previous"] = original;
            jsonObject["_version"] = Id;
        }

        protected abstract void DoTransform(JObject jsonObject);

        public abstract int Id { get; }

        public virtual string FriendlyFileName
        {
            get { return this.GetType().Name; }
        }
    }
}
