using LibUR.Auxiliary;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class FrameThrottleTests
    {
        [Test]
        public void CanUpdate_WithLimit2_ReturnsTrueEverySecondCall()
        {
            var t = new FrameThrottle(2, randomStartIndex: false);
            Assert.That(t.CanUpdate(), Is.False); // 1
            Assert.That(t.CanUpdate(), Is.True);  // 2
            Assert.That(t.CanUpdate(), Is.False); // 3
            Assert.That(t.CanUpdate(), Is.True);  // 4
        }

        [Test]
        public void CanUpdate_WithLimit3_ReturnsTrueEveryThirdCall()
        {
            var t = new FrameThrottle(3, randomStartIndex: false);
            Assert.That(t.CanUpdate(), Is.False); // 1
            Assert.That(t.CanUpdate(), Is.False); // 2
            Assert.That(t.CanUpdate(), Is.True);  // 3
            Assert.That(t.CanUpdate(), Is.False); // 4
            Assert.That(t.CanUpdate(), Is.False); // 5
            Assert.That(t.CanUpdate(), Is.True);  // 6
        }

        [Test]
        public void CanUpdate_WithLimit0_ReturnsTrueEveryCall()
        {
            var t = new FrameThrottle(0, randomStartIndex: false);
            Assert.That(t.CanUpdate(), Is.True);
            Assert.That(t.CanUpdate(), Is.True);
            Assert.That(t.CanUpdate(), Is.True);
        }

        [Test]
        public void CanUpdate_WithLimit1_ReturnsTrueEveryCall()
        {
            var t = new FrameThrottle(1, randomStartIndex: false);
            Assert.That(t.CanUpdate(), Is.True);
            Assert.That(t.CanUpdate(), Is.True);
            Assert.That(t.CanUpdate(), Is.True);
        }

        [Test]
        public void CanUpdate_WithLimit5_EventuallyReturnsTrue()
        {
            var t = new FrameThrottle(5, randomStartIndex: false);
            for (int i = 0; i < 5; i++)
                if (t.CanUpdate()) return;
            Assert.Fail("CanUpdate did not return true within 5 calls.");
        }
    }
}
