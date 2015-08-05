using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sourcery
{
    public static class JTokenExtension
    {
        public static object ToObject(this JToken token, Type targetType, JsonSerializerSettings settings)
        {
            var jsonSerializer = JsonSerializer.Create(settings);
            using (var jtokenReader = new JTokenReader(token))
            {
                return jsonSerializer.Deserialize(jtokenReader, targetType);
            }
        }
    }
}