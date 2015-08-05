using System;
using System.Collections.Generic;
using System.Threading;

namespace Sourcery
{
    class PushScope<T> : IDisposable{
        private Stack<T> stack;

        public PushScope(AsyncLocal<Stack<T>> asyncLocal, T instance)
        {
            if (asyncLocal.Value == null)
            {
                asyncLocal.Value = new Stack<T>();
            }
            this.stack = asyncLocal.Value;
            stack.Push(instance);
        }

        public void Dispose()
        {
            stack.Pop();
        }
    }
}