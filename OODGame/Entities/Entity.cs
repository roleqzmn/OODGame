using System;
using OODGame.Events;
using OODGame.Logger;
using OODGame.Map;

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
        public int Xpos { get; private set; } = -1;
        public int Ypos { get; private set; } = -1;
        public IMapNavigator? Navigator { get; private set; }

        public bool IsAlive => Health > 0;

        public void SetSpatialContext(int x, int y, IMapNavigator navigator)
        {
            Xpos = x;
            Ypos = y;
            Navigator = navigator;
        }

        public void UpdatePosition(int x, int y)
        {
            Xpos = x;
            Ypos = y;
        }

        public void ClearSpatialContext()
        {
            Xpos = -1;
            Ypos = -1;
            Navigator = null;
        }

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

        protected virtual void OnNoiseBroadcast(NoiseBroadcastEvent noiseEvent)
        {
            if (Navigator == null || Xpos < 0 || Ypos < 0)
                return;

            int? distance = Navigator.GetShortestPathDistance(
                (Xpos, Ypos),
                (noiseEvent.SourceX, noiseEvent.SourceY),
                noiseEvent.Range);

            if (!distance.HasValue)
                return;

            EventLogger.Instance?.LogEvent(
                $"{Name} [{Species}] heard {noiseEvent.Category} noise at ({noiseEvent.SourceX},{noiseEvent.SourceY}) from ({Xpos},{Ypos}), distance: {distance.Value}.");
        }
    }
}

