namespace OODGame.Entities
{
    public class Demon : Enemy
    {
        public Demon()
        {
            Name      = "Demon";
            Species   = "Demon";
            MaxHealth = 50;
            Health    = 50;
            Damage    = 10;
            Armor     = 5;
            Temperament = EnemyTemperament.Aggressive;
            SoundReaction = EnemyReactionMode.Follow;
            PlayerReaction = EnemyReactionMode.Follow;
        }

        public Demon(int health, int damage, int armor)
        {
            Name      = "Demon";
            Species   = "Demon";
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
