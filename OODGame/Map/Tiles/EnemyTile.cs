using OODGame.Entities;
using OODGame.Fight;
using OODGame.Items;
using OODGame.Players;

namespace OODGame.Map
{
    public class EnemyTile : Tile
    {
        public Enemy Enemy { get; private set; }

        public EnemyTile(Enemy enemy)
        {
            Enemy = enemy;
            Symbol = enemy.Name[0]; 
        }   

        public override bool CanEnter() => true;
        public override bool CanInteract() => true;
        public override bool CanPlace() => false;

        public override void Interact(Player player)
        {
            var fight = new FightRunner(player, Enemy);
            bool enemyDefeated = fight.Run();

            if (enemyDefeated)
                Symbol = ' ';
        }

        public override void PlaceItem(Item item) { }
    }
}
