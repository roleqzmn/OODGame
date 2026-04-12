using OODGame.Items;
using OODGame.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
        public bool CanPickup(Item item) => (item.Weight + Stats.CurrentLoad < Stats.InventoryLimit);
        public void Pickup(Item item) {
            Inventory.Add(item);
            Stats.CurrentLoad += item.Weight;
        }
        private void ReturnToInventory(Weapon item) {
            Inventory.Add(item);
        }
        public void OpenInventory(Tile tile)
        {
            if (Inventory.Count == 0) return;
            int i = 0;
            Draw.DrawItems(Inventory);
            Draw.DrawItemInv(Inventory[i], this);
            while (true)
            {
                int size = Inventory.Count;
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Escape:
                        Draw.EraseItems(Inventory); Draw.EraseItem();
                        return;
                    case ConsoleKey.LeftArrow:
                        if (i > 0) i--;
                        Draw.EraseItem(); Draw.DrawItemInv(Inventory[i], this);
                        break;
                    case ConsoleKey.RightArrow:
                        if (i < size - 1) i++;
                        Draw.EraseItem(); Draw.DrawItemInv(Inventory[i], this);
                        break;
                    case ConsoleKey.E:
                        if (Inventory[i].CanEquip(this))
                        {
                            var itemToEquip = Inventory[i];
                            Stats.CurrentLoad -= itemToEquip.Weight;
                            if (itemToEquip.Equip(this))
                            {
                                Inventory.Remove(itemToEquip);
                                if (Inventory.Count < 1) { Draw.EraseItem(); Draw.EraseItems(Inventory); return; }
                                if (i >= Inventory.Count) i = Inventory.Count - 1;
                                Draw.EraseItem();
                                Draw.EraseItems(Inventory);
                                Draw.DrawItems(Inventory);
                                Draw.DrawItemInv(Inventory[i], this);
                            }
                            else
                            {
                                Stats.CurrentLoad += itemToEquip.Weight;
                            }
                        }
                        break;
                    case ConsoleKey.Q:
                        if (tile.CanPlace())
                        {
                            tile.PlaceItem(Inventory[i]);
                            Stats.CurrentLoad -= Inventory[i].Weight;
                            Inventory.RemoveAt(i);
                            Draw.EraseItem();
                            Draw.EraseItems(Inventory);
                            if (Inventory.Count < 1) return;
                            if (i >= Inventory.Count) i = Inventory.Count - 1;
                            Draw.DrawItems(Inventory);
                            Draw.DrawItemInv(Inventory[i], this);
                        }
                        break;
                    default: break;
                }
            }
        }

        public void InteractWithTile(Tile tile)
        {
            if(tile.CanInteract())
                tile.Interact(this);
        }

        public bool EquipWeapon(Weapon item)
        {
            if (!item.CanEquip(this))
                return false;
            if (!item.IsTwoHanded)
            {
                if (EItems.HasTwoHanded)
                {
                    EItems.HasTwoHanded = false;
                    if (EItems.RightHand != null)
                        ReturnToInventory(EItems.RightHand);
                    EItems.RightHand = null;
                }
                Draw.DrawHandChoice();
                var choice = Console.ReadKey(true).Key;
                Draw.EraseHandChoice();

                if (choice == ConsoleKey.L)
                {
                    if (EItems.LeftHand != null)
                        ReturnToInventory(EItems.LeftHand);
                    EItems.LeftHand = (Weapon)item;
                    Draw.EraseEq();
                    Draw.DrawEq(this);
                    return true;
                }
                else if (choice == ConsoleKey.R)
                {
                    if (EItems.RightHand != null)
                        ReturnToInventory(EItems.RightHand);
                    EItems.RightHand = (Weapon)item;
                    Draw.EraseEq();
                    Draw.DrawEq(this);
                    return true;
                }
                return false;
            }
            else
            {
                EItems.HasTwoHanded = true;

                if (EItems.RightHand != null)
                    ReturnToInventory(EItems.RightHand);
                if (EItems.LeftHand != null)
                    ReturnToInventory(EItems.LeftHand);

                EItems.RightHand = item;
                EItems.LeftHand = null;
                Draw.EraseEq();
                Draw.DrawEq(this);
                return true;
            }
        }
    }
}