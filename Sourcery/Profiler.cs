using System;

namespace Sourcery
{
    public static class Profiler
    {
        public static Func<string, IDisposable> DoStep { get; set; }
        public static IDisposable Step(string message)
        {
            if (DoStep != null) return DoStep(message);
            return new DummyDisposable();
        }
    }

    class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
}