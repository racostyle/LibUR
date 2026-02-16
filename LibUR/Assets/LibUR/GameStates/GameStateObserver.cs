using System;
using System.Collections.Generic;
using System.Linq;

namespace LibUR.GameStates
{
    public class GameStateObserver<T> : IGameStateObserver<T> where T : Enum
    {
        private readonly Dictionary<T, List<SubscriberData>> _subscribers;
        public Dictionary<T, List<SubscriberData>> Subscribers => _subscribers;

        public GameStateObserver()
        {
            _subscribers = new Dictionary<T, List<SubscriberData>>();
        }

        public void Subscribe(T state, Action action, string subscriberName)
        {
            if (_subscribers.ContainsKey(state))
            {
                if (_subscribers[state].Any(x => x.Name == subscriberName))
                {
                    UnityEngine.Debug.LogWarning($"{subscriberName} already subscribed to {state}");
                    return;
                }

                _subscribers[state].Add(new SubscriberData(subscriberName, action));
                return;
            }

            
            else
                _subscribers.Add(state, new List<SubscriberData> { new SubscriberData(subscriberName, action) });
        }

        public void Unsubscribe(T state, string subscriberName)
        {
            if (!_subscribers.ContainsKey(state))
                return;

            _subscribers[state].RemoveAll(x => x.Name.Equals(subscriberName, StringComparison.OrdinalIgnoreCase));
        }

        public void Unsubscribe(string subscriberName)
        {
            foreach (var state in _subscribers)
                state.Value.RemoveAll(x => x.Name.Equals(subscriberName, StringComparison.OrdinalIgnoreCase));
        }

        public void Fire(T state)
        {
            if (!_subscribers.TryGetValue(state, out var list))
                return;

            foreach (var subscriber in list)
                subscriber.Action?.Invoke();
        }

        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}
