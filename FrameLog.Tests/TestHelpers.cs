using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace FrameLog.Tests.Helpers
{
    public static class TestHelpers
    {
        public static void Contains(string expected, string actual, string message = null)
        {
            Assert.True(actual.Contains(expected),
                message ?? string.Format("Expected string to contain '{0}'. Actual value: '{1}'.",
                expected, actual));
        }

        public static void Contains<T>(T expected, IEnumerable<T> enumerable, string message = null)
        {
            Assert.True(enumerable.Contains(expected), message ?? string.Format("Expected enumerable to contain '{0}'. Actual contents: '{1}'.",
                expected, string.Join(", ", enumerable)));
        }

        public static void DoesNotContain(string expected, string actual, string message = null)
        {
            Assert.False(actual.Contains(expected),
                message ?? string.Format("Did not expect string to contain '{0}'. Actual value: '{1}'.",
                expected, actual));
        }

        public static void DoesNotContain<T>(T expected, IEnumerable<T> enumerable, string message = null)
        {
            Assert.False(enumerable.Contains(expected), message ?? string.Format("Expected enumerable not to contain '{0}'. Actual contents: '{1}'.",
                expected, string.Join(", ", enumerable)));
        }

        public static void ContainsAll<T>(IEnumerable<T> enumerable, params T[] expected)
        {
            var notFound = expected.Where(ex => !enumerable.Contains(ex));
            if (notFound.Any())
                Assert.Fail(string.Format("The following items were expected to be contained by the enumerable: '{0}'. Actual contents '{1}'.",
                    string.Join(", ", notFound), string.Join(", ", enumerable)));
        }

        public static void IsRecent(DateTime timestamp, TimeSpan span, string message = null, DateTime? now = null)
        {
            now = now ?? DateTime.Now;
            var actualSpan = now - timestamp;
            if (actualSpan > span)
                Assert.Fail(message ?? string.Format("Timestamp {0} was not recent. It was not within {1}, it was {2}", timestamp, span, actualSpan));
        }

        public static void EnumerablesAreEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null)
        {
            if (!enumerablesAreEqual(expected, actual))
            {
                Assert.Fail(message ?? string.Format("Expected enumerables to be equal.\nMissing elements: {0}\nUnexpected elements: {1}", 
                    join(expected.Except(actual)), join(actual.Except(expected))));
            }
        }
        public static void EnumerablesAreOrderedEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual, string message = null)
        {
            if (!AreEnumerablesOrderedEqual(expected, actual))
                Assert.Fail(message ?? string.Format("Expected enumerables to be equal (respecting ordering).\nExpected: {0}\nActual: {1}", join(expected), join(actual)));
        
        }
        public static bool AreEnumerablesOrderedEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (expected.Count() != actual.Count())
                return false;

            return expected.Zip(actual, (a, b) => object.Equals(a, b)).All(b => b);
        }

        private static string join<T>(IEnumerable<T> enumerable)
        {
            return string.Join(", ", enumerable);
        }

        public static void IsEmpty<T>(IEnumerable<T> enumerable, string message = null)
        {
            Assert.That(!enumerable.Any(), message ?? "Expected enumerable to be empty");
        }

        private static bool enumerablesAreEqual<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            foreach (T item in a)
                if (!b.Contains(item))
                    return false;
            foreach (T item in b)
                if (!a.Contains(item))
                    return false;
            return true;
        }

        public static void ContainsOfType<TNeedle>(IEnumerable<object> enumerable)
        {
            foreach (object item in enumerable)
            {
                if (item is TNeedle)
                    return;
            }
            Assert.Fail(string.Format("No item of type '{0}' found in this collection: {1}",
                typeof(TNeedle), join(enumerable)));
        }
    }
}
