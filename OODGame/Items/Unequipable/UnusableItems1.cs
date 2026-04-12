using OODGame.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Items.Unequipable
{
    public class Unusable1 : Item
    {
        public Unusable1()
        {
            Name = "Burnt Page";
            Description = "Unreadable page";
            Symbol = 'U';
        }
        public override void OnPickedUp(Player player){
            if (player.CanPickup(this))
                player.Pickup(this);
        }
        public override bool CanEquip(Player player) => false;
        public override bool Equip(Player player) => false;

    }
    public class Unusable2 : Item
    {
        public Unusable2()
        {
            Name = "Rusty Knife";
            Description = "Don't hurt yourself";
            Symbol = 'U';
        }
        public override void OnPickedUp(Player player)
        {
            if (player.CanPickup(this))
                player.Pickup(this);
        }
        public override bool CanEquip(Player player) => false;
        public override bool Equip(Player player) => false;
    }
    public class Unusable3 : Item
    {
        public Unusable3()
        {
            Name = "Broken Handle";
            Description = "Unrepariable :c";
            Symbol = 'U';
        }
        public override void OnPickedUp(Player player)
        {
            if (player.CanPickup(this))
                player.Pickup(this);
        }
        public override bool CanEquip(Player player) => false;
        public override bool Equip(Player player) => false;
    }
}
