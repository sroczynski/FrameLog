using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrameLog.Example.Models;
using NUnit.Framework;

namespace FrameLog.Tests
{
    public class DynamicProxiesTests : DatabaseBackedTest
    {
        private ModelWithDynamicProxy m;

        protected override void setupWithDatabase(Example.ExampleContext db)
        {
            base.setupWithDatabase(db);
            m = db.ModelsWithDynamicProxies.Create();
            db.ModelsWithDynamicProxies.Add(m);
            db.Save(user);
        }

        [Test]
        public void ModelHasDynamicProxy()
        {
            Assert.AreNotEqual(typeof(ModelWithDynamicProxy), m.GetType());
        }

        /// <summary>
        /// Here we are wanting to check that the type that gets logged is the
        /// poco type - not the generated type of the dynamic proxy.
        /// </summary>
        [Test]
        public void CanLogWithCorrectType()
        {
            m.Field = 5;
            db.Save(user);
            var c = lastChangeSet();
            string loggedType = c.ObjectChanges.Single().TypeName;

            Assert.AreEqual(typeof(ModelWithDynamicProxy).Name, loggedType);
        }
    }
}
