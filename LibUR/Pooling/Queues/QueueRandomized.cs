using System;
using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    public class QueueRandomized : IQueue
    {
        private readonly Queue<int> _queue;
        private readonly List<int> _indexes;
        private readonly Random _random;

        public QueueRandomized(int? seed = null) {
            _random = seed.HasValue ? new Random(seed.Value) : new Random(Guid.NewGuid().GetHashCode());
            _queue = new Queue<int>();
            _indexes = new List<int>();
        }

        public void AddToQueue(int i)
        {
            _indexes.Add(i);
        }

        public void RebuildQueue()
        {
            _queue.Clear();
            for (int i = _indexes.Count - 1; i > 0; i--)
            {
                int swapIndex = _random.Next(i + 1);
                (_indexes[i], _indexes[swapIndex]) = (_indexes[swapIndex], _indexes[i]);
            }

            for (int i = 0; i < _indexes.Count; i++)
                _queue.Enqueue(_indexes[i]);

            _indexes.Clear();
        }

        public int Dequeue()
        {
            return _queue.Dequeue();
        }

        public void Clear()
        {
            _queue.Clear();
        }

        public int Count => _queue.Count;
    }
}
