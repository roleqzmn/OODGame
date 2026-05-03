using System;
namespace OODGame.Entities
{
    public interface IEntity
    {
        string Name { get; }
    }

    public abstract class Enemy : IEntity
    {
        public string Name { get; protected set; }
        public int Armor { get; protected set; }
        public int Damage { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int Health { get; set; }

        public bool IsAlive => Health > 0;
    }
}

