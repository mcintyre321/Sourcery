using System;
using System.Collections.Generic;

namespace Sourcery
{
    internal static class RecurseExtension
    {
        public static IEnumerable<T> Recurse<T>(this T t, Func<T, T> getNext) where T : class
        {
            while (t != null)
            {
                yield return t;
                t = getNext(t);
            }
        }

        public static IEnumerable<T> RecurseMany<T>(this T t, Func<T, IEnumerable<T>> getNext) where T : class
        {
            yield return t;
            foreach (var child in getNext(t))
            {
                var childItems = child.RecurseMany(getNext);
                foreach (var childItem in childItems)
                {
                    yield return childItem;
                }
            }
        }

    }
}