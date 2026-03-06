using OODGame.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;
namespace OODGame.Map
{
    public abstract class Tile
    {
        public char Symbol { get; protected set; }
        public abstract bool CanEnter();
        public abstract void Interact(Player player);
        public abstract bool CanInteract();
    }

    public class EmptyTile : Tile
    {
        public EmptyTile() { Symbol = ' '; }
        public override bool CanEnter() => true;
        public override void Interact(Player player) { }
        public override bool CanInteract() => false;
    }

    public class WallTile : Tile
    {
        public WallTile() { Symbol = '█'; }
        public override bool CanEnter() => false;
        public override void Interact(Player player) { }
        public override bool CanInteract() => false;
    }

    public class ItemTile : Tile
    {
        public List<Item> Items { get; protected set; }
        public ItemTile(List<Item>? items) { 
            if (items != null) 
                Items = items;
            else 
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
                        if (i > 0)
                            i--;
                        Draw.EraseItem();
                        Draw.DrawItem(Items[i]);
                        break;

                    case ConsoleKey.RightArrow:
                        if (i < Items.Count - 1)
                            i++;
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
        public override bool CanInteract() => true;
    }

    public class ChestTile : Tile
    {
        public override bool CanEnter() => true;
        public override void Interact(Player player)
        {
            List<Item> items = GenerateItems();
            if (items.Count == 0)
                return;

            Draw.DrawItems(items);
            int i = 0;
            Draw.DrawItem(items[i]);

            while (true)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.Q:
                        Draw.EraseItems(items);
                        Draw.EraseItem();
                        return;

                    case ConsoleKey.LeftArrow:
                        if (i > 0)
                            i--;
                        Draw.EraseItem();
                        Draw.DrawItem(items[i]);
                        break;

                    case ConsoleKey.RightArrow:
                        if (i < items.Count - 1)
                            i++;
                        Draw.EraseItem();
                        Draw.DrawItem(items[i]);
                        break;

                    case ConsoleKey.E:
                        if (player.CanPickup(items[i]))
                        {
                            player.Pickup(items[i]);
                            items.RemoveAt(i);

                            if (items.Count == 0)
                            {
                                Draw.EraseItems(items);
                                Draw.EraseItem();
                                return;
                            }

                            if (i >= items.Count)
                                i = items.Count - 1;

                            Draw.EraseItems(items);
                            Draw.EraseItem();
                            Draw.DrawItems(items);
                            Draw.DrawItem(items[i]);
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
    }
}
