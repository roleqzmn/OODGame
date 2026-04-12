using OODGame.Items;
using OODGame.Players;

namespace OODGame.Map
{
    public class WallTile : Tile
    {
        public WallTile() { Symbol = '█'; }
        public override bool CanEnter() => false;
        public override void Interact(Player player) { }
        public override bool CanInteract() => false;
        public override void PlaceItem(Item item) { }
        public override bool CanPlace() => false;
    }
}