using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Player;

namespace OODGame.Items
{
    public interface Item
    {
        public string Name { get; }
        public string Description { get; }
        public char Symbol { get; }
        public void OnPickedUp(Player.Player player);
        public bool TryEquip(Player.Player player);
    }
    public interface Weapon : Item
    {
        public int Damage { get; set; }
        public int MinLvl { get; }
        public int MaxLvl { get; }
        public bool TryAttack(Player.Player player);
        public void OnAttack(Player.Player player);
    }

}
