using System;
using System.Collections.Generic;
using System.Linq;

namespace Sourcery
{
    public class UpdateException : AggregateException
    {
        public UpdateException(IEnumerable<RebuildException> commandExceptions) : base(BuildMessage(commandExceptions), commandExceptions)
        {
        }

        private static string BuildMessage(IEnumerable<RebuildException> commandExceptions)
        {
            return "There were errors\r\n" + string.Join("\r\n", commandExceptions.Select(ce => ce.Message));
        }
    }
}