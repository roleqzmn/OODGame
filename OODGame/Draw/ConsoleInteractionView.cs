using OODGame.Actions;
using OODGame.Input;
using OODGame.Items;
using OODGame.Logger;
using OODGame.Map;
using OODGame.Players;

namespace OODGame
{
    public static class ConsoleInteractionView
    {
        public static void OpenTileItems(Player player, EmptyTile tile, IInputSource? inputSource = null)
        {
            if (tile.Items.Count == 0)
                return;

            int i = 0;
            IInputSource source = inputSource ?? new ConsoleInputSource();
            Draw.DrawItems(tile.Items);
            Draw.DrawItem(tile.Items[i]);

            while (true)
            {
                var key = source.ReadKey();
                switch (key)
                {
                    case ConsoleKey.Escape:
                        Draw.EraseItems(tile.Items);
                        Draw.EraseItem();
                        return;

                    case ConsoleKey.LeftArrow:
                        if (i > 0) i--;
                        Draw.EraseItem();
                        Draw.DrawItem(tile.Items[i]);
                        break;

                    case ConsoleKey.RightArrow:
                        if (i < tile.Items.Count - 1) i++;
                        Draw.EraseItem();
                        Draw.DrawItem(tile.Items[i]);
                        break;

                    case ConsoleKey.E:
                        if (tile.PickupItem(player, i).Success)
                        {
                            if (tile.Items.Count == 0)
                            {
                                Draw.EraseItems(tile.Items);
                                Draw.EraseItem();
                                return;
                            }

                            if (i >= tile.Items.Count)
                                i = tile.Items.Count - 1;

                            Draw.EraseItems(tile.Items);
                            Draw.EraseItem();
                            Draw.DrawItems(tile.Items);
                            Draw.DrawItem(tile.Items[i]);
                        }
                        break;
                }
            }
        }

        public static void OpenInventory(Player player, Tile tile, IInputSource? inputSource = null)
        {
            var inventory = player.Inventory;
            var playerActions = new PlayerActions();
            if (inventory.Count == 0)
                return;

            IInputSource source = inputSource ?? new ConsoleInputSource();
            int i = 0;
            Draw.DrawItems(inventory.Items);
            Draw.DrawItemInv(inventory.Items[i], player);

            while (true)
            {
                int size = inventory.Count;
                var key = source.ReadKey();

                switch (key)
                {
                    case ConsoleKey.Escape:
                        Draw.EraseItems(inventory.Items);
                        Draw.EraseItem();
                        return;

                    case ConsoleKey.LeftArrow:
                        if (i > 0) i--;
                        Draw.EraseItem();
                        Draw.DrawItemInv(inventory.Items[i], player);
                        break;

                    case ConsoleKey.RightArrow:
                        if (i < size - 1) i++;
                        Draw.EraseItem();
                        Draw.DrawItemInv(inventory.Items[i], player);
                        break;

                    case ConsoleKey.E:
                        if (inventory.Items[i].CanEquip(player))
                        {
                            var itemToEquip = inventory.Items[i];
                            bool equipSuccess = TryEquipItem(player, itemToEquip, source);
                            if (equipSuccess)
                            {
                                inventory.RemoveItem(itemToEquip);
                                EventLogger.Instance?.LogEvent($"Player equipped: {itemToEquip.Name}.");
                                Draw.EraseEq();
                                Draw.DrawEq(player);

                                if (inventory.Count < 1)
                                {
                                    Draw.EraseItem();
                                    Draw.EraseItems(inventory.Items);
                                    return;
                                }

                                if (i >= inventory.Count)
                                    i = inventory.Count - 1;

                                Draw.EraseItem();
                                Draw.EraseItems(inventory.Items);
                                Draw.DrawItems(inventory.Items);
                                Draw.DrawItemInv(inventory.Items[i], player);
                            }
                        }
                        break;

                    case ConsoleKey.Q:
                        if (playerActions.DropFromInventory(player, tile, i).Success)
                        {
                            Draw.EraseItem();
                            Draw.EraseItems(inventory.Items);
                            if (inventory.Count < 1)
                                return;
                            if (i >= inventory.Count)
                                i = inventory.Count - 1;
                            Draw.DrawItems(inventory.Items);
                            Draw.DrawItemInv(inventory.Items[i], player);
                        }
                        break;
                }
            }
        }

        private static bool TryEquipItem(Player player, Item item, IInputSource inputSource)
        {
            if (item is Weapon weapon)
            {
                if (!weapon.IsTwoHanded)
                {
                    Draw.DrawHandChoice();
                    var choice = inputSource.ReadKey();
                    Draw.EraseHandChoice();

                    if (choice == ConsoleKey.L)
                        return player.EquipWeapon(weapon, WeaponHand.Left);
                    if (choice == ConsoleKey.R)
                        return player.EquipWeapon(weapon, WeaponHand.Right);
                    return false;
                }

                return player.EquipWeapon(weapon, WeaponHand.Right);
            }

            return item.Equip(player);
        }
    }
}
