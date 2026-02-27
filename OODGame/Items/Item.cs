using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Player;
using OODGame.Entities;

namespace OODGame.Items
{
    public abstract class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public char Symbol { get; set; }
        public Int16 Weight { get; set; }
        public abstract void OnPickedUp(Player.Player player);
        public abstract bool CanEquip(Player.Player player);
        public abstract void Equip(Player.Player player);
        public Item(){
            Name=String.Empty; Description=String.Empty;
        }
    }
    public abstract class Weapon : Item
    {
        public Int16 Damage { get; set; }
        public Int16 MinLvl { get; set; }
        public Int16 AttackRate { get; set; }
        public Int16 Range { get; set; }
        public abstract bool IsTwoHanded { get; }
        public abstract void Attack(Player.Player player, IEnemy enemy);
    }
    public abstract class Armor : Item
    {
        public Int16 ArmorPoints {  get; set; }

    }
}
