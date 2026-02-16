using System;
using LibUR.Delegates;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class WrappedActionTests
    {
        [Test]
        public void Invoke_WithoutRegister_DoesNotThrow()
        {
            var w = new WrappedAction();
            Assert.DoesNotThrow(() => w.Invoke());
        }

        [Test]
        public void Register_AndInvoke_InvokesDelegate()
        {
            var w = new WrappedAction();
            int called = 0;
            w.Register(() => called++);
            w.Invoke();
            Assert.That(called, Is.EqualTo(1));
        }

        [Test]
        public void Register_OnlyFirstRegistrationIsUsed()
        {
            var w = new WrappedAction();
            int first = 0, second = 0;
            w.Register(() => first++);
            w.Register(() => second++);
            w.Invoke();
            Assert.That(first, Is.EqualTo(1));
            Assert.That(second, Is.EqualTo(0));
        }

        [Test]
        public void WrappedAction_T1_Invoke_CallsDelegate()
        {
            var w = new WrappedAction<int>();
            int received = -1;
            w.Register(x => received = x);
            w.Invoke(99);
            Assert.That(received, Is.EqualTo(99));
        }
    }
}
