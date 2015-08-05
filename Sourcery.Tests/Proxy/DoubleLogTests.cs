using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Sourcery.Tests.Proxy
{
    public class DoubleLogTests
    {
        public class Registry
        {
            public static bool throwError;

            public Registry()
            {
                if (throwError) throw new Exception("Constructor must have run twice!");
            }
        }
        [Test]
        public void WontRunConstructorWhenCreatingProxy()
        {
            var s = Guid.NewGuid().ToString();
            var db = new SourceryDb().Get<Registry>(s);
            var model = db.ReadModel;
            Registry.throwError = true;
            var proxy = db.Proxy();
        } 
    }
}