using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sourcery
{
    [Serializable]
    public class MethodCommand : CommandBase 
    {
        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
        public string[] Path { get; set; }
        public string TypeName { get; set; }

        public MethodCommand(string typeName, string methodName, object[] arguments, string[] path)
            : base(DateTimeOffset.UtcNow, Gateway.Current)
        {
            TypeName = typeName;
            MethodName = methodName;
            Arguments = arguments;
            Path = path;
        }


        public MethodCommand()
        {
        }

        public override string ShortDescriptionForFilename
        {
            get
            {
                return base.ShortDescriptionForFilename + " " + TypeName + "." + MethodName;
            }
        }
        protected override object Invoke(object o)
        {
            var target = GetTarget(o);
            return target.GetType().GetMethod(MethodName, BindingFlags.Public| BindingFlags.NonPublic | BindingFlags.Instance).Invoke(target, Arguments);
        }

        private object GetTarget(object o)
        {
            var fragments = Path;
            var target = o;

            foreach (var fragment in fragments)
            {
                int counter = 0;
                if (int.TryParse(fragment, out counter))
                {
                    var enumerator = (IEnumerator<object>) target;
                    var i = 0;
                    do
                    {
                        enumerator.MoveNext();
                        i++;
                    } while (i < counter);
                    target = enumerator.Current;
                }
                else
                {
                    var type = target.GetType();

                    var propertyInfo = type.GetProperty(fragment);
                    target = propertyInfo != null ? propertyInfo.GetValue(target, null) : type.GetMethod(fragment).Invoke(target, null);
                }
            }
            return target;
        }
    }
}