using System;
using Sourcery.Helpers;

namespace Sourcery
{
    public abstract class CommandBase
    {
        public DateTimeOffset Timestamp { get; set; }
        public CommandBase(DateTimeOffset timestamp, Gateway gateway)
            : this()
        {
            Timestamp = timestamp;
            Gateway = gateway;
        }
        public CommandBase()
            : base()
        {
            Gateway = new Gateway();
        }
        public Gateway Gateway { get; set; }

        public virtual string ShortDescriptionForFilename => "";


        public object Apply(object target)
        {
            return Invoke(target);
        }

        protected abstract object Invoke(object target);
    }
}