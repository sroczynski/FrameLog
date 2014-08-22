using System.Linq;
using System.Threading;
using FrameLog.Example;
using FrameLog.Example.Models;
using NUnit.Framework;

namespace FrameLog.Tests
{
    public abstract class DatabaseBackedTest
    {
        protected ExampleContext db;
        protected User user;

        [TestFixtureSetUp]
        public void CreateDatabase()
        {
            var db = makeDatabase();
            db.Database.Delete();
            db.Database.Create();

            user = new User() { Name = "TestUser" };
            db.Users.Add(user);
            db.Save(user);
        }

        [TestFixtureTearDown]
        public void DeleteDatabase()
        {
            var db = makeDatabase();
            db.Database.Delete();
        }

        [SetUp]
        public void CreateContext()
        {
            db = makeDatabase();
            setupWithDatabase(db);
        }
        protected virtual void setupWithDatabase(ExampleContext db)
        {
            //by default, do nothing
        }
        protected virtual ExampleContext makeDatabase()
        {
            return new ExampleContext();
        }

        [TearDown]
        public void DisposeContext()
        {
            db.Dispose();
        }

        protected Book makeBook()
        {
            var book = new Book() { Title = "The Hobbit" };
            db.Books.Add(book);
            db.Save(user);
            pause();
            return book;
        }
        protected Publisher makePublisher()
        {
            var pub = new Publisher() { Name = "Acme Publishing" };
            db.Publishers.Add(pub);
            db.Save(user);
            pause();
            return pub;
        }
        protected User makeUser()
        {
            var u = new User();
            db.Users.Add(u);
            db.Save(user);
            pause();
            return u;
        }
        protected void pause()
        {
            // We pause to ensure consistent timestamp ordering for logs
            Thread.Sleep(20);
        }
        protected ChangeSet lastChangeSet()
        {
            return db.ChangeSets.OrderByDescending(c => c.Timestamp).First();
        }
    }
}
