using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Expression = System.Linq.Expressions.Expression;

namespace Sourcery
{
    [Serializable]
    public class MethodCommand : CommandBase 
    {
        public string MethodName { get; set; }
        public object[] Arguments { get; set; }
        public string[] Path { get; set; }
        public string TypeName { get; set; }

        internal static MethodCommand CreateFromLambda<T>(Expression<Action<T>> exp, object context = null)
        {
            var mce = (MethodCallExpression)exp.Body;

            var args = mce.Arguments
                .Select(GetValueFromExpression)
                .ToArray();

            var path = new List<string>();
            var propertyExpression = ((MethodCallExpression)exp.Body).Object as MemberExpression;
            while (propertyExpression != null)
            {
                path.Add(propertyExpression.Member.Name);
                propertyExpression = propertyExpression.Expression as MemberExpression;
            }

            return new MethodCommand(mce.Method.DeclaringType, mce.Method.Name, args, path.ToArray(), DateTimeOffset.UtcNow, new Gateway());
        }
        public static MethodCommand CreateFromLambda<T, TOut>(Expression<Func<T, TOut>> exp, object context = null)
        {
            var mce = (MethodCallExpression)exp.Body;

            var args = mce.Arguments
                .Select(GetValueFromExpression)
                .ToArray();

            var path = new List<string>();
            var propertyExpression = ((MethodCallExpression)exp.Body).Object as MemberExpression;
            while (propertyExpression != null)
            {
                path.Add(propertyExpression.Member.Name);
                propertyExpression = propertyExpression.Expression as MemberExpression;
            }

            return new MethodCommand(mce.Method.DeclaringType, mce.Method.Name , args, path.ToArray(), DateTimeOffset.UtcNow, new Gateway());
        }


        static object GetValueFromExpression(Expression exp)
        {
            var constant = exp as ConstantExpression;
            if (constant != null)
            {
                return constant.Value;
            }
            var memberAccess = exp as MemberExpression;
            if (memberAccess != null)
            {
                var constantSelector = (ConstantExpression)memberAccess.Expression;
                return ((dynamic)memberAccess.Member).GetValue(constantSelector.Value);
            }
            throw new NotImplementedException();
        }
        public MethodCommand(Type type, string methodName, object[] arguments, string[] path, DateTimeOffset now, Gateway gateway)
            : this(type.AssemblyQualifiedName, methodName, arguments, path, now, gateway)
        {
        }

        public MethodCommand(string typeName, string methodName, object[] arguments, string[] path, DateTimeOffset now, Gateway gateway)
            : base(now, gateway)
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