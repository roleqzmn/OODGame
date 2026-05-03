using OODGame.Items;
using OODGame.Items.Unequipable;
using OODGame.Players;
using OODGame.Actions;
using System;
using System.Collections.Generic;

namespace OODGame.Map
{
    public class ChestTile : Tile
    {
        private static readonly PlayerActions _playerActions = new PlayerActions();
        public List<Item> Items { get; protected set; }

        public ChestTile() 
        { 
            Items = new List<Item>();
            Symbol = '?';
        }

        public override bool CanEnter() => true;

        public override void Interact(Player player)
        {
            Items = GenerateItems();
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

        public override bool CanInteract() => true;

        public List<Item> GenerateItems()
        {
            return new List<Item>();
        }

        public override void PlaceItem(Item item)
        {
            Items.Add(item);
        }

        public override bool CanPlace() => true;
    }
}
