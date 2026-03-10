using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Players;
using OODGame.Entities;

namespace OODGame.Items
{
    public abstract class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public char Symbol { get; set; }
        public Int16 Weight { get; set; }
        public abstract void OnPickedUp(Player player);
        public abstract bool CanEquip(Player player);
        public abstract void Equip(Player player);
        public Item(){
            Name=string.Empty; Description=string.Empty;
        }
    }
    public abstract class Armor : Item
    {
        public Int16 ArmorPoints {  get; set; }

    }
}
