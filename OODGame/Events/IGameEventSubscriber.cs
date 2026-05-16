namespace OODGame.Events
{
    public interface IGameEventSubscriber
    {
        void OnEvent(IGameEvent gameEvent);
    }
}
