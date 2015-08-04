using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sourcery.Web
{
    public static class HandyExtensions
    {
        public static T Then<T>(this T t, Action<T> action)
        {
            action(t);
            return t;
        }
        public static IEnumerable<T> Recurse<T>(this T t, Func<T, T> getNext) where T : class
        {
            while (t != null)
            {
                yield return t;
                t = getNext(t);
            }
        }

        public static IEnumerable<T> Recurse<T>(this T t, Func<T, IEnumerable<T>> getNext) where T : class
        {
            yield return t;
            foreach (var child in getNext(t))
            {
                var childItems = child.Recurse(getNext);
                foreach (var childItem in childItems)
                {
                    yield return childItem;
                }
            }
        } 
    }
}
