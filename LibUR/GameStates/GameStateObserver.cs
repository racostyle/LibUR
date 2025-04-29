using System;
using System.Collections.Generic;

namespace LibUR.GameStates
{
    public class GameStateObserver : IGameStateObserver
    {
        private readonly Dictionary<GameState, Action> _states;

        public GameStateObserver()
        {
            _states = new Dictionary<GameState, Action>();
        }

        public void Subscribe(GameState state, Action action)
        {
            _states.Add(state, action);
        }

        public void Unsubscribe(GameState state)
        {
            _states.Remove(state);
        }

        public void Fire(GameState state)
        {
            foreach (var st in _states)
            {
                if (st.Key.Equals(state)) st.Value?.Invoke();
            }
        }
    }
}
