/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
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
            _equipment = new Item[Enum.GetNames(typeof(EquipmentSlots)).Length];
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
