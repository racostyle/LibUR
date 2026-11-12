using System.Linq;
using Assets.LibUR;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class ExtensionsTests
    {
        [Test]
        public void Shuffle_EmptyList_DoesNotThrow()
        {
            var list = new int[0];
            Assert.DoesNotThrow(() => list.Shuffle());
        }

        [Test]
        public void Shuffle_SingleItem_LeavesItemInPlace()
        {
            var list = new[] { 4 };
            list.Shuffle();
            Assert.That(list, Is.EqualTo(new[] { 4 }));
        }

        [Test]
        public void Shuffle_KeepsTheSameElements()
        {
            var list = new[] { 1, 2, 3, 4, 5, 6, 7, 8 };
            var expected = list.ToArray();
            list.Shuffle();
            Assert.That(list.OrderBy(x => x), Is.EqualTo(expected));
        }
    }
}
