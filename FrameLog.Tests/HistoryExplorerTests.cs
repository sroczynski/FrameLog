using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Linq;
using FrameLog.Example;
using FrameLog.Example.Models;
using FrameLog.History;
using FrameLog.Tests.Helpers;
using NUnit.Framework;
using FrameLog.Example.Models.Testing;

namespace FrameLog.Tests
{
    public class HistoryExplorerTests : DatabaseBackedTest
    {
        private HistoryExplorer<ChangeSet, User> explorer;

        protected override void setupWithDatabase(ExampleContext db)
        {
            base.setupWithDatabase(db);
            explorer = new HistoryExplorer<ChangeSet, User>(db.FrameLogContext);
        }

        [Test]
        public void CanRetrieveChangesToStringProperty()
        {
            var book = makeBook();
            book.Title = "Foo";
            db.Save(user);
            pause();
            book.Title = "Bar";
            db.Save(user);

            var result = explorer.ChangesTo(book, b => b.Title);
            var change1 = result.First();
            var change2 = result.Skip(1).First();

            check("Bar", user, change1);
            check("Foo", user, change2);
        }

        [Test]
        public void CanRetrieveChangesToNavigationProperty()
        {
            var book = makeBook();
            var a = makeBook();
            var b = makeBook();

            book.Sequel = a;
            db.Save(user);
            book.Sequel = b;
            db.Save(user);

            var result = explorer.ChangesTo(book, bk => bk.Sequel);
            var change1 = result.First();
            var change2 = result.Skip(1).First();

            check(b, user, change1);
            check(a, user, change2);
        }

        [Test]
        public void CanRetrieveChangesToNavigationPropertyThatHasBeenNull()
        {
            var book = makeBook();
            var sequel = makeBook();
            book.Sequel = sequel;
            db.Save(user);

            book.Sequel = null;
            db.Save(user);

            var result = explorer.ChangesTo(book, bk => bk.Sequel);
            var change1 = result.First();
            var change2 = result.Skip(1).First();

            check(sequel, user, change2);
            check(null, user, change1);
        }

        [Test]
        public void CanRetrieveChangesToCollectionProperty()
        {
            var publisher = makePublisher();
            var a = makeBook();
            var b = makeBook();

            publisher.Books.Add(a);
            db.Save(user);
            publisher.Books.Add(b);
            db.Save(user);

            var result = explorer.ChangesTo(publisher, bk => bk.Books);
            var change1 = result.First();
            var change2 = result.Skip(1).First();

            check(new List<Book> { a, b }, user, change1, TestHelpers.AreEnumerablesOrderedEqual);
            check(new List<Book> { a }, user, change2, TestHelpers.AreEnumerablesOrderedEqual);
        }

        [Test]
        public void CanRetrieveChangesToEntityCollectionProperty()
        {
            var obj = db.ClassesWithEntityCollection.Add(new ClassWithEntityCollection());
            var a = makeUser();
            var b = makeUser();

            obj.Users.Add(a);
            db.Save(user);
            obj.Users.Add(b);
            db.Save(user);

            var result = explorer.ChangesTo(obj, o => o.Users);
            var change1 = result.First();
            var change2 = result.Skip(1).First();

            check(new EntityCollection<User> { a, b }, user, change1, TestHelpers.AreEnumerablesOrderedEqual);
            check(new EntityCollection<User> { a }, user, change2, TestHelpers.AreEnumerablesOrderedEqual);
        }

        [Test]
        public void ChangesToADifferentObjectDoNotAffectChangesToThisObject()
        {
            var a = makeBook();
            var b = makeBook();
            a.Title = "Foo";
            db.Save(user);
            b.Title = "Bar";
            db.Save(user);

            var result = explorer.ChangesTo(a, bk => bk.Title);
            var change1 = result.First();
            var change2 = result.Skip(1).First();
            check("Foo", user, change1);
            check("The Hobbit", user, change2);
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public void CanRetrieveChangesToAComplexObject()
        {
            var entity = makeBook();
            var book1 = makeBook();
            var book2 = makeBook();

            entity.Title = "Foo";
            entity.NumberOfFans = 0;
            entity.Sequel = book1;
            var a = entity.Copy();
            db.Save(user);

            entity.Title = "Bar";
            entity.NumberOfFans = 1;
            entity.Sequel = book2;
            var b = entity.Copy();
            db.Save(user);

            var result = explorer.ChangesTo(entity);
            var change1 = result.First();
            var change2 = result.Skip(1).First();

            check(b, user, change1, (x, y) => compare(x, y));
            check(a, user, change2, (x, y) => compare(x, y));
        }

        [Test]
        public void CanRetrieveObjectCreationInformation()
        {
            var book = makeBook();
            db.Save(user); pause();
            book.Title = "Foo";
            db.Save(user);

            var result = explorer.ChangesTo(book);
            // The "first" change is the most recent one (setting the title) whereas the 
            // next one is the original creation of the book. We expect the GetCreation
            // method to return the same information as this object.
            var change = result.Skip(1).First();

            var creation = explorer.GetCreation(book);
            Assert.AreEqual(book, creation.Value);
            Assert.AreEqual(change.Author, creation.Author);
            Assert.AreEqual(change.Timestamp, creation.Timestamp);
        }

        [Test]
        public void IfObjectCreationInformationIsNotAvailableExceptionIsThrown()
        {
            db.Logger.Enabled = false;
            var book = makeBook();
            db.Save(user); pause();
            db.Logger.Enabled = true;
            book.Title = "Foo";
            db.Save(user);

            var changes = explorer.ChangesTo(book);
            Assert.AreEqual(1, changes.Count(), "Expected only one change to be logged - the change in title. It looks like maybe the creation event was also logged?");
            Assert.Throws<CreationDoesNotExistInLogException>(() => explorer.GetCreation(book));
        }

        private bool compare(Book a, Book b)
        {
            return a.Title == b.Title
                && a.NumberOfFans == b.NumberOfFans
                && a.Sequel == b.Sequel;
        }

        private void check<T>(T value, User author, IChange<T, User> change, Func<T, T, bool> equalityCheck = null)
        {
            equalityCheck = equalityCheck ?? EqualityComparer<T>.Default.Equals;
            Assert.True(equalityCheck(value, change.Value),
                string.Format("Values were not equal. Expected: '{0}'. Actual: '{1}'", value, change.Value));
            Assert.AreEqual(author, change.Author);
            TestHelpers.IsRecent(change.Timestamp, TimeSpan.FromSeconds(5));
        }
    }
}
