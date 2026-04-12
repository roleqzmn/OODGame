using OODGame.Items;
using OODGame.Players;

namespace OODGame.Map
{
    public abstract class Tile
    {
        public char Symbol { get; protected set; }
        public abstract bool CanEnter();
        public abstract void Interact(Player player);
        public abstract bool CanInteract();
        public abstract void PlaceItem(Item item);
        public abstract bool CanPlace();
    }
}
