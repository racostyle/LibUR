using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    public class QueueOrdered : IQueue
    {
        private readonly Queue<int> _queue;
        private readonly List<int> _indexes;

        public QueueOrdered()
        {
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
            for (int i = 0; i < _indexes.Count; i++)
                _queue.Enqueue(_indexes[i]);

            _indexes.Clear();
        }

        public int Dequeue()
        {
            return _queue.Dequeue();
        }

        public int Count => _queue.Count;
    }
}
