using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Entities
{
    public abstract class Entity
    {
        public string Name { get; }
        public Entity() { Name=string.Empty;}
    }
    public abstract class Enemy : Entity
    {
        public int Armor { get; }
        public int Damage { get; } 
        public int AttackSpeed { get; }
        public int MaxHealth { get; }
        public int Health { get; set; }
    }
}
