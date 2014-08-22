using FrameLog.Contexts;
using FrameLog.History.Binders;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FrameLog.Tests
{
    public class BindingTests
    {
        private BindManager manager;

        [SetUp]
        public void CreateManager()
        {
            var db = new Mock<IHistoryContext>();
            manager = new BindManager(db.Object);
        }

        [Test]
        public void CanBindInteger()
        {
            check(5);
        }
        [Test]
        public void CanBindFloat()
        {
            check(5.6f);
        }
        [Test]
        public void CanBindDouble()
        {
            check(5.6);
        }
        [Test]
        public void CanBindDecimal()
        {
            check(5.6M);
        }
        [Test]
        public void CanBindString()
        {
            check("Foo");
        }
        [Test]
        public void CanBindBoolean()
        {
            check(true);
        }
        [Test]
        public void CanBindDateTime()
        {
            check(new DateTime(2014, 03, 26, 12, 50, 36));
        }
        [Test]
        public void CanBindGuid()
        {
            check(new Guid());
        }
        [Test]
        public void CanBindCollection()
        {
            check(new List<int>() { 1, 2, 3 }, serialized: "1, 2,3");
        }        
        [Test]
        public void CanBindCollectionInterface()
        {
            check((ICollection<int>)new List<int>() { 1, 2, 3 }, serialized: "1, 2,3");
        }

        private void check<T>(T value, string serialized = null)
        {
            serialized = serialized ?? value.ToString();
            Assert.AreEqual(value, manager.Bind<T>(serialized));
        }
    }
}
