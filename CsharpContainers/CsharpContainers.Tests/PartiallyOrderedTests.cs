using System;
using System.Collections.Generic;
using Containers;
using NUnit.Framework;

namespace CsharpContainers.Tests
{
    [TestFixture]
    public class PartiallyOrderedTests
    {
        [Test]
        public void partial_ordering_demonstration ()
        {
            var list = new List<OrderableThing>{ new OrderableThing(), new OrderableThing(), new OrderableThing() };
            list.Sort();

            Assert.That(list[0] < list[1]);
            Assert.That(list[2] > list[1]);
            Assert.That(list[0] != list[1]);
        }

        internal class OrderableThing : PartiallyOrdered
        {
            public readonly int RandomValue;

            public OrderableThing()
            {
                var g = Guid.NewGuid().ToByteArray();
                RandomValue = g[0] + (g[1] << 8) + (g[2] << 16) + (g[3] << 24);
            }

            public override int CompareTo(object obj)
            {
                if ( ! (obj is OrderableThing)) return 0;
                return RandomValue.CompareTo(((OrderableThing)obj).RandomValue);
            }

            public override int GetHashCode()
            {
                return RandomValue;
            }
        }
    }
}
