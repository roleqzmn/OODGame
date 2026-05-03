namespace OODGame.Entities
{
    public class Demon : Enemy
    {
        public Demon()
        {
            Name      = "Demon";
            MaxHealth = 50;
            Health    = 50;
            Damage    = 10;
            Armor     = 5;
        }

        public Demon(int health, int damage, int armor)
        {
            Name      = "Demon";
            MaxHealth = health;
            Health    = health;
            Damage    = damage;
            Armor     = armor;
        }
    }
}
