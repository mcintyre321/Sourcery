using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sourcery
{
    public class Gateway
    {
        public static Gateway Current
        {
            get { return ThreadScoper.Count<Gateway>() == 0 ? null : ThreadScoper.Peek<Gateway>(); }
        }

        [JsonIgnore]
        Hashtable Cache { get; set; }
        public T TryGetFromCache<T>(string key, Func<T> create)
        {
            return (T) (Cache[key] ?? (T) (Cache[key] = create()));
        }


        public JArray Results { get; set; }

        public Gateway()
        {
            Results = new JArray();
            Cache = new Hashtable();
        }

        public void ExecuteVoid(Action a)
        {
            if (ResultCounter == null)
            {
                a();
            }
        }
        public void ExecuteVoid(Func<object> a)
        {
            if (ResultCounter == null)
            {
                a();
            }
        }
        public void Execute(Action a)
        {
            if (ResultCounter == null)
            {
                a();
            }
        }

        private static JsonSerializer serializer = JsonSerializer.Create(new CustomSerializerSettings());
        public T Execute<T>(string identifier, Func<T> a)
        {
            if (string.IsNullOrWhiteSpace(identifier))
                throw new InvalidOperationException("Must provide identifier to gateway so logged command has context");
            if (ResultCounter == null)
            {
                var obj = new JObject();
                obj["key"] = identifier;
                var result = a();
                obj["value"] = JToken.FromObject(result, serializer);
                Results.Add(obj);
                return result;
            }
            else
            {
                var data = Results[ResultCounter++];
                
                var gatewayKey = data["key"].Value<string>();
                if (gatewayKey != identifier)
                {
                    throw new GatewayMismatchException(requestedKey: identifier, foundKey: gatewayKey,
                                                       foundValue: data["value"], desiredType: typeof(T));
                }
                var foundValue = data["value"].ToObject<T>(serializer);

                return foundValue;
            }
        }

        public int? ResultCounter { private get; set; }
    }

    public class GatewayMismatchException : Exception
    {
        public string RequestedKey { get; private set; }
        public string FoundKey { get; private set; }
        public JToken  FoundValue { get; private set; }
        public Type DesiredType { get; private set; }

        public GatewayMismatchException(string requestedKey, string foundKey, JToken foundValue, Type desiredType)

            : base("Looked for '" + requestedKey + "', found '" + foundKey + "'")
        {
            this.RequestedKey = requestedKey;
            this.FoundKey = foundKey;
            this.FoundValue = foundValue;
            DesiredType = desiredType;
        }

    }
}
