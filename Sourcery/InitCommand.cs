using System;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;

namespace Sourcery
{
    public class InitCommand : CommandBase
    {
        public InitCommand(Gateway current)
            :base(DateTimeOffset.Now, current)
        {
        }

        public string Type { get; set; }
        protected override object Invoke(object target)
        {
            return null;
        }

        public JObject Arguments { get; set; }

        
    }
}