using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sourcery
{
    

    public class Gateway
    {
        public static AsyncLocal<Stack<Gateway>> Stack { get; } = new AsyncLocal<Stack<Gateway>>();
         

        public static Gateway Current { get { return Stack.Value.Peek(); } }

        [JsonIgnore]
        Hashtable Cache { get; set; }
        public T TryGetFromCache<T>(string key, Func<T> create)
        {
            return (T) (Cache[key] ?? (T) (Cache[key] = create()));
        }


        public JObject Results { get; set; }

        public Gateway()
        {
            Results = new JObject();
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

        public T Execute<T>(string key, Func<T> a)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Must provide identifier to gateway so logged command has context");
            if (ResultCounter == null)
            {
                var existing = Results[key];
                if (existing != null) return existing.ToObject<T>();
                var o = a();
                Results.Add(key, o == null ? null : JToken.FromObject(o, serializer));
                return o;
            }
            else
            {
                var data = Results[key];

                var foundValue = data.ToObject<T>(serializer);

                return foundValue;
            }
        }

        public int? ResultCounter { private get; set; }
    }
}
