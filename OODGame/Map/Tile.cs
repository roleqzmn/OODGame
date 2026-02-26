using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ODDGame;
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
        public Items.Item Item { get; protected set; }
        public ItemTile(Items.Item item) { Item = item; Symbol = Item.Symbol; }
        public override bool CanEnter() => true;
        public override void Interact()
        {

        }
        public override bool CanInteract() => true;
    }
}
