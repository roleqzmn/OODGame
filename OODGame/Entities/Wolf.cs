namespace OODGame.Entities
{
    public class Wolf : Enemy
    {
        public Wolf()
        {
            Name      = "Wolf";
            Species   = "Wolf";
            MaxHealth = 25;
            Health    = 25;
            Damage    = 6;
            Armor     = 2;
            Temperament = EnemyTemperament.Aggressive;
            SoundReaction = EnemyReactionMode.Follow;
            PlayerReaction = EnemyReactionMode.Follow;
        }

        public Wolf(int health, int damage, int armor)
        {
            Name      = "Wolf";
            Species   = "Wolf";
            MaxHealth = health;
            Health    = health;
            Damage    = damage;
            Armor     = armor;
            Temperament = EnemyTemperament.Aggressive;
            SoundReaction = EnemyReactionMode.Follow;
            PlayerReaction = EnemyReactionMode.Follow;
        }
    }
}
