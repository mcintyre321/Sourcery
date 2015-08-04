using System;

namespace Sourcery
{
    internal class OnDispose : IDisposable
    {
        private readonly Action _action;

        public OnDispose(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            _action();
        }
    }
}