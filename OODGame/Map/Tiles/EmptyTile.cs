using OODGame.Items;
using OODGame.Players;
using System;
using System.Collections.Generic;

namespace OODGame.Map
{
    public class EmptyTile : Tile
    {
        public List<Item> Items { get; protected set; }

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
            Symbol = Items.Count > 0 ? 'I' : ' ';
        }

        public override bool CanEnter() => true;

        public override void Interact(Player player)
        {
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
                    case ConsoleKey.Q:
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
                        if (player.CanPickup(Items[i]))
                        {
                            player.Pickup(Items[i]);
                            Items.RemoveAt(i);
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

        public override bool CanInteract() => Items.Count > 0;

        public override void PlaceItem(Item item)
        {
            Items.Add(item);
            UpdateSymbol();
        }

        public override bool CanPlace() => true;
    }
}
