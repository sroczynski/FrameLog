using FrameLog.Example;
using FrameLog.Example.Models;
using FrameLog.Exceptions;
using NUnit.Framework;
using System.Linq;
using System.Transactions;

namespace FrameLog.Tests
{
    /// <summary>
    /// In addition to the tests here it would be good to check that, in the absence
    /// of an explicit transaction wrapped around the call to SaveChanges, FrameLog
    /// really is doing its work within a transaction it creates itself, but that
    /// can't be done by these kind of functional unit tests.
    /// 
    /// Note that in these tests we pass through the default ExampleContext.Save 
    /// method to interact directly with the FrameLogModule.SaveChanges overloads.
    /// </summary>
    public class TransactionTests : DatabaseBackedTest
    {
        /// <summary>
        /// We check that the changes that FrameLog makes to the database are really
        /// wrapped in the transaction we opened on the database context.
        /// 
        /// We do this by creating a second database context, and seeing at what point
        /// this second can see the changes made by first - we shouldn't be able to
        /// see anything until the transaction is committed. (Because EF uses
        /// Read Commited isolation level by default).
        /// </summary>
        [Test]
        public void FrameLogMakesUseOfDbContextTransaction()
        {
            Book book;
            var db2 = new ExampleContext();
            using (var transaction = db.Database.BeginTransaction())
            {
                book = new Book() { Title = "Title" };
                db.Books.Add(book);
                db.Logger.SaveChangesWithinExplicitTransaction(user);

                Assert.IsNull(db2.Books.SingleOrDefault(b => b.Id == book.Id));
                transaction.Commit();
            }
            Assert.IsNotNull(db2.Books.SingleOrDefault(b => b.Id == book.Id));
        }

        /// <summary>
        /// We check that the changes that FrameLog makes to the database are really
        /// wrapped in the transaction scope we wrapped the save changes call in.
        /// 
        /// We do this by, during the transaction, opening a new transaction scope
        /// with TransactionScopeOption.Suppress. This means that the code inside
        /// this scope takes place outside the current transaction.
        /// </summary>
        [Test]
        public void FrameLogMakesUseOfTransactionScope()
        {
            Book book;
            using (var transaction = new TransactionScope())
            {
                book = new Book() { Title = "Title" };
                db.Books.Add(book);
                db.Logger.SaveChanges(user);

                using (var outsideTransaction = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    Assert.IsNull(db.Books.SingleOrDefault(b => b.Id == book.Id));
                }
                transaction.Complete();
            }
            Assert.IsNotNull(db.Books.SingleOrDefault(b => b.Id == book.Id));
        }

        /// <summary>
        /// If you try and use DbContextTransactions, but use the SaveChanges overload
        /// that uses a TransactionScope, you get an exception.
        /// </summary>
        [Test]
        public void ErrorIsThrownWhenTryingToMixTransactionSystems()
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var book = new Book() { Title = "Title" };
                db.Books.Add(book);
                Assert.Throws<ConflictingTransactionException>(() => db.Logger.SaveChanges(user));
                transaction.Rollback();
            }
        }

        /// <summary>
        /// If you use DbContextTransactions with the SaveChanges overload that
        /// supports it, everything works.
        [Test]
        public void NoErrorIsThrownIfExplicitTransactionIsPassedToFrameLog()
        {
            using (var transaction = db.Database.BeginTransaction())
            {
                var book = new Book() { Title = "Title" };
                db.Books.Add(book);
                Assert.DoesNotThrow(() => db.Logger.SaveChangesWithinExplicitTransaction(user));
                transaction.Rollback();
            }
        }
    }
}
