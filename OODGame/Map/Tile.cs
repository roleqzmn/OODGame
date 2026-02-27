using ODDGame;
using OODGame.Items;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Player;
namespace OODGame.Map
{
    public abstract class Tile
    {
        public char Symbol { get; protected set; }
        public abstract bool CanEnter();
        public abstract void Interact(Player.Player player);
        public abstract bool CanInteract();
    }

    public class EmptyTile : Tile
    {
        public EmptyTile() { Symbol = ' '; }
        public override bool CanEnter() => true;
        public override void Interact(Player.Player player) { }
        public override bool CanInteract() => false;
    }

    public class WallTile : Tile
    {
        public WallTile() { Symbol = '█'; }
        public override bool CanEnter() => false;
        public override void Interact(Player.Player player) { }
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
            Symbol = 'I'; 
        }
        public override bool CanEnter() => true;
        public override void Interact(Player.Player player)
        {
        }
        public override bool CanInteract() => true;
    }

    public class ChestTile : Tile
    {
        public override bool CanEnter() => true;
        public override void Interact(Player.Player player) {
            List<Item> items = GenerateItems();
            Draw.DrawItems(items);
            var key = Console.ReadKey(true).Key;
            switch (key)
            {
                case ConsoleKey.Escape: Draw.EraseItems(); Draw.EraseItem(); return;
                case ConsoleKey.LeftArrow: break;
                case ConsoleKey.RightArrow: break;
                case ConsoleKey.E: break;
                default: break;
            }
        }
        public override bool CanInteract() => true;
        public List<Item> GenerateItems()
        {
            return new List<Item>();
        }
    }
}
