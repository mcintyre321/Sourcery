using System.Collections;
using System.Runtime.CompilerServices;

namespace Sourcery.Proxy
{
    public static class ProxyLookupExtensions
    {
        private static readonly ConditionalWeakTable<object, object> ProxyLookup = new ConditionalWeakTable<object, object>();
        internal static T SetProxy<T>(this T t, object proxy)
        {
            ProxyLookup.Remove(t);
            ProxyLookup.Add(t, proxy);
            return t;
        }
        public static T GetProxy<T>(this T t, object proxy) where T : class
        {
            return ProxyLookup.GetValue(t, k => null) as T; ;
        }
    }
}