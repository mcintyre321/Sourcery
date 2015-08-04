using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sourcery.Db;
using Sourcery.EventStores;

namespace Sourcery.Tests
{
    [TestFixture]
    public class SourcererTests
    {
        public class Egg
        {
            public bool IsCracked { get; set; }
            public void CrackIt()
            {
                IsCracked = true;
            }
        }
        [Test]
        public void CanPersistAnObject()
        {
            var db = new SourceryDb();
            var sourcerer = db.Get<Egg>("Eggs");
            var eggCommand = new LambdaCommand<Egg>(e => e.CrackIt());

            Assert.False(sourcerer.ReadModel.IsCracked);
            sourcerer.ApplyCommandAndLog(eggCommand);

            Assert.True(sourcerer.ReadModel.IsCracked);


            var sourcerer2 = new SourceryDb().Get<Egg>("Eggs");
            Assert.True(sourcerer2.ReadModel.IsCracked);
        }


        public class Book
        {
            public enum BookTypes
            {
                WarComedy
            }
            public string Title { get; private set; }
            public BookTypes BookType { get; private set; }

            public Book(string title, BookTypes bookType)
            {
                Title = title;
                BookType = bookType;
            }
        }

        [Test]
        public void CanPersistAnObjectWithAConstructor()
        {
            var db = new SourceryDb();
            var sourcerer = db.Get<Book>("Book", new object[] { "Catch 22", Book.BookTypes.WarComedy });

            Assert.AreEqual("Catch 22", sourcerer.ReadModel.Title);


            var sourcerer2 = new SourceryDb().Get<Book>("Book");
            Assert.AreEqual("Catch 22", sourcerer2.ReadModel.Title);
        }


        //CustomSerializerSettings settings = new CustomSerializerSettings();

        //[Test]
        //public void DeserializeSomethingThatNoLongerExists()
        //{
        //    var text = File.ReadAllText(@"D:\projects\Sourcery\Sourcery.Tests\bin\Debug\App_Data\Eggs\634700078771969235.json");
        //    var json = JObject.Parse(text);
        //    var command = JsonConvert.DeserializeObject<EditableLambdaExpression>(json["Action"].ToString(), settings);

        //}

        [Test]
        public void ReturnsNullWhenNoObjectIdMatch()
        {
            var sourceryDb = new SourceryDb();
            Assert.IsFalse(sourceryDb.Exists("objectId"));
        }
        [Test]
        public void ReturnsSomethingOnceAnEventHasBeenLogged()
        {
            var sourceryDb = new SourceryDb();
            sourceryDb.Get<Egg>("objectId").ApplyCommandAndLog(new LambdaCommand<Egg>(e => e.CrackIt()));
            Assert.IsTrue(sourceryDb.Exists("objectId"));
        }

        [Test]
        public void ReturnsObjectWhenObjectIdMatches()
        {
            var sourceryDb = new SourceryDb();
            var egg = sourceryDb.Get<Egg>("egg1");
            egg.ApplyCommandAndLog(new LambdaCommand<Egg>(e => e.CrackIt()));

            var sourceryDb2 = new SourceryDb();
            var egg2 = sourceryDb2.Get<Egg>("egg1");
            Assert.IsTrue(egg2.ReadModel.IsCracked);
        }

        [Test]
        public void SameInstancesAreReturnedByDifferentGets()
        {
            var sourceryDb = new SourceryDb();
            var egg = sourceryDb.Get<Egg>("egg1");
            egg.ApplyCommandAndLog(new LambdaCommand<Egg>(e => e.CrackIt()));
            var egg2 = sourceryDb.Get<Egg>("egg1");
            Assert.AreSame(egg.ReadModel, egg2.ReadModel);
        }
        [Test]
        public void SameInstancesAreReturnedBySourcere()
        {
            var sourceryDb = new SourceryDb();
            var egg = sourceryDb.Get<Egg>("egg1");
            var egg2 = sourceryDb.Get<Egg>("egg1");
            Assert.AreSame(egg.ReadModel, egg2.ReadModel);
        }


    }

    
}
