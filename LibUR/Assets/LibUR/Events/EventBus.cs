using System;
using System.Collections.Generic;

namespace LibUR.Events
{
    public class EventBus : IEventBus
    {
        private Dictionary<Type, Delegate> _eventRegistry;

        internal EventBus()
        {
            _eventRegistry = new Dictionary<Type, Delegate>();
        }

        public static IEventBus Build() => new EventBus();

        #region [REGISTER METHODS]
        public void Subscribe<T>(EventBusDelegate<T> eventDelegate) where T : struct, IEvent
        {
            var t = typeof(T);
            if (_eventRegistry.TryGetValue(t, out Delegate d))
            {
                _eventRegistry[t] = Delegate.Combine(d, eventDelegate);
            }
            else
            {
                _eventRegistry.Add(t, eventDelegate);
            }
        }

        public void Unsubscribe<T>(EventBusDelegate<T> eventDelegate) where T : struct, IEvent
        {
            var t = typeof(T);
            if (_eventRegistry.TryGetValue(t, out Delegate d))
            {
                Delegate currentDelegate = Delegate.Remove(d, eventDelegate);

                if (currentDelegate == null)
                {
                    _eventRegistry.Remove(t);
                }
                else
                {
                    _eventRegistry[t] = currentDelegate;
                }
            }
        }
        #endregion

        #region [START METHOD]
        public void Fire<T>(in T eventData) where T : struct, IEvent
        {
            if (_eventRegistry.TryGetValue(typeof(T), out Delegate d))
            {
                EventBusDelegate<T> actionDelegate = d as EventBusDelegate<T>;
                actionDelegate?.Invoke(eventData);
            }
        }
        #endregion
    }
}
