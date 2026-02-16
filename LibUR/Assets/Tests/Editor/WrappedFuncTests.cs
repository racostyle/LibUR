using System;
using LibUR.Delegates;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class WrappedFuncTests
    {
        [Test]
        public void Invoke_WithoutRegister_ThrowsInvalidOperationException()
        {
            var w = new WrappedFunc<int>();
            var ex = Assert.Throws<InvalidOperationException>(() => w.Invoke());
            Assert.That(ex.Message, Does.Contain("No delegate registered"));
        }

        [Test]
        public void Register_AndInvoke_ReturnsValue()
        {
            var w = new WrappedFunc<int>();
            w.Register(() => 42);
            Assert.That(w.Invoke(), Is.EqualTo(42));
        }

        [Test]
        public void Register_OnlyFirstRegistrationIsUsed()
        {
            var w = new WrappedFunc<int>();
            w.Register(() => 1);
            w.Register(() => 2);
            Assert.That(w.Invoke(), Is.EqualTo(1));
        }

        [Test]
        public void WrappedFunc_T1_InvokeWithoutRegister_Throws()
        {
            var w = new WrappedFunc<int, string>();
            Assert.Throws<InvalidOperationException>(() => w.Invoke(0));
        }

        [Test]
        public void WrappedFunc_T1_RegisterAndInvoke_ReturnsValue()
        {
            var w = new WrappedFunc<int, string>();
            w.Register(i => i.ToString());
            Assert.That(w.Invoke(42), Is.EqualTo("42"));
        }

        [Test]
        public void WrappedFunc_T1_T2_InvokeWithoutRegister_Throws()
        {
            var w = new WrappedFunc<int, int, int>();
            Assert.Throws<InvalidOperationException>(() => w.Invoke(1, 2));
        }

        [Test]
        public void WrappedFunc_T1_T2_RegisterAndInvoke_ReturnsValue()
        {
            var w = new WrappedFunc<int, int, int>();
            w.Register((a, b) => a + b);
            Assert.That(w.Invoke(3, 4), Is.EqualTo(7));
        }
    }
}
