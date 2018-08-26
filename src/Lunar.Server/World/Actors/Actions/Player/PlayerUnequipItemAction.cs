using System;
using Lunar.Core.World;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Actors.Actions.Player
{
    public class PlayerUnequipItemAction : IAction<Actors.Player>
    {
        private readonly int _slotNum;

        public PlayerUnequipItemAction(int slotNum)
        {
            _slotNum = slotNum;
        }

        public void Execute(Actors.Player player)
        {
            // Sanity check: is there actually an item in this slot?
            if (player.Equipment.GetSlot(_slotNum) == null)
            {
                // Log it!
                Logger.LogEvent($"Player attempted to unequip bad item! User: {player.Descriptor.Name} SlotNum: {_slotNum}.", LogTypes.GAME, Environment.StackTrace);

                return;
            }

            var item = player.Equipment.GetSlot(_slotNum);

            if (item.Descriptor.ItemType != ItemTypes.Equipment || item.Descriptor.SlotType == EquipmentSlots.NE)
            {
                // Log it!
                Logger.LogEvent($"Player attempted to unequip unequippable item! User: {player.Descriptor.Name} SlotNum: {_slotNum}.", LogTypes.GAME, Environment.StackTrace);

                return;
            }

            player.Equipment.SetSlot(_slotNum, null);
            player.Inventory.Add(item, 1);
            player.NetworkComponent.SendEquipmentUpdate();
            player.CalculateBoostedStats();
        }
    }
}
