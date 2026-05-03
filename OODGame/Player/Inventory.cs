using OODGame;
using OODGame.Items;

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
}