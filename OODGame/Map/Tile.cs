using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODDGame;
using OODGame.Items;
namespace OODGame.Map
{
    public abstract class Tile
    {
        public char Symbol { get; protected set; }
        public abstract bool CanEnter();
        public abstract void Interact();
        public abstract bool CanInteract();
    }

    public class EmptyTile : Tile
    {
        public EmptyTile() { Symbol = ' '; }
        public override bool CanEnter() => true;
        public override void Interact() { }
        public override bool CanInteract() => false;
    }

    public class WallTile : Tile
    {
        public WallTile() { Symbol = '█'; }
        public override bool CanEnter() => false;
        public override void Interact() { }
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
        public override void Interact()
        {
        }
        public override bool CanInteract() => true;
    }

    public class ChestTile : Tile
    {
        public override bool CanEnter() => true;
        public override void Interact() {
            var key = Console.ReadKey(true).Key;
            List<Item> items = GenerateItems();
            while (true)
            {
                switch (key)
                {

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
