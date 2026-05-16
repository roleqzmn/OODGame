using System;
using OODGame.Events;

namespace OODGame.Entities
{
    public interface IEntity
    {
        string Name { get; }
    }

    public abstract class Enemy : IEntity, IGameEventSubscriber
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; protected set; }
        public string Species { get; protected set; }
        public int Armor { get; protected set; }
        public int Damage { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int Health { get; set; }

        public bool IsAlive => Health > 0;

        public void OnEvent(IGameEvent gameEvent)
        {
            if (!IsAlive)
                return;

            if (gameEvent is EnemyDeathEvent deathEvent)
            {
                if (deathEvent.Species == Species && deathEvent.EnemyId != Id)
                    OnAllyDeath();
                return;
            }

            if (gameEvent is NoiseBroadcastEvent noiseEvent)
                OnNoiseBroadcast(noiseEvent);
        }

        protected virtual void OnAllyDeath() { }
        protected virtual void OnNoiseBroadcast(NoiseBroadcastEvent noiseEvent) { }
    }
}

