using OODGame.Entities;
using OODGame.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OODGame.Items
{
    public abstract class Weapon : Item
    {
        public Int16 Damage { get; set; }
        public Int16 MinLvl { get; set; }
        public Int16 AttackRate { get; set; }
        public Int16 Range { get; set; }
        public abstract bool IsTwoHanded { get; }
        public abstract void Attack(Player player, Enemy enemy);
        public override void Equip(Player player)
        {
            player.EquipWeapon(this);
        }
    }
    public abstract class OneHandedWeapon : Weapon
    {
      
    }
    public abstract class TwoHandedWeapon : Weapon
    {

    }
}
