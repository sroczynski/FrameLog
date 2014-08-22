using FrameLog.Example.Models;
using FrameLog.Example.Models.Testing;
using FrameLog.Filter;
using NUnit.Framework;
using System.Linq;

namespace FrameLog.Tests
{
    public class WhitelistLoggingFilterTests : DatabaseBackedTest
    {
        protected override Example.ExampleContext makeDatabase()
        {
            return new Example.ExampleContext(new WhitelistLoggingFilter.Provider());
        }

        [Test]
        public void NothingIsLoggedByDefault()
        {
            var obj = db.VanillaTestClasses.Add(new VanillaTestClass());
            db.Save(user);

            obj.ScalarProperty = 5;
            db.Save(user);
            Assert.IsFalse(db.ChangeSets.Any(c => c.ObjectChanges.Any(o => o.TypeName == typeof(VanillaTestClass).Name)));

            obj.NavigationProperty = user;
            db.Save(user);
            Assert.IsFalse(db.ChangeSets.Any(c => c.ObjectChanges.Any(o => o.TypeName == typeof(VanillaTestClass).Name)));
        }

        [Test]
        public void OnlyMarkedScalarPropertiesAreLogged()
        {
            ChangeSet set;

            var a = makeObject();
            db.Save(user);
            var creation = lastChangeSet();

            a.ExcludedScalarProperty = "George";
            db.Save(user);
            set = lastChangeSet();
            Assert.AreEqual(creation, set, "A change set was created even though the only change was to a field that should not be logged");

            a.IncludedScalarProperty = "George";
            db.Save(user);
            set = lastChangeSet();
            Assert.AreNotEqual(creation, set, "A change set was not created even though there was a change to a field that should be logged");
        }

        [Test]
        public void OnlyMarkedNavigationPropertiesAreLogged()
        {
            ChangeSet set;

            var a = makeObject();
            db.Save(user);
            var creation = lastChangeSet();

            a.ExcludedNavigationProperty = user;
            db.Save(user);
            set = lastChangeSet();
            Assert.AreEqual(creation, set, "A change set was created even though the only change was to a field that should not be logged");

            a.IncludedNavigationProperty = user;
            db.Save(user);
            set = lastChangeSet();
            Assert.AreNotEqual(creation, set, "A change set was not created even though there was a change to a field that should be logged");
        }

        private ClassWithSomeIncludedProperties makeObject()
        {
            return db.ClassesWithSomeIncludedProperties.Add(new ClassWithSomeIncludedProperties());
        }
    }
}
