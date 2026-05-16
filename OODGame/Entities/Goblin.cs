using System;

namespace OODGame.Entities
{
    public class Goblin : Enemy
    {
        public Goblin()
        {
            Name      = "Goblin";
            Species   = "Goblin";
            MaxHealth = 30;
            Health    = 30;
            Damage    = 5;
            Armor     = 2;
        }

        public Goblin(int health, int damage, int armor)
        {
            Name      = "Goblin";
            Species   = "Goblin";
            MaxHealth = health;
            Health    = health;
            Damage    = damage;
            Armor     = armor;
        }

        protected override void OnAllyDeath()
        {
            Damage = Math.Max(1, Damage - 1);
            Armor = Math.Max(0, Armor - 1);
        }
    }
}
