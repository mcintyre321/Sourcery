using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            var ctors = target.GetType().GetConstructors()
                .OrderByDescending(c => c.GetParameters().Count());
            var ctorAndArgs = ctors
                .Select(c => new {ctor = c, args = MakeArray(c, Arguments, this)})
                .First(pair => pair.args != null);
            ctorAndArgs.ctor.Invoke(target, ctorAndArgs.args);
            return null;
        }

        public JObject Arguments { get; set; }

        private static object[] MakeArray(ConstructorInfo constructorInfo, JObject arguments, CommandBase command)
        {
            var parameters = constructorInfo.GetParameters();

            List<object> args = new List<object>();
            foreach (var parameter in parameters)
            {
                if (parameter.ParameterType == command.GetType())
                {
                    args.Add(command);
                    continue;
                }
                if (arguments[parameter.Name] == null) return null;
                var token = arguments[parameter.Name];
                var arg = token.ToObject(parameter.ParameterType, new CustomSerializerSettings());
                args.Add(arg);
            }
            return args.ToArray();
        }

    }
}