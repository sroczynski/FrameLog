using System.Collections.Generic;
using System.Linq;
using FrameLog.Logging;
using NUnit.Framework;

namespace FrameLog.Tests
{
    public class DeferredValueMapTests
    {
        private DeferredValueMap<int> map;
        
        [SetUp]
        public void CreateBlankMap()
        {
            map = new DeferredValueMap<int>();
        }

        [Test]
        public void CanStoreAndRetrieveSingleValue()
        {
            map.Store(5, "A", () => 2);
            var result = map.CalculateAndRetrieve(5);

            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual("A", result.Keys.Single());
            Assert.AreEqual(2, result["A"]);
        }

        [Test]
        public void CanStoreMultipleValuesForAContainer()
        {
            map.Store(5, "A", () => 1);
            map.Store(5, "B", () => 2);
            var result = map.CalculateAndRetrieve(5);
            Assert.AreEqual(2, result.Keys.Count);
            Assert.AreEqual(1, result["A"]);
            Assert.AreEqual(2, result["B"]);
        }

        [Test]
        public void LaterValuesOverwriteEarlierValues()
        {
            map.Store(5, "A", () => 1);
            map.Store(5, "A", () => 2);
            var result = map.CalculateAndRetrieve(5);
            Assert.AreEqual(1, result.Keys.Count);
            Assert.AreEqual(2, result["A"]);
        }

        [Test]
        public void UnknownKeyThrowsKeyNotFound()
        {
            map.Store(5, "A", () => 2);
            Assert.Throws<KeyNotFoundException>(() => map.CalculateAndRetrieve(1));
        }

        [Test]
        public void DifferentContainersValuesAreStoredSeparately()
        {
            map.Store(1, "A", () => 1);
            map.Store(2, "B", () => 2);
            var one = map.CalculateAndRetrieve(1);
            var two = map.CalculateAndRetrieve(2);
            Assert.IsTrue(one.ContainsKey("A"));
            Assert.IsFalse(two.ContainsKey("A"));
            Assert.IsFalse(one.ContainsKey("B"));
            Assert.IsTrue(two.ContainsKey("B"));
        }

        [Test]
        public void WorkIsDeferred()
        {
            const string errorMessage = "The work was not deferred, it was calculated on storage";
            // Note simultaneous assignment and return in these delegates

            int value = 0;
            map.Store(1, "A", () => value = 1);
            Assert.AreEqual(0, value, errorMessage);
            map.Store(1, "A", () => value = 2);
            Assert.AreEqual(0, value, errorMessage);
            map.Store(2, "A", () => value = 3);
            Assert.AreEqual(0, value, errorMessage);
            var result = map.CalculateAndRetrieve(1);
            
            Assert.AreEqual(2, value, @"If this value is 1, then the original delegate was never overwritten.
If it is 3, then when we invoked calculation for container 1, container 2 was invoked as well/instead");
            Assert.AreEqual(2, result["A"]);
        }

        [Test]
        public void HasContainerIsTrueAfterStoringSomethingAgainstThatContainer()
        {
            Assert.IsFalse(map.HasContainer(1));
            map.Store(1, "A", () => 1);
            Assert.IsTrue(map.HasContainer(1));
        }
    }
}
