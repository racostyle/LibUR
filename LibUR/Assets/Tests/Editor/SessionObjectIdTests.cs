using System;
using Assets.LibUR.Auxiliary;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class SessionObjectIdTests
    {
        [Test]
        public void Get_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SessionObjectId.Get(null));
        }

        [Test]
        public void Get_SameObject_ReturnsSameId()
        {
            var obj = new object();
            int id1 = SessionObjectId.Get(obj);
            int id2 = SessionObjectId.Get(obj);
            Assert.That(id1, Is.EqualTo(id2));
        }

        [Test]
        public void Get_DifferentObjects_ReturnsDifferentIds()
        {
            var a = new object();
            var b = new object();
            Assert.That(SessionObjectId.Get(a), Is.Not.EqualTo(SessionObjectId.Get(b)));
        }

        [Test]
        public void Get_IdsArePositive()
        {
            var obj = new object();
            Assert.That(SessionObjectId.Get(obj), Is.GreaterThan(0));
        }
    }
}
