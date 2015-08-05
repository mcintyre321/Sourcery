using System.Collections;
using System.Runtime.CompilerServices;

namespace Sourcery
{
    public static class SourcererExtensions
    {
        private static readonly ConditionalWeakTable<object, ISourcerer> SourcererLookup = new ConditionalWeakTable<object, ISourcerer>();
        internal static T SetSourcerer<T>(this T t, ISourcerer<T> sourcerer)
        {
            SourcererLookup.Add(t, sourcerer);
            return t;
        }
        public static ISourcerer<T> Sourcerer<T>(this T o)
        {
            return (ISourcerer<T>)SourcererLookup.GetValue(o, k => null);
        }
        

    }
}