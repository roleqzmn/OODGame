using OODGame;
using OODGame.Actions;
using OODGame.Items;
using OODGame.Map;
using OODGame.Logger;

namespace OODGame.Players;

public class Inventory
{
    public List<Item> Items { get; private set; }
    public int Capacity { get; private set; }
    public int CurrentLoad { get; private set; }
    public int Count => Items.Count;

    public Item this[int index] => Items[index];

    public Inventory(int capacity)
    {
        Capacity = capacity;
        Items = new List<Item>();
        CurrentLoad = 0;
    }

    public bool AddItem(Item item)
    {
        if (CurrentLoad + item.Weight <= Capacity)
        {
            Items.Add(item);
            CurrentLoad += item.Weight;
            return true;
        }
        return false;
    }

    public bool RemoveItem(Item item)
    {
        if (Items.Remove(item))
        {
            CurrentLoad -= item.Weight;
            return true;
        }
        return false;
    }

    public bool RemoveAt(int index)
    {
        if (index < 0 || index >= Items.Count)
            return false;

        CurrentLoad -= Items[index].Weight;
        Items.RemoveAt(index);
        return true;
    }

    public void Open(Player player, Tile tile)
    {
        var playerActions = new PlayerActions();
        if (Count == 0) return;
        int i = 0;
        Draw.DrawItems(Items);
        Draw.DrawItemInv(Items[i], player);
        while (true)
        {
            int size = Count;
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Escape:
                    Draw.EraseItems(Items); Draw.EraseItem();
                    return;
                case ConsoleKey.LeftArrow:
                    if (i > 0) i--;
                    Draw.EraseItem(); Draw.DrawItemInv(Items[i], player);
                    break;
                case ConsoleKey.RightArrow:
                    if (i < size - 1) i++;
                    Draw.EraseItem(); Draw.DrawItemInv(Items[i], player);
                    break;
                case ConsoleKey.E:
                    if (Items[i].CanEquip(player))
                    {
                        var itemToEquip = Items[i];
                        if (itemToEquip.Equip(player))
                        {
                            RemoveItem(itemToEquip);
                            EventLogger.Instance?.LogEvent($"Player equipped: {itemToEquip.Name}.");
                            if (Count < 1) { Draw.EraseItem(); Draw.EraseItems(Items); return; }
                            if (i >= Count) i = Count - 1;
                            Draw.EraseItem();
                            Draw.EraseItems(Items);
                            Draw.DrawItems(Items);
                            Draw.DrawItemInv(Items[i], player);
                        }
                    }
                    break;
                case ConsoleKey.Q:
                    if (playerActions.DropFromInventory(player, tile, i).Success)
                    {
                        Draw.EraseItem();
                        Draw.EraseItems(Items);
                        if (Count < 1) return;
                        if (i >= Count) i = Count - 1;
                        Draw.DrawItems(Items);
                        Draw.DrawItemInv(Items[i], player);
                    }
                    break;
                default: break;
            }
        }
    }
}
