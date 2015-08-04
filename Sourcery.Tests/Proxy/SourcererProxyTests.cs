using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Sourcery.Db;
using Sourcery.EventStores;
using Sourcery.Proxy;

namespace Sourcery.Tests.Proxy
{
    [TestFixture]
    public class SourcererProxyTests
    {
        public class Basket
        {
            public Basket()
            {
                _eggs.Add(new Egg());
                _eggs.Add(new Egg());
            }

            private IList<Egg> _eggs = new List<Egg>();

            public virtual IEnumerable<Egg> Eggs
            {
                get { return _eggs; }
            }
        }

        public class Cup
        {
            public virtual Egg Egg { get; set; }

        }

        public class CupWithEgg : Cup
        {
            public CupWithEgg()
            {
                Egg = new Egg();
            }

            public virtual Egg GetTheEgg()
            {
                return Egg;
            }
        }

        public class Egg
        {
            public virtual bool IsCracked { get; set; }

            public virtual void CrackIt()
            {
                IsCracked = true;
            }
        }

        [Test]
        public void CanPersistAMethodCall()
        {
            var db = new SourceryDb();
            var objectid = Guid.NewGuid().ToString();
            var egg = db.Get<Egg>(objectid).Proxy();


            Assert.False(egg.IsCracked);
            egg.CrackIt();
            Assert.True(egg.IsCracked);

            var sourcerer2 = new SourceryDb().Get<Egg>(objectid);
            Assert.True(sourcerer2.ReadModel.IsCracked);
        }

        [Test]
        public void CanPersistASetProperty()
        {
            var db = new SourceryDb();
            var objectid = Guid.NewGuid().ToString();
            var egg = db.Get<Egg>(objectid).Proxy();

            Assert.False(egg.IsCracked);
            egg.IsCracked = true;
            Assert.True(egg.IsCracked);

            var sourcerer2 = new SourceryDb().Get<Egg>(objectid);
            Assert.True(sourcerer2.ReadModel.IsCracked);
        }

        [Test]
        public void CanPersistACallToAChild()
        {
            var objectid = Guid.NewGuid().ToString();
            var cup = new SourceryDb()
                .Get<CupWithEgg>(objectid)
                .Proxy();
            Assert.False(cup.Egg.IsCracked);
            cup.Egg.IsCracked = true;
            Assert.True(cup.Egg.IsCracked);

            var sourcerer2 = new SourceryDb().Get<CupWithEgg>(objectid);
            Assert.True(sourcerer2.ReadModel.Egg.IsCracked);
        }

        [Test]
        public void CanPersistACallToAChildRetreivedViaAMethodCall()
        {
            var db = new SourceryDb();
            var cup = db.Get<CupWithEgg>("Cup123").Proxy();
            Assert.False(cup.Egg.IsCracked);
            cup.GetTheEgg().IsCracked = true;
            Assert.True(cup.Egg.IsCracked);

            var sourcerer2 = new SourceryDb().Get<CupWithEgg>("Cup123");
            Assert.True(sourcerer2.ReadModel.Egg.IsCracked);
        }

        [Test]
        public void CanPersistACallToSetChild()
        {
            var s = Guid.NewGuid().ToString();
            var cup = new SourceryDb().Get<Cup>(s).Proxy();
            cup.Egg = new Egg();
            var sourcerer2 = new SourceryDb().Get<Cup>(s);
            Assert.True(sourcerer2.ReadModel.Egg != null);
        }

        [Test]
        public void CanPersistACallToAnEnumerable()
        {
            var s = Guid.NewGuid().ToString();
            var basket = new SourceryDb().Get<Basket>(s).Proxy();
            var enumerator = basket.Eggs.GetEnumerator();
            enumerator.MoveNext();
            enumerator.Current.CrackIt();
            var sourcerer2 = new SourceryDb().Get<Basket>(s);
            Assert.True(sourcerer2.ReadModel.Eggs.First().IsCracked);
        }
    }
}
