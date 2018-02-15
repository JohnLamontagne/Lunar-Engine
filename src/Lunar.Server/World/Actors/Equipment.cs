using System.Collections.Generic;
using System.Linq;
using Lunar.Core;
using Lunar.Core.World;

namespace Lunar.Server.World.Actors
{
    public class Equipment
    {
        private readonly Player _player;
        private readonly Item[] _equipment;

        public IEnumerable<Item> Items => _equipment.ToList();

        public Equipment(Player player)
        {
            _player = player;
            _equipment = new Item[(int)EquipmentSlots.COUNT];
        }

        public Item GetSlot(int slotNum)
        {
            return _equipment[slotNum];
        }

        public void SetSlot(int slotNum, Item value)
        {
            _equipment[slotNum] = value;
        }

        /// <summary>
        /// Equips the specified item from the specified item slot (default: -1)
        /// </summary>
        /// <param name="item">Item to be equipped</param>
        public void Equip(Item item)
        {

            if (item.SlotType == EquipmentSlots.NE)
            {
                _player.SendChatMessage("You cannot equip this item!", ChatMessageType.Alert);
                return;
            }

            if (_equipment[(int)item.SlotType] != null)
            {
                var unequippedItem = _equipment[(int)item.SlotType];

                _player.Inventory.Add(unequippedItem, 1);
            }

            _equipment[(int)item.SlotType] = item;

            _player.CalculateBoostedStats();

            _player.SendEquipmentUpdate();
        }
    }
}
