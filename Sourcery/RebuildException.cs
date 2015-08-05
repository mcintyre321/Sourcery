using System;
using Sourcery.EventStores;

namespace Sourcery
{
    public sealed class RebuildException : Exception
    {
        public RebuildException(Exception ex, SourceryEvent @event)
            : base(MakeMessage(@event.Index.ToString(), ex, @event.Content), ex)
        {
            Event = @event;
        }

        private static string MakeMessage(string name, Exception inner, string json)
        {
            return string.Format("Error rebuilding command '{0}' - {1}\r\nContent: \r\n{2}\r\n", name, inner.Message, json);
        }

        public SourceryEvent Event { get; private set; }
    }
}