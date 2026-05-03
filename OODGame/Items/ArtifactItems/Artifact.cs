using OODGame.Players;

namespace OODGame.Items.Artifacts
{
    public abstract class Artifact : Item
    {
        public override void OnPickedUp(Player player)
        {
            if (player.CanPickup(this))
                player.Pickup(this);
        }

        public override bool CanEquip(Player player) => false;
        public override bool Equip(Player player) => false;
    }
}
