using LibUR.Events;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class EventBusTests
    {
        private struct STestEvent : IEvent
        {
            public int Value;
        }

        [Test]
        public void Fire_NoSubscribers_DoesNotThrow()
        {
            var bus = EventBus.Build();
            Assert.DoesNotThrow(() => bus.Fire(new STestEvent { Value = 1 }));
        }

        [Test]
        public void Subscribe_AndFire_InvokesHandlerWithPayload()
        {
            var bus = EventBus.Build();
            int received = -1;
            bus.Subscribe<STestEvent>(e => received = e.Value);

            bus.Fire(new STestEvent { Value = 7 });

            Assert.That(received, Is.EqualTo(7));
        }

        [Test]
        public void Unsubscribe_RemovesHandler()
        {
            var bus = EventBus.Build();
            int calls = 0;
            EventBusDelegate<STestEvent> handler = _ => calls++;
            bus.Subscribe(handler);
            bus.Unsubscribe(handler);

            bus.Fire(new STestEvent { Value = 1 });

            Assert.That(calls, Is.EqualTo(0));
        }

        [Test]
        public void Fire_DifferentEventType_DoesNotInvoke()
        {
            var bus = EventBus.Build();
            int calls = 0;
            bus.Subscribe<STestEvent>(_ => calls++);

            bus.Fire(new SOtherEvent());

            Assert.That(calls, Is.EqualTo(0));
        }

        private struct SOtherEvent : IEvent { }
    }
}
