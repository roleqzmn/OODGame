using OODGame.Items;
using OODGame.Players;
using OODGame.Actions;
using OODGame.Entities;
using OODGame.Events;
using OODGame.Fight;
using OODGame.Logger;
using System.Collections.Generic;

namespace OODGame.Map
{
    public class EmptyTile : Tile
    {
        private static readonly PlayerActions _playerActions = new PlayerActions();
        public List<Item> Items { get; protected set; }
        public Enemy? Enemy { get; private set; }
        public bool HasEnemy => Enemy != null && Enemy.IsAlive;

        public EmptyTile(List<Item>? items)
        {
            if (items != null)
                Items = items;
            else
                Items = new List<Item>();
            UpdateSymbol();
        }

        public EmptyTile()
        {
            Items = new List<Item>();
            UpdateSymbol();
        }

        private void UpdateSymbol()
        {
            if (HasEnemy)
                Symbol = Enemy!.Name[0];
            else
                Symbol = Items.Count > 0 ? 'I' : ' ';
        }

        public override bool CanEnter() => true;

        public override void Interact(Player player)
        {
            if (HasEnemy)
            {
                var fight = new FightRunner(player, Enemy!);
                bool enemyDefeated = fight.Run();

                if (enemyDefeated)
                {
                    player.EventBus?.Publish(new EnemyDeathEvent(Enemy!.Id, Enemy.Species));
                    player.EventBus?.Unsubscribe(Enemy!);
                    Enemy!.ClearSpatialContext();
                    EventLogger.Instance?.LogEvent($"{player.Name} defeated {Enemy.Name}.");
                    RemoveEnemy();
                }

                return;
            }

            return;
        }

        public override bool CanInteract() => HasEnemy || Items.Count > 0;

        public void SetEnemy(Enemy enemy)
        {
            Enemy = enemy;
            UpdateSymbol();
        }

        public void RemoveEnemy()
        {
            Enemy = null;
            UpdateSymbol();
        }

        public PlayerActionResult PickupItem(Player player, int itemIndex)
        {
            var result = _playerActions.PickupFromTile(player, Items, itemIndex);
            if (result.Success)
                UpdateSymbol();
            return result;
        }

        public override void PlaceItem(Item item)
        {
            Items.Add(item);
            UpdateSymbol();
        }

        public override bool CanPlace() => true;
    }
}
