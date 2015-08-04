using System.Reflection;
using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Sourcery
{
    public class CustomSerializerSettings : JsonSerializerSettings
    {
        public CustomSerializerSettings()
        {
            this.ContractResolver = new InternalContractResolver();
            TypeNameHandling = TypeNameHandling.All;
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
        }


        class InternalContractResolver : DefaultContractResolver
        {
            //public override JsonContract ResolveContract(System.Type type)
            //{
            //    var c = base.ResolveContract(type);

            //    `if (c.UnderlyingType.Name.EndsWith("Proxy")) c.UnderlngType = c.UnderlyingType.BaseType;
            //    return c;
            //}
            protected override JsonObjectContract CreateObjectContract(System.Type objectType)
            {
                var jsonObjectContract = base.CreateObjectContract(objectType);
                if (objectType.Name.EndsWith("Proxy")) jsonObjectContract.CreatedType = objectType.BaseType;
                return jsonObjectContract;
            }

            public InternalContractResolver()
            {
                this.DefaultMembersSearchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            }
        }
    }
}