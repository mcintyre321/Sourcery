using System;

namespace Sourcery
{
    public abstract class CommandBase
    {
        public DateTimeOffset Timestamp { get; set; }
        public int _version { get; set; }

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
        public Gateway Gateway { get; private set; }

        public virtual string ShortDescriptionForFilename
        {
            get { return ""; }
        }

        public object Apply(object target)
        {
            using (ThreadScoper.Use(this.Gateway))
            {
                return Invoke(target);
            }
        }

        protected abstract object Invoke(object target);
    }
}