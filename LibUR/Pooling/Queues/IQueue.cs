using System;
using System.Collections.Generic;

namespace LibUR.Pooling.Queues
{
    public interface IQueue
    {
        public void AddToQueue(int i);
        public void RebuildQueue();
        public int Dequeue();
        public int Count { get; }
    }
}
