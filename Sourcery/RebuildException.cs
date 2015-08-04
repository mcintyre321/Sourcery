using System;

namespace Sourcery
{
    public sealed class RebuildException : Exception
    {
        public RebuildException(Exception ex) : base("Error rebuilding:" + ex.Message, ex)
        {
        }

        public CommandBase Command { get; set; }

        public string Name { get; set; }

        public string Json { get; set; }

        public ISourcerer Sourcerer { get; set; }

        public object Model { get; set; }
    }
}