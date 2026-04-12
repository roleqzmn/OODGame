namespace OODGame.Entities
{
    public class Goblin : Enemy
    {
        public Goblin()
        {
            Name      = "Goblin";
            MaxHealth = 30;
            Health    = 30;
            Damage    = 5;
            Armor     = 2;
        }

        public Goblin(int health, int damage, int armor)
        {
            Name      = "Goblin";
            MaxHealth = health;
            Health    = health;
            Damage    = damage;
            Armor     = armor;
        }
    }
}
