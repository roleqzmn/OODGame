namespace OODGame.Entities
{
    public class Wolf : Enemy
    {
        public Wolf()
        {
            Name      = "Wolf";
            MaxHealth = 25;
            Health    = 25;
            Damage    = 6;
            Armor     = 2;
        }

        public Wolf(int health, int damage, int armor)
        {
            Name      = "Wolf";
            MaxHealth = health;
            Health    = health;
            Damage    = damage;
            Armor     = armor;
        }
    }
}
