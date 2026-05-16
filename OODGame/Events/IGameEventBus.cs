namespace OODGame.Events
{
    public interface IGameEventBus
    {
        void Subscribe(IGameEventSubscriber subscriber);
        void Unsubscribe(IGameEventSubscriber subscriber);
        void Publish(IGameEvent gameEvent);
    }
}
