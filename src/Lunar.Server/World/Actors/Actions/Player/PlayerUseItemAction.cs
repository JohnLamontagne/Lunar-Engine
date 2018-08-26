using System;
using Lunar.Core.World;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Actors.Actions.Player
{
    class PlayerUseItemAction : IAction<Actors.Player>
    {
        private readonly int _slotNum;

        public PlayerUseItemAction(int slotNum)
        {
            _slotNum = slotNum;
        }

        public void Execute(Actors.Player player)
        {
            // Sanity check: is there actually an item in this slot?
            if (player.Inventory.GetSlot(_slotNum) == null)
            {
                // Log it!
                Logger.LogEvent($"Player attempted to equip bad item! User: {player.Descriptor.Name} SlotNum: {_slotNum}.", LogTypes.GAME, Environment.StackTrace);

                return;
            }

            Item item = player.Inventory.GetSlot(_slotNum).Item;

            if (item.Descriptor.ItemType == ItemTypes.Equipment)
            {
                player.Equipment.Equip(item);
                item.OnEquip(player);
                player.Inventory.RemoveItem(_slotNum, 1);
            }
            else if (item.Descriptor.ItemType == ItemTypes.Usable)
            {
                item.OnUse(player);
            }
        }
    }
}
