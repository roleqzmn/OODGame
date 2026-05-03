using OODGame.Items;
using OODGame.Map;
using OODGame.Players;
using System.Collections.Generic;

namespace OODGame.Actions
{
    public sealed class PlayerActionResult
    {
        public bool Success { get; }
        public string Message { get; }

        public PlayerActionResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }

    public sealed class PickupItemAction : IAction<PlayerActionResult>
    {
        private readonly Player _player;
        private readonly List<Item> _tileItems;
        private readonly int _itemIndex;

        public PickupItemAction(Player player, List<Item> tileItems, int itemIndex)
        {
            _player = player;
            _tileItems = tileItems;
            _itemIndex = itemIndex;
        }

        public PlayerActionResult Execute()
        {
            if (_itemIndex < 0 || _itemIndex >= _tileItems.Count)
                return new PlayerActionResult(false, "Invalid item index.");

            var item = _tileItems[_itemIndex];
            if (!_player.TryPickup(item))
                return new PlayerActionResult(false, "Cannot pick up this item.");

            _tileItems.RemoveAt(_itemIndex);
            return new PlayerActionResult(true, $"Picked up {item.Name}.");
        }
    }

    public sealed class DropItemAction : IAction<PlayerActionResult>
    {
        private readonly Player _player;
        private readonly Tile _tile;
        private readonly int _inventoryIndex;

        public DropItemAction(Player player, Tile tile, int inventoryIndex)
        {
            _player = player;
            _tile = tile;
            _inventoryIndex = inventoryIndex;
        }

        public PlayerActionResult Execute()
        {
            if (!_tile.CanPlace())
                return new PlayerActionResult(false, "Cannot drop item on this tile.");

            if (_inventoryIndex < 0 || _inventoryIndex >= _player.Inventory.Count)
                return new PlayerActionResult(false, "Invalid inventory index.");

            var item = _player.Inventory[_inventoryIndex];
            if (!_player.TryDrop(item, _tile))
                return new PlayerActionResult(false, "Cannot drop this item.");

            return new PlayerActionResult(true, $"Dropped {item.Name}.");
        }
    }

    public sealed class PlayerActions
    {
        public PlayerActionResult PickupFromTile(Player player, List<Item> tileItems, int itemIndex)
        {
            return new PickupItemAction(player, tileItems, itemIndex).Execute();
        }

        public PlayerActionResult DropFromInventory(Player player, Tile tile, int inventoryIndex)
        {
            return new DropItemAction(player, tile, inventoryIndex).Execute();
        }
    }
}