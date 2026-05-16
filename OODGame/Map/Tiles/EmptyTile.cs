using OODGame.Items;
using OODGame.Players;
using OODGame.Actions;
using OODGame.Entities;
using OODGame.Events;
using OODGame.Fight;
using OODGame.Logger;
using System;
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

            if (Items.Count == 0)
                return;

            Draw.DrawItems(Items);
            int i = 0;
            Draw.DrawItem(Items[i]);

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Escape:
                        Draw.EraseItems(Items);
                        Draw.EraseItem();
                        return;

                    case ConsoleKey.LeftArrow:
                        if (i > 0) i--;
                        Draw.EraseItem();
                        Draw.DrawItem(Items[i]);
                        break;

                    case ConsoleKey.RightArrow:
                        if (i < Items.Count - 1) i++;
                        Draw.EraseItem();
                        Draw.DrawItem(Items[i]);
                        break;

                    case ConsoleKey.E:
                        if (_playerActions.PickupFromTile(player, Items, i).Success)
                        {
                            UpdateSymbol();

                            if (Items.Count == 0)
                            {
                                Draw.EraseItems(Items);
                                Draw.EraseItem();
                                return;
                            }

                            if (i >= Items.Count)
                                i = Items.Count - 1;

                            Draw.EraseItems(Items);
                            Draw.EraseItem();
                            Draw.DrawItems(Items);
                            Draw.DrawItem(Items[i]);
                        }
                        break;

                    default:
                        break;
                }
            }
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

        public override void PlaceItem(Item item)
        {
            Items.Add(item);
            UpdateSymbol();
        }

        public override bool CanPlace() => true;
    }
}
