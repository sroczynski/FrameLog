using System;
using System.Linq;
using FrameLog.Example.Models;
using FrameLog.Tests.Helpers;
using NUnit.Framework;

namespace FrameLog.Tests
{
    public class LoggingTests : DatabaseBackedTest
    {
        [Test]
        public void AuthorIsLogged()
        {
            makeABookAndChangeItsName();
            var changeset = lastChangeSet();
            Assert.AreEqual(user, changeset.Author);
        }

        [Test]
        public void TimestampIsLogged()
        {
            makeABookAndChangeItsName();
            var changeset = lastChangeSet();
            TestHelpers.IsRecent(changeset.Timestamp, TimeSpan.FromSeconds(5));
        }

        [Test]
        public void ChangesToTextPropertyAreLogged()
        {
            var book = makeABookAndChangeItsName();
            var propertyChange = expectOnePropertyChangeForEntity(book);
            checkPropertyChange(propertyChange, "Title", book.Title);
        }

        [Test]
        public void ChangesToIntegerPropertyAreLogged()
        {
            var book = makeBook();
            book.NumberOfFans = 100;
            db.Save(user);

            var propertyChange = expectOnePropertyChangeForEntity(book);
            checkPropertyChange(propertyChange, "NumberOfFans", 100);
        }

        [Test]
        public void ChangesToRelationshipPropertyAreLogged()
        {
            var book = makeBook();
            var sequel = makeBook();
            book.Sequel = sequel;
            db.Save(user);

            var propertyChange = expectOnePropertyChangeForEntity(book);
            checkPropertyChange(propertyChange, "Sequel", sequel.Id);
        }

        [Test]
        public void SettingRelationshipPropertyToNullIsLogged()
        {
            var book = makeBook();
            var sequel = makeBook();
            book.Sequel = sequel;
            db.Save(user);

            book.Sequel = null;
            db.Save(user);

            var propertyChange = expectOnePropertyChangeForEntity(book);
            checkPropertyChange(propertyChange, "Sequel", null);
        }

        [Test]
        public void ChangesToSetAreLogged()
        {
            var b1 = makeBook();
            var b2 = makeBook();
            var b3 = makeBook();
            var p = makePublisher();

            // Can add one
            p.Books.Add(b1);
            db.Save(user);
            checkSetLog(p, b1);

            // Can add more than one at once
            p.Books.Add(b2);
            p.Books.Add(b3);
            db.Save(user);
            checkSetLog(p, b1, b2, b3);

            // Can remove one
            p.Books.Remove(b1);
            db.Save(user);
            checkSetLog(p, b2, b3);
            
            // Can add and remove at once
            p.Books.Add(b1);
            p.Books.Remove(b2);
            db.Save(user);
            checkSetLog(p, b1, b3);

            // Can remove more than one at once
            p.Books.Remove(b1);
            p.Books.Remove(b3);
            db.Save(user);
            checkSetLog(p);
        }
        private void checkSetLog(Publisher pub, params Book[] books)
        {
            var change = expectOnePropertyChangeForEntity(pub);
            Assert.AreEqual("Books", change.PropertyName);
            Assert.AreEqual(string.Join(",", books.Select(i => db.FrameLogContext.GetReferenceForObject(i))), change.Value);
        }

        [Test]
        public void MultipleChangesWithASingleSaveResultInASingleChangeset()
        {
            var a = makeBook();
            var b = makeBook();
            var count = db.ChangeSets.Count();

            a.NumberOfFans++;
            b.Title += ": An unexpected journey";
            db.Save(user);

            Assert.AreEqual(1, db.ChangeSets.Count() - count, "Expected just one changeset");
        }

        [Test]
        public void MultipleSavesResultInMultipleChangesets()
        {
            var book = makeBook();
            var count = db.ChangeSets.Count();

            book.NumberOfFans++;
            db.Save(user);
            book.NumberOfFans++;
            db.Save(user);

            Assert.AreEqual(2, db.ChangeSets.Count() - count, "Expected two changesets");
        }

        [Test]
        public void ChangesAreNotLoggedWhenLoggingIsDisabled()
        {
            db.Logger.Enabled = false;
            var book = makeBook();

            string reference = book.Id.ToString();
            string typeName = typeof(Book).Name;
            var matches = db.ObjectChanges.Where(o => o.TypeName == typeName && o.ObjectReference == reference);
            Assert.IsFalse(matches.Any(), "Found matching log items, expected none");
        }

        private void checkPropertyChange(PropertyChange propertyChange, string expectedName, object value)
        {
            Assert.AreEqual(expectedName, propertyChange.PropertyName);
            Assert.AreEqual(toString(value), propertyChange.Value);

            if (value is int)
                Assert.AreEqual(value, propertyChange.ValueAsInt);
            else
                Assert.Null(propertyChange.ValueAsInt);
        }

        private PropertyChange expectOnePropertyChangeForEntity(object entity)
        {
            var changeset = lastChangeSet();
            string reference = db.FrameLogContext.GetReferenceForObject(entity);

            Assert.AreEqual(1, changeset.ObjectChanges.Count(), 
                "Expected just one object change. Instead we had the following: {0}", string.Join(", ", changeset.ObjectChanges));
            var objectChange = changeset.ObjectChanges.Single();
            Assert.AreEqual(entity.GetType().Name, objectChange.TypeName, "The object change was for an entity of the wrong type");
            Assert.AreEqual(reference, objectChange.ObjectReference, "The object change was for the wrong entity (ID mismatch)");

            Assert.AreEqual(1, objectChange.PropertyChanges.Count(),
                "Expected just one property change. Instead we had the following: {0}", string.Join(", ", objectChange.PropertyChanges));
            return objectChange.PropertyChanges.Single();
        }

        private string toString(object value)
        {
            if (value == null)
                return null;
            else 
                return value.ToString();
        }

        private Book makeABookAndChangeItsName()
        {
            var book = makeBook();
            string oldTitle = book.Title;
            string newTitle = book.Title = oldTitle + "New";
            db.Save(user);
            pause();
            return book;
        }
    }
}
