using System;
using System.Collections.Generic;

namespace LibUR.GameStates
{
    public interface IGameStateObserver
    {
        Dictionary<GameState, List<SubscriberData>> Subscribers { get; }
        void Subscribe(GameState state, Action action, string subscriberName);
        void Unsubscribe(GameState state, string subscriberName);
        void Unsubscribe(string subscriberName);
        void Fire(GameState state);
        void Clear();
    }
}
