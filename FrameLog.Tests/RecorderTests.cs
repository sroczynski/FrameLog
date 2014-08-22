using System;
using System.Linq;
using FrameLog.Example.Models;
using FrameLog.Logging;
using NUnit.Framework;

namespace FrameLog.Tests
{
    public class RecorderTests
    {
        private Recorder<ChangeSet, User> recorder;
        private User author;
        private DateTime now;

        [SetUp]
        public void CreateRecorder()
        {
            recorder = new Recorder<ChangeSet, User>(new ChangeSetFactory());
            author = new User() { Name = "The author" };
            now = DateTime.Now;
        }

        [Test]
        public void RecordAddsToObjectChanges()
        {
            var a = new TestClass() { Id = 1 };

            recorder.Record(a, "1", "Property", () => 2);
            var set = recorder.Bake(now, author);
            Assert.AreEqual(1, set.ObjectChanges.Count());

            var change = set.ObjectChanges.Single();
            Assert.AreEqual(set, change.ChangeSet);
            Assert.AreEqual(a.Id, int.Parse(change.ObjectReference));

            var propertyChange = change.PropertyChanges.Single();
            Assert.AreEqual(change, propertyChange.ObjectChange);
            Assert.AreEqual("2", propertyChange.Value);
            Assert.AreEqual(2, propertyChange.ValueAsInt);
        }        

        [Test]
        public void MultipleRecordsWithDifferentObjectsResultInMultipleObjectChanges()
        {
            var a = new TestClass() { Id = 1 };
            var b = new TestClass() { Id = 2 };

            recorder.Record(a, "1", "Property", () => 2);
            recorder.Record(b, "2", "Property", () => 2);
            var set = recorder.Bake(now, author);
            Assert.AreEqual(2, set.ObjectChanges.Count());
        }

        [Test]
        public void MultipleRecordsWithSameObjectResultInSingleObjectChangeWithMultiplePropertyChanges()
        {
            var a = new TestClass() { Id = 1 };

            recorder.Record(a, "1", "Property", () => 2);
            recorder.Record(a, "1", "Name", () => "y");
            var set = recorder.Bake(now, author);
            Assert.AreEqual(1, set.ObjectChanges.Count());

            var change = set.ObjectChanges.Single();
            Assert.AreEqual(2, change.PropertyChanges.Count());
        }

        [Test]
        public void BakeSetsAuthor()
        {
            var a = new TestClass() { Id = 1 };
            recorder.Record(a, "1", "Property", () => 1);
            var set = recorder.Bake(now, author);
            Assert.AreEqual(author, set.Author);
        }
        [Test]
        public void BakeSetsTimestamp()
        {
            var a = new TestClass() { Id = 1 };
            recorder.Record(a, "1", "Property", () => 1);
            var set = recorder.Bake(now, author);
            Assert.AreEqual(now, set.Timestamp);
        }

        private class TestClass
        {
            public int Property { get; set; }
            public string Name { get; set; }
            public int Id { get; set; }
        }
    }
}
