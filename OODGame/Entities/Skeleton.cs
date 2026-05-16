namespace OODGame.Entities
{
    public class Skeleton : Enemy
    {
        public Skeleton()
        {
            Name      = "Skeleton";
            Species   = "Skeleton";
            MaxHealth = 20;
            Health    = 20;
            Damage    = 8;
            Armor     = 1;
        }

        public Skeleton(int health, int damage, int armor)
        {
            Name      = "Skeleton";
            Species   = "Skeleton";
            MaxHealth = health;
            Health    = health;
            Damage    = damage;
            Armor     = armor;
        }

        protected override void OnAllyDeath()
        {
            Damage += 1;
            Armor += 1;
        }
    }
}
