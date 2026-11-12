using System;
using System.Collections.Generic;

namespace LibUR.Signals
{
    public interface ISignalBus<T> where T : Enum
    {
        Dictionary<T, List<SubscriberData>> Subscribers { get; }
        void Subscribe(T state, Action action, string subscriberName);
        void Unsubscribe(T state, string subscriberName);
        void Unsubscribe(string subscriberName);
        void Fire(T state);
        void Clear();
    }
}
