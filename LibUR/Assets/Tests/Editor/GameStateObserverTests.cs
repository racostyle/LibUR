using System;
using LibUR.GameStates;
using NUnit.Framework;

namespace LibUR.Tests
{
    public enum TestState { Idle, Running, Paused }

    public class GameStateObserverTests
    {
        [Test]
        public void Fire_NoSubscribers_DoesNotThrow()
        {
            var obs = new GameStateObserver<TestState>();
            Assert.DoesNotThrow(() => obs.Fire(TestState.Running));
        }

        [Test]
        public void Subscribe_AndFire_InvokesAction()
        {
            var obs = new GameStateObserver<TestState>();
            int called = 0;
            obs.Subscribe(TestState.Running, () => called++, "Test");
            obs.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(1));
        }

        [Test]
        public void Fire_DifferentState_DoesNotInvoke()
        {
            var obs = new GameStateObserver<TestState>();
            int called = 0;
            obs.Subscribe(TestState.Running, () => called++, "Test");
            obs.Fire(TestState.Idle);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Subscribe_MultipleSubscribers_AllInvoked()
        {
            var obs = new GameStateObserver<TestState>();
            int a = 0, b = 0;
            obs.Subscribe(TestState.Running, () => a++, "A");
            obs.Subscribe(TestState.Running, () => b++, "B");
            obs.Fire(TestState.Running);
            Assert.That(a, Is.EqualTo(1));
            Assert.That(b, Is.EqualTo(1));
        }

        [Test]
        public void Unsubscribe_ByName_RemovesSubscriber()
        {
            var obs = new GameStateObserver<TestState>();
            int called = 0;
            obs.Subscribe(TestState.Running, () => called++, "Sub");
            obs.Unsubscribe(TestState.Running, "Sub");
            obs.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Unsubscribe_ByName_IsCaseInsensitive()
        {
            var obs = new GameStateObserver<TestState>();
            int called = 0;
            obs.Subscribe(TestState.Running, () => called++, "Sub");
            obs.Unsubscribe(TestState.Running, "sub");
            obs.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Unsubscribe_BySubscriberName_RemovesFromAllStates()
        {
            var obs = new GameStateObserver<TestState>();
            int called = 0;
            obs.Subscribe(TestState.Idle, () => called++, "Sub");
            obs.Subscribe(TestState.Running, () => called++, "Sub");
            obs.Unsubscribe("Sub");
            obs.Fire(TestState.Idle);
            obs.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }

        [Test]
        public void Clear_RemovesAllSubscribers()
        {
            var obs = new GameStateObserver<TestState>();
            int called = 0;
            obs.Subscribe(TestState.Running, () => called++, "Sub");
            obs.Clear();
            obs.Fire(TestState.Running);
            Assert.That(called, Is.EqualTo(0));
        }
    }
}
