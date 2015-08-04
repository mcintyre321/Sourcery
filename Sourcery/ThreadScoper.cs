using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Sourcery
{
    public static class ThreadScoper
    {
        public static IDisposable NewScope()
        {
            return new DisposableScope();
        }
        [ThreadStatic]
        internal static IDictionary scope;
        private static IDictionary Scope
        {
            get
            {
                if (HttpContext.Current != null)
                    return HttpContext.Current.Items;
                return scope ?? (scope = new Hashtable());
            }
        }

        static ThreadScoper()
        {

            Store = (k, v) => Scope[k] = v;
            Retrieve = k => Scope[k];
            GetStack = key =>
            {
                var stack = Retrieve(key) as Stack<object>;
                if (stack == null)
                {
                    stack = new Stack<object>();
                    Store(key, stack);
                }
                return stack;
            };
        }

        static Action<string, object> Store;
        static Func<string, object> Retrieve;
        static Func<string, Stack<object>> GetStack;

        public static T Peek<T>(string key = null)
        {
            key = key ?? typeof(T).FullName;
            return (T)GetStack(key).Peek();
        }

        public static T PeekOrCreate<T>(string key = null, Func<T> create = null)
        {
            key = key ?? typeof(T).FullName;
            var stack = GetStack(key);
            if (stack.Any())
            {
                return (T)stack.Peek();
            }
            create = create ?? Activator.CreateInstance<T>;
            var t = create();
            stack.Push(t);
            return t;
        }


        public static T Pop<T>(string key = null)
        {
            key = key ?? typeof(T).FullName;
            return (T)GetStack(key).Pop();
        }

        public static void Push<T>(T t, string key = null)
        {
            key = key ?? typeof(T).FullName;
            GetStack(key).Push(t);
        }

        public static IDisposable Use<T>(T t, string key = null)
        {
            key = key ?? typeof(T).FullName;
            Push(t, key);
            return new OnDispose(() => Pop<T>(key));
        }

        public static int Count<T>(string key = null)
        {
            key = key ?? typeof(T).FullName;
            return GetStack(key).Count;
        }
    }

    public class DisposableScope : IDisposable
    {
        private IDictionary prev;

        public DisposableScope()
        {
            prev = ThreadScoper.scope;
            ThreadScoper.scope = new Hashtable();
        }
        public void Dispose()
        {
            ThreadScoper.scope = prev;
        }
    }
}