using System;

namespace LibUR.GameStates
{
    public interface IGameStateObserver
    {
        void Subscribe(GameState state, Action action, string subscriberName);
        void Unsubscribe(GameState state, string subscriberName);
        void Fire(GameState state);
        void Clear();
    }
}
