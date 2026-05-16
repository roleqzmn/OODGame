using System.Collections.Generic;

namespace OODGame.Events
{
    public sealed class GameEventBus : IGameEventBus
    {
        private readonly HashSet<IGameEventSubscriber> _subscribers = new HashSet<IGameEventSubscriber>();

        public void Subscribe(IGameEventSubscriber subscriber)
        {
            _subscribers.Add(subscriber);
        }

        public void Unsubscribe(IGameEventSubscriber subscriber)
        {
            _subscribers.Remove(subscriber);
        }

        public void Publish(IGameEvent gameEvent)
        {
            var snapshot = new List<IGameEventSubscriber>(_subscribers);
            foreach (var subscriber in snapshot)
            {
                subscriber.OnEvent(gameEvent);
            }
        }
    }
}
