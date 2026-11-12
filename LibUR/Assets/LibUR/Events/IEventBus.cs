namespace LibUR.Events
{
    public delegate void EventBusDelegate<T>(T gameEvent) where T : struct, IEvent;

    public interface IEventBus
    {
        void Fire<T>(in T eventData) where T : struct, IEvent;
        void Subscribe<T>(EventBusDelegate<T> eventDelegate) where T : struct, IEvent;
        void Unsubscribe<T>(EventBusDelegate<T> eventDelegate) where T : struct, IEvent;
    }
}
