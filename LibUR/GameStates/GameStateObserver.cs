using System;
using System.Collections.Generic;
using System.Linq;

namespace LibUR.GameStates
{
    public class GameStateObserver : IGameStateObserver
    {
        private readonly Dictionary<GameState, List<Subscriber>> _states;

        public GameStateObserver()
        {
            _states = new Dictionary<GameState, List<Subscriber>>();
        }

        public void Subscribe(GameState state, Action action, string subscriberName)
        {
            if (_states.ContainsKey(state))
            {
                _states[state].Add(new Subscriber(subscriberName, action));
                return;
            }

            if (_states[state].Any(x => x.Name == subscriberName))
                UnityEngine.Debug.LogWarning($"{subscriberName} already subscribed to {state}");
            else
                _states.Add(state, new List<Subscriber> { new Subscriber(subscriberName, action) });
        }

        public void Unsubscribe(GameState state, string subscriberName)
        {
            if (!_states.ContainsKey(state))
                return;

            _states[state].RemoveAll(x => x.Name == subscriberName);
        }

        public void Fire(GameState state)
        {
            foreach (var st in _states)
            {
                if (!st.Key.Equals(state))
                    continue;

                foreach (var subscriber in st.Value)
                    subscriber.Action?.Invoke();
            }
        }

        public void Clear()
        {
            _states.Clear();
        }
    }

    internal struct Subscriber
    {
        internal string Name;
        internal Action Action;

        public Subscriber(string name, Action action)
        {
            Name = name;
            Action = action;
        }
    }
}
