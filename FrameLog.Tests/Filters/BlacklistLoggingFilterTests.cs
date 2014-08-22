using FrameLog.Example.Models;
using FrameLog.Example.Models.Testing;
using FrameLog.Filter;
using NUnit.Framework;
using System.Linq;

namespace FrameLog.Tests
{
    public class BlacklistLoggingFilterTests : DatabaseBackedTest
    {
        protected override Example.ExampleContext makeDatabase()
        {
            return new Example.ExampleContext(new BlacklistLoggingFilter.Provider());
        }

        [Test]
        public void EverythingIsLoggedByDefault()
        {
            ObjectChange objectChange;

            var obj = db.VanillaTestClasses.Add(new VanillaTestClass());
            db.Save(user);

            obj.ScalarProperty = 5;
            db.Save(user);
            objectChange = lastChangeSet().ObjectChanges.Single();
            Assert.AreEqual(typeof(VanillaTestClass).Name, objectChange.TypeName);
            Assert.AreEqual(obj.Id.ToString(), objectChange.ObjectReference);
            Assert.AreEqual("ScalarProperty", objectChange.PropertyChanges.Single().PropertyName);

            obj.NavigationProperty = user;
            db.Save(user);
            objectChange = lastChangeSet().ObjectChanges.Single();
            Assert.AreEqual(typeof(VanillaTestClass).Name, objectChange.TypeName);
            Assert.AreEqual(obj.Id.ToString(), objectChange.ObjectReference);
            Assert.AreEqual("NavigationProperty", objectChange.PropertyChanges.Single().PropertyName);
        }

        [Test]
        public void DoNotLogPreventsPropertyFromBeingLogged()
        {
            var a = makeObject();
            db.Save(user);
            var creation = lastChangeSet();

            a.ExcludedScalarProperty = "George";
            db.Save(user);
            var set = lastChangeSet();

            Assert.AreEqual(creation, set, "A change set was created even though the only change was to a field that should not be logged");            
        }

        [Test]
        public void DoNotLogPreventsNavigationPropertyFromBeingLogged()
        {
            var a = makeObject();
            db.Save(user);
            var creation = lastChangeSet();

            a.ExcludedNavigationProperty = user;
            db.Save(user);
            var set = lastChangeSet();

            Assert.AreEqual(creation, set, "A change set was created even though the only change was to a field that should not be logged");
        }

        [Test]
        public void DoNotLogInMetadataPreventsPropertyFromBeingLogged()
        {
            var a = makeObject();
            db.Save(user);
            var creation = lastChangeSet();

            a.PropertyExcludedByMetadata = 5;
            db.Save(user);
            var set = lastChangeSet();

            Assert.AreEqual(creation, set, "A change set was created even though the only change was to a field that should not be logged");
        }

        [Test]
        public void DoNotLogPreventsPrivatePropertyFromBeingLogged()
        {
            var a = makeObject();
            db.Save(user);
            var creation = lastChangeSet();

            a.SetPrivateExcludedProperty(5);
            db.Save(user);
            var set = lastChangeSet();

            Assert.AreEqual(creation, set, "A change set was created even though the only change was to a field that should not be logged");
        }

        [Test]
        public void DoNotLogPreventsClassFromBeingLogged()
        {
            var a = db.ClassesWithDoNotLog.Add(new ClassWithDoNotLog());
            a.Property = "Foo";
            db.Save(user);

            foreach (var objectChange in lastChangeSet().ObjectChanges)
                Assert.AreNotEqual(typeof(ClassWithDoNotLog).Name, objectChange.TypeName);
        }

        private ClassWithSomeExcludedProperties makeObject()
        {
            return db.ClassesWithSomeExcludedProperties.Add(new ClassWithSomeExcludedProperties());
        }
    }
}
