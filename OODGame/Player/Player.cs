using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Items;

namespace OODGame.Player
{
    public class Attributes
    {
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Health { get; set; }
        public int MaxHealth { get; set; }
        public int Luck { get; set; }
        public int Aggression { get; set; }
        public int Wisdom { get; set; }
        public int InventoryLimit { get; set; }
        public int CurrentLoad { get; set; }
        public int Gold { get; set; }
        public int Coins { get; set; }

        public Attributes(int str, int dex, int hp, int lck, int agg, int wis, int lim)
        {
            Strength = str;
            Dexterity = dex;
            Health = hp;
            MaxHealth = hp;
            Luck = lck;
            Aggression = agg;
            Wisdom = wis;
            InventoryLimit = lim;
        }
    }
    public class Player
    {
        public int x_pos { get; set; }
        public int y_pos { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int SkillPoints { get; set; }
        public Attributes Stats { get; private set; }
        public string Name { get; }
        public List<Item> Inventory { get; set; } //for later
        public char Symbol { get; } = '¶';

        public Player(int startX = 0, int startY = 0, string name = "roleq")
        {
            x_pos = startX;
            y_pos = startY;
            Name = name;
            Inventory = new List<Item>();
            Stats = new Attributes(10, 10, 100, 5, 2, 5, 20);
        }
    }
}
