using System;
using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    public interface IQueue
    {
        void AddToQueue(int i);
        void RebuildQueue();
        bool TryDequeue(out int index);
        int Dequeue();
        void Clear();
        int Count { get; }
    }
}
