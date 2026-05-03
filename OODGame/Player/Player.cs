using OODGame.Items;
using OODGame.Actions;
using OODGame.Map;
using OODGame.Entities;
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
            InventoryLimit = lim; //przepisac ten kontruktor
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
    public class Player: IEntity
    {
        public int Xpos { get; set; }
        public int Ypos { get; set; }
        public int Level { get; set; }
        public int Experience { get; set; }
        public int SkillPoints { get; set; }
        public Attributes Stats { get; private set; }
        public EquippedItems EItems { get; set; }
        public string Name { get; }
        public Inventory Inventory { get; set; } //zrób klase inventory deklu
        public char Symbol { get; } = '¶';
        public Player(int startX = 0, int startY = 0, string name = "roleq"){
            Xpos = startX;
            Ypos = startY;
            Name = name;
            Inventory = new Inventory(20);
            Stats = new Attributes(10, 10, 100, 5, 2, 5, 20);
            EItems = new EquippedItems();
            SyncCurrentLoad();
        }

        public bool CanPickup(Item item) => Inventory.CurrentLoad + item.Weight <= Stats.InventoryLimit;

        public bool TryPickup(Item item)
        {
            if (!CanPickup(item))
                return false;

            bool picked = Inventory.AddItem(item);
            if (picked)
                SyncCurrentLoad();

            return picked;
        }

        public bool TryDrop(Item item, Tile tile)
        {
            if (!tile.CanPlace())
                return false;

            if (!Inventory.RemoveItem(item))
                return false;

            tile.PlaceItem(item);
            SyncCurrentLoad();
            return true;
        }

        public void Pickup(Item item)
        {
            TryPickup(item);
        }

        private void SyncCurrentLoad()
        {
            Stats.CurrentLoad = Inventory.CurrentLoad;
        }

        private void ReturnToInventory(Weapon item) {
            if (Inventory.AddItem(item))
                SyncCurrentLoad();
        }

        public void OpenInventory(Tile tile)
        {
            var playerActions = new PlayerActions();
            if (Inventory.Count == 0) return;
            int i = 0;
            Draw.DrawItems(Inventory.Items);
            Draw.DrawItemInv(Inventory[i], this);
            while (true)
            {
                int size = Inventory.Count;
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.Escape:
                        Draw.EraseItems(Inventory.Items); Draw.EraseItem();
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
                            if (itemToEquip.Equip(this))
                            {
                                Inventory.RemoveItem(itemToEquip);
                                SyncCurrentLoad();
                                if (Inventory.Count < 1) { Draw.EraseItem(); Draw.EraseItems(Inventory.Items); return; }
                                if (i >= Inventory.Count) i = Inventory.Count - 1;
                                Draw.EraseItem();
                                Draw.EraseItems(Inventory.Items);
                                Draw.DrawItems(Inventory.Items);
                                Draw.DrawItemInv(Inventory[i], this);
                            }
                        }
                        break;
                    case ConsoleKey.Q:
                        if (playerActions.DropFromInventory(this, tile, i).Success)
                        {
                            Draw.EraseItem();
                            Draw.EraseItems(Inventory.Items);
                            if (Inventory.Count < 1) return;
                            if (i >= Inventory.Count) i = Inventory.Count - 1;
                            Draw.DrawItems(Inventory.Items);
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

        public bool EquipWeapon(Weapon item) //przenies do equpped item
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