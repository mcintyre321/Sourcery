﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sourcery
{
    

    public class Gateway
    {
         


        public JObject Results { get; set; }

        public Gateway()
        {
            Results = new JObject();
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

        public T Execute<T>(string key, Func<T> a)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Must provide identifier to gateway so logged command has context");
            var serializer1 = JsonSerializer.Create(new CustomSerializerSettings());
            if (ResultCounter == null)
            {
                var existing = Results[key];
                if (existing != null) return existing.ToObject<T>();
                var o = a();
                Results.Add(key, o == null ? null : JToken.FromObject(o, serializer1));
                return o;
            }
            else
            {
                var data = Results[key];

                var foundValue = data.ToObject<T>(serializer1);

                return foundValue;
            }
        }

        public int? ResultCounter { private get; set; }
    }
}
