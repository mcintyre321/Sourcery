using System;
using System.Collections.Generic;
using System.Linq;
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
             

            protected override List<MemberInfo> GetSerializableMembers(Type objectType)
            {
                var bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
                return
                    (objectType.GetProperties(bindingFlags)).Cast<MemberInfo>()
                    .Concat(objectType.GetFields(bindingFlags))
                        .ToList();
            }
        }
    }
}