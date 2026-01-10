using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    /// <summary>
    /// Queue implementation that maintains objects in the order they were added.
    /// Uses FIFO (First-In-First-Out) ordering, ensuring objects are activated in the same order they were added to the queue.
    /// Best for scenarios where you want predictable, sequential object activation.
    /// </summary>
    public class QueueOrdered : IQueue
    {
        private readonly Queue<int> _queue;
        private readonly List<int> _indexes;

        /// <summary>
        /// Creates a new ordered queue instance.
        /// </summary>
        public QueueOrdered()
        {
            _queue = new Queue<int>();
            _indexes = new List<int>();
        }

        /// <summary>
        /// Adds an index to the queue's collection. The index will be processed when BuildQueue is called.
        /// </summary>
        /// <param name="i">The index to add to the queue</param>
        public void AddToQueue(int i)
        {
            _indexes.Add(i);
        }

        /// <summary>
        /// Builds the internal queue structure from all indices added via AddToQueue.
        /// Indices are enqueued in the order they were added, maintaining FIFO ordering.
        /// Must be called after adding all indices and before dequeuing.
        /// </summary>
        public void BuildQueue()
        {
            _queue.Clear();
            for (int i = 0; i < _indexes.Count; i++)
                _queue.Enqueue(_indexes[i]);

            _indexes.Clear();
        }

        /// <summary>
        /// Removes and returns the next index from the queue.
        /// Returns indices in the order they were added (FIFO).
        /// </summary>
        /// <returns>The next index in the queue (oldest added first)</returns>
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
