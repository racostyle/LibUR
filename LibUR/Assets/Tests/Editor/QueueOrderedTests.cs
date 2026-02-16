using LibUR.Pooling.Queues;
using NUnit.Framework;

namespace LibUR.Tests
{
    public class QueueOrderedTests
    {
        [Test]
        public void Dequeue_AfterBuild_ReturnsInOrder()
        {
            var q = new QueueOrdered();
            q.AddToQueue(10);
            q.AddToQueue(20);
            q.AddToQueue(30);
            q.BuildQueue();
            Assert.That(q.Dequeue(), Is.EqualTo(10));
            Assert.That(q.Dequeue(), Is.EqualTo(20));
            Assert.That(q.Dequeue(), Is.EqualTo(30));
        }

        [Test]
        public void Count_AfterBuild_EqualsAddedCount()
        {
            var q = new QueueOrdered();
            q.AddToQueue(0);
            q.AddToQueue(1);
            q.BuildQueue();
            Assert.That(q.Count, Is.EqualTo(2));
        }

        [Test]
        public void Clear_EmptiesQueue()
        {
            var q = new QueueOrdered();
            q.AddToQueue(0);
            q.BuildQueue();
            q.Clear();
            Assert.That(q.Count, Is.EqualTo(0));
        }
    }
}
