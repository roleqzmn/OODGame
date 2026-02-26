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
        string Name { get; }
        string Description { get; }
        char Symbol { get; }
        void OnPickedUp(Player.Player player);
        bool TryEquip(Player.Player player);
    }
    public interface Weapon : Item
    {
        int Damage { get; set; }
        int MinLvl { get; }
        int MaxLvl { get; }
        bool TryAttack(Player.Player player);
        bool OnAttack(Player.Player player);
    }

}
