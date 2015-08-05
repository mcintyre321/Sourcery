using System.Collections;
using System.Runtime.CompilerServices;

namespace Sourcery
{
    public static class SourcererExtensions
    {
        private static readonly ConditionalWeakTable<object, ISourcedObject> SourcererLookup = new ConditionalWeakTable<object, ISourcedObject>();
        internal static T SetSourcerer<T>(this T t, ISourcedObject<T> sourcedObject)
        {
            SourcererLookup.Add(t, sourcedObject);
            return t;
        }
        public static ISourcedObject<T> Sourcerer<T>(this T o)
        {
            return (ISourcedObject<T>)SourcererLookup.GetValue(o, k => null);
        }
        

    }
}