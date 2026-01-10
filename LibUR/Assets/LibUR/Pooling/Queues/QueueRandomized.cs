using System;
using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    /// <summary>
    /// Queue implementation that randomizes the order of objects using Fisher-Yates shuffle algorithm.
    /// Objects are selected in random order, which helps distribute wear evenly across pooled objects.
    /// Best for scenarios where you want unpredictable, randomized object activation.
    /// </summary>
    public class QueueRandomized : IQueue
    {
        private readonly Queue<int> _queue;
        private readonly List<int> _indexes;
        private readonly Random _random;

        /// <summary>
        /// Creates a new randomized queue instance.
        /// </summary>
        /// <param name="seed">Optional seed for random number generation. If null, uses a random seed based on GUID hash.</param>
        public QueueRandomized(int? seed = null) {
            _random = seed.HasValue ? new Random(seed.Value) : new Random(Guid.NewGuid().GetHashCode());
            _queue = new Queue<int>();
            _indexes = new List<int>();
        }

        /// <summary>
        /// Adds an index to the queue's collection. The index will be processed and randomized when BuildQueue is called.
        /// </summary>
        /// <param name="i">The index to add to the queue</param>
        public void AddToQueue(int i)
        {
            _indexes.Add(i);
        }

        /// <summary>
        /// Builds the internal queue structure from all indices added via AddToQueue.
        /// Uses Fisher-Yates shuffle algorithm to randomize the order of indices before enqueuing them.
        /// Must be called after adding all indices and before dequeuing.
        /// </summary>
        public void BuildQueue()
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

        /// <summary>
        /// Removes and returns the next index from the queue.
        /// Returns indices in randomized order (determined during BuildQueue).
        /// </summary>
        /// <returns>The next index in the randomized queue</returns>
        public int Dequeue()
        {
            return _queue.Dequeue();
        }

        /// <summary>
        /// Removes all indices from the queue, resetting it to an empty state.
        /// </summary>
        public void Clear()
        {
            _queue.Clear();
        }

        /// <summary>
        /// Gets the number of indices currently in the queue.
        /// </summary>
        public int Count => _queue.Count;
    }
}
