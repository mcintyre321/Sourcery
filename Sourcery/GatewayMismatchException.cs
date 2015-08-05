using System;
using Newtonsoft.Json.Linq;

namespace Sourcery
{
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