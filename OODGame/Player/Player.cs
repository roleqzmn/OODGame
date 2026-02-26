using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Items;

namespace OODGame.Player
{
    enum Stats
    {
        Strength, Dexterity, Health, Luck, Aggression, Wisdom
    }
    public class Player
    {
        public int x_pos { get; set; }
        public int y_pos { get; set; }
        public string Name { get; }
        public List<Item> Inventory { get; set; }
        public char Symbol { get; } = '¶';
        public Player(int startX = 0, int startY = 0, string name = "")
        {
            x_pos = startX;
            y_pos = startY;
            Name = name;
            Inventory = new List<Item>();
        }
    }
}
