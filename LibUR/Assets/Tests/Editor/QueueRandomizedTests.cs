using System.Linq;
using LibUR.Pooling.Queues;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class QueueRandomizedTests
    {
        [Test]
        public void Dequeue_AfterBuild_ReturnsAllIndicesExactlyOnce()
        {
            var q = new QueueRandomized(seed: 12345);
            q.AddToQueue(0);
            q.AddToQueue(1);
            q.AddToQueue(2);
            q.BuildQueue();
            var results = new[] { q.Dequeue(), q.Dequeue(), q.Dequeue() };
            Assert.That(results.OrderBy(x => x).ToArray(), Is.EqualTo(new[] { 0, 1, 2 }));
        }

        [Test]
        public void SameSeed_ProducesSameOrder()
        {
            var q1 = new QueueRandomized(seed: 42);
            var q2 = new QueueRandomized(seed: 42);
            for (int i = 0; i < 5; i++) { q1.AddToQueue(i); q2.AddToQueue(i); }
            q1.BuildQueue();
            q2.BuildQueue();
            for (int i = 0; i < 5; i++)
                Assert.That(q1.Dequeue(), Is.EqualTo(q2.Dequeue()));
        }

        [Test]
        public void Count_AfterBuild_EqualsAddedCount()
        {
            var q = new QueueRandomized(seed: 0);
            q.AddToQueue(0);
            q.AddToQueue(1);
            q.BuildQueue();
            Assert.That(q.Count, Is.EqualTo(2));
        }
    }
}
