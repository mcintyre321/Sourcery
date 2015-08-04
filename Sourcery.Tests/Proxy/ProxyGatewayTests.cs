using System;
using NUnit.Framework;
using Sourcery.Db;
using Sourcery.Proxy;

namespace Sourcery.Tests.Proxy
{
    public class ProxyGatewayTests
    {
        private static DateTimeOffset Now = DateTimeOffset.Now;
        public class Thing
        {
            public virtual DateTimeOffset? Then { get; set; }
            public virtual void MethodThatGetsTime()
            {
                Then = Gateway.Current.Execute("the time", () => DateTimeOffset.Now);
            }
        }

        [Test]
        public void CanUseGatewayInProxy()
        {
            var s = Guid.NewGuid().ToString();
            var db = new SourceryDb().Get<Thing>(s);
            var model = db.ReadModel;
            var proxy = db.Proxy();
            proxy.MethodThatGetsTime();


            var db2 = new SourceryDb().Get<Thing>(s);
            var model2 = db2.ReadModel;
            Assert.NotNull(model2.Then);

        }
    }
}