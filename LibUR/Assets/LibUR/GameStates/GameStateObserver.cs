using System;
using System.Collections.Generic;
using System.Linq;

namespace LibUR.GameStates
{
    public class GameStateObserver : IGameStateObserver
    {
        private readonly Dictionary<GameState, List<SubscriberData>> _subscribers;
        public Dictionary<GameState, List<SubscriberData>> Subscribers => _subscribers;

        public GameStateObserver()
        {
            _subscribers = new Dictionary<GameState, List<SubscriberData>>();
        }

        public void Subscribe(GameState state, Action action, string subscriberName)
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

        public void Unsubscribe(GameState state, string subscriberName)
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

        public void Fire(GameState state)
        {
            foreach (var st in _subscribers)
            {
                if (!st.Key.Equals(state))
                    continue;

                foreach (var subscriber in st.Value)
                    subscriber.Action?.Invoke();
            }
        }

        public void Clear()
        {
            _subscribers.Clear();
        }
    }
}
