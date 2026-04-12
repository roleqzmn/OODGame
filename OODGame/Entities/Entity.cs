using System;
namespace OODGame.Entities
{
    public abstract class Entity
    {
        public string Name { get; protected set; }
        public Entity() { Name = string.Empty; }
    }

    public abstract class Enemy : Entity
    {
        public int Armor { get; protected set; }
        public int Damage { get; protected set; }
        public int MaxHealth { get; protected set; }
        public int Health { get; set; }

        public bool IsAlive => Health > 0;
    }
}

