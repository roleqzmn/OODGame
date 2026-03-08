using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OODGame.Items;
using OODGame.Map;

namespace OODGame.Players
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
    public class EquippedItems
    {
        public bool HasTwoHanded { get; set; }
        public Weapon? RightHand { get; set; }
        public Weapon? LeftHand { get; set; }
        public Armor? Helmet { get; set; }
        public Armor? Chestplate { get; set; }
        public Armor? Leggins { get; set; }
        public Armor? Boots { get; set; }
        public Armor? Gloves { get; set; }
    }
    public class Player
    {
        public int Xpos { get; set; }
        public int Ypos { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int SkillPoints { get; set; }
        public Attributes Stats { get; private set; }
        public EquippedItems EItems { get; set; }
        public string Name { get; }
        public List<Item> Inventory { get; set; } //for later
        public char Symbol { get; } = '¶';
        public Player(int startX = 0, int startY = 0, string name = "roleq"){
            Xpos = startX;
            Ypos = startY;
            Name = name;
            Inventory = new List<Item>();
            Stats = new Attributes(10, 10, 100, 5, 2, 5, 20);
            EItems = new EquippedItems();
        }
        public bool CanPickup(Item item){
            return (item.Weight + Stats.CurrentLoad < Stats.InventoryLimit);
        }
        public void Pickup(Item item) {
            Inventory.Add(item);
        }
        public void OpenInventory(Tile tile)
        {
            int i=0, size = Inventory.Count;
            Draw.DrawItems(Inventory);
            Draw.DrawItemInv(Inventory[i]);
            while (true)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Escape: 
                        Draw.EraseItems(Inventory); Draw.EraseItem();
                        return;
                    case ConsoleKey.LeftArrow: 
                        if (i > 0) 
                            i--; 
                        Draw.EraseItem(); if (size > 0) Draw.DrawItemInv(Inventory[i]); 
                        break;
                    case ConsoleKey.RightArrow: 
                        if (i < size-1) 
                            i++; 
                        Draw.EraseItem(); if (size > 0) Draw.DrawItemInv(Inventory[i]); 
                        break;
                    case ConsoleKey.E: 
                        if (Inventory[i].CanEquip(this)) 
                        {
                            Inventory[i].Equip(this); 
                            Inventory.RemoveAt(i);
                            Draw.EraseItem();
                            Draw.EraseItems(Inventory);
                            size=Inventory.Count;
                            if (size < 1) return;
                            Draw.DrawItems(Inventory);
                            if (i > 0) i--;
                            Draw.DrawItemInv(Inventory[i]);
                        } 
                        break;
                    case ConsoleKey.Q: 
                        if (tile.CanPlace()) 
                        { 
                            tile.PlaceItem(Inventory[i]);
                            Inventory.RemoveAt(i);
                            size = Inventory.Count;
                            Draw.EraseItem();
                            Draw.EraseItems(Inventory);
                            if (size < 1) return;
                            Draw.DrawItems(Inventory);
                            if (i > 0) i--;
                            Draw.DrawItemInv(Inventory[i]);
                        }
                        break;
                    default: break;
                }
            }
        }
    }
}
