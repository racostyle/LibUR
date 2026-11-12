using LibUR.Signals;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class SignalBusTests
    {
        [Test]
        public void Fire_NoSubscribers_DoesNotThrow()
        {
            var bus = SignalBus<TestState>.Build();
            Assert.DoesNotThrow(() => bus.Fire(TestState.Running));
        }

        [Test]
        public void Subscribe_AndFire_InvokesAction()
        {
            var bus = SignalBus<TestState>.Build();
            int called = 0;
            bus.Subscribe(TestState.Running, () => called++, "Test");
            bus.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(1));
        }

        [Test]
        public void Fire_DifferentState_DoesNotInvoke()
        {
            var bus = SignalBus<TestState>.Build();
            int called = 0;
            bus.Subscribe(TestState.Running, () => called++, "Test");
            bus.Fire(TestState.Idle);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Unsubscribe_ByName_IsCaseInsensitive()
        {
            var bus = SignalBus<TestState>.Build();
            int called = 0;
            bus.Subscribe(TestState.Running, () => called++, "Sub");
            bus.Unsubscribe(TestState.Running, "sub");
            bus.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Unsubscribe_BySubscriberName_RemovesFromAllStates()
        {
            var bus = SignalBus<TestState>.Build();
            int called = 0;
            bus.Subscribe(TestState.Idle, () => called++, "Sub");
            bus.Subscribe(TestState.Running, () => called++, "Sub");
            bus.Unsubscribe("Sub");
            bus.Fire(TestState.Idle);
            bus.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Clear_RemovesAllSubscribers()
        {
            var bus = SignalBus<TestState>.Build();
            int called = 0;
            bus.Subscribe(TestState.Running, () => called++, "Sub");
            bus.Clear();
            bus.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }
    }
}
