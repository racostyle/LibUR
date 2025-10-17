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
        
        public bool TryDequeue(out int index)
        {
            if (_queue.Count == 0) { index = default; return false; }
            index = _queue.Dequeue();
            return true;
        }

        public void Clear()
        {
            _indexes.Clear();
            _queue.Clear();
        }

        public int Count => _queue.Count;
    }
}
