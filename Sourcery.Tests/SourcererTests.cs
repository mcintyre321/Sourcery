using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Sourcery.EventStores;
using Sourcery.IO;
using FileInfo = Sourcery.IO.FileInfo;

namespace Sourcery.Tests
{
    [TestFixture]
    public class SourcererTests
    {
        private string _bookName = "Catch 22";

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
            var fac = new InMemEventStoreFactory();

            var db = new SourceryDb(fac.Get);
            var sourcerer = db.Get<Egg>("Eggs");

            Assert.False(sourcerer.ReadModel.IsCracked);
            sourcerer.ApplyCommandAndLog(e => e.CrackIt());

            Assert.True(sourcerer.ReadModel.IsCracked);


            var sourcerer2 = new SourceryDb(fac.Get).Get<Egg>("Eggs");
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
            public string Id { get; set; }

            public Book(string title, BookTypes bookType)
            {
                Title = title;
                BookType = bookType;
            }

            public Book(string title, BookTypes bookType, Gateway gw)
            {
                Title = title;
                BookType = bookType;
                Id = gw.Execute("Id", () => Guid.NewGuid().ToString());
            }
            public Book(string title, BookTypes bookType, InitCommand command)
            {
                Title = title;
                BookType = bookType;
                Created = command.Timestamp;
            }

            public DateTimeOffset? Created { get; set; }
        }

        [Test]
        public void CanPersistAnObjectWithAConstructor()
        {
            var fac = new InMemEventStoreFactory();
            var db = new SourceryDb(fac.Get);
            var sourcerer = db.Get<Book>("Book", () => new Book(_bookName, Book.BookTypes.WarComedy));

            Assert.AreEqual(_bookName, sourcerer.ReadModel.Title);


            var sourcerer2 = new SourceryDb(fac.Get).Get<Book>("Book");
            Assert.AreEqual(_bookName, sourcerer2.ReadModel.Title);
        }
 
        [Test]
        public void CanPersistAnObjectWithAConstructorWhichUsesTheCommand()
        {
            var fac = new InMemEventStoreFactory();
            var db = new SourceryDb(fac.Get);
            var sourcerer = db.Get<Book>("Book", () => new Book(_bookName, Book.BookTypes.WarComedy, null as InitCommand));
            Assert.NotNull(sourcerer.ReadModel.Created);

            var sourcerer2 = new SourceryDb(fac.Get).Get<Book>("Book");
            Assert.NotNull(sourcerer2.ReadModel.Created);
        }

        [Test]
        public void ReturnsNullWhenNoObjectIdMatch()
        {
            var fac = new InMemEventStoreFactory();
            var sourceryDb = new SourceryDb(fac.Get);
            Assert.IsFalse(sourceryDb.Exists("objectId"));
        }
        [Test]
        public void ReturnsSomethingOnceAnEventHasBeenLogged()
        {
            var fac = new InMemEventStoreFactory();
            var sourceryDb = new SourceryDb(fac.Get);
            sourceryDb.Get<Egg>("objectId").ApplyCommandAndLog(e => e.CrackIt());
            Assert.IsTrue(sourceryDb.Exists("objectId"));
        }

        [Test]
        public void ReturnsObjectWhenObjectIdMatches()
        {
            //Given an eventstore factgory
            var fac = new InMemEventStoreFactory();
            var sourceryDb = new SourceryDb(fac.Get);
            //And that eventstore factory contains an entity
            var egg = sourceryDb.Get<Egg>("egg1");
            //And that aggregate has had a command applied
            egg.ApplyCommandAndLog(e => e.CrackIt());

            //When a second sourcerydb is based on the eventstorefactory
            var sourceryDb2 = new SourceryDb(fac.Get);
            //And the aggregate is retreived
            var egg2 = sourceryDb2.Get<Egg>("egg1");
            //Then the command should have been applied
            Assert.IsTrue(egg2.ReadModel.IsCracked);
        }

        [Test]
        public void SameInstancesAreReturnedByDifferentGets()
        {
            var fac = new InMemEventStoreFactory();
            var sourceryDb = new SourceryDb(fac.Get);
            var egg = sourceryDb.Get<Egg>("egg1");
            egg.ApplyCommandAndLog(e => e.CrackIt());
            var egg2 = sourceryDb.Get<Egg>("egg1");
            Assert.AreSame(egg.ReadModel, egg2.ReadModel);
        }
        [Test]
        public void SameInstancesAreReturnedBySourcere()
        {
            var fac = new InMemEventStoreFactory();
            var sourceryDb = new SourceryDb(fac.Get);
            var egg = sourceryDb.Get<Egg>("egg1");
            var egg2 = sourceryDb.Get<Egg>("egg1");
            Assert.AreSame(egg.ReadModel, egg2.ReadModel);
        }


    }

    internal class InMemDirectory : SortedDictionary<string, object>, IDirectory
    {
        public void Create()
        {
        }

        public string Name { get; }
        public IDirectorySession OpenSession()
        {
            return new InMemDirectorySession(this);
        }

        public void Delete()
        {
        }

    }

    internal class InMemDirectorySession : IDirectorySession
    {
        private readonly InMemDirectory _parent;
        List<Action> changes = new List<Action>(); 
        public InMemDirectorySession(InMemDirectory parent)
        {
            _parent = parent;
        }

        public void Dispose()
        {
        }

        public void Write(string filename, string content)
        {
            changes.Add(() => _parent[filename] = content);
        }

        public void Save()
        {
            foreach (var change in changes)
            {
                change();
            }
            changes.Clear();
        }

        public IEnumerable<FileInfo> EnumerateFiles(string searchPattern)
        {
            var regex = new Regex(WildcardToRegex(searchPattern), RegexOptions.IgnoreCase);
            return _parent.Keys.Cast<string>()
                .Select(k => new InMemFileInfo(k))
                .Where(fi => regex.IsMatch(fi.Name));
        }

        public string ReadAllText(FileInfo fi)
        {
            return _parent[fi.Name] as string;
        }

        public void Delete()
        {
            _parent.Clear();
        }
        public static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern)
                              .Replace(@"\*", ".*")
                              .Replace(@"\?", ".")
                       + "$";
        }

        public int Count(string searchPattern)
        {
            return EnumerateFiles(searchPattern).Count();
        }
    }

    internal class InMemFileInfo : FileInfo
    {
        public InMemFileInfo(string name)
        {
            Name = name;
        }

        public override string Name { get;  }
    }
}
