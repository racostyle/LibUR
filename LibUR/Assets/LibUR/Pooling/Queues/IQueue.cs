using System;
using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    /// <summary>
    /// Interface for queue implementations used by object pools to manage object selection order.
    /// Provides methods for adding indices, building the queue, dequeuing, and managing queue state.
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// Adds an index to the queue's collection. The index will be processed when BuildQueue is called.
        /// </summary>
        /// <param name="i">The index to add to the queue</param>
        void AddToQueue(int i);

        /// <summary>
        /// Builds the internal queue structure from all indices added via AddToQueue.
        /// Must be called after adding all indices and before dequeuing.
        /// </summary>
        void BuildQueue();

        /// <summary>
        /// Removes and returns the next index from the queue.
        /// The behavior (ordered or randomized) depends on the implementation.
        /// </summary>
        /// <returns>The next index in the queue</returns>
        int Dequeue();

        /// <summary>
        /// Removes all indices from the queue, resetting it to an empty state.
        /// </summary>
        void Clear();

        /// <summary>
        /// Gets the number of indices currently in the queue.
        /// </summary>
        int Count { get; }
    }
}
