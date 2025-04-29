using System;

namespace LibUR.GameStates
{
    public interface IGameStateObserver
    {
        void Subscribe(GameState state, Action action);
        void Unsubscribe(GameState state);
        void Fire(GameState state);
    }
}
