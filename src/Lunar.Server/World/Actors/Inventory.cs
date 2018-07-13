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
using Lunar.Core;

namespace Lunar.Server.World.Actors
{
    public class Inventory
    {
        private readonly InventorySlot[] _inventory;
        private Player _player;

        public Inventory(Player player)
        {
            _player = player;
            _inventory = new InventorySlot[Settings.MaxInventoryItems];
        }

        public InventorySlot GetSlot(int slotNum)
        {
            return _inventory[slotNum];
        }

        public void SetSlot(int sloNum, InventorySlot value)
        {
            _inventory[sloNum] = value;
        }

        /// <summary>
        /// Removes the specified item and amount from player's inventory
        /// </summary>
        /// <param name="slotNum">Slot number of item to remove</param>
        /// <param name="amount">Amount of item to remove (default: 0 = all)</param>
        public void RemoveItem(int slotNum, int amount = -1)
        {
            if (slotNum >= 0 && slotNum < _inventory.Length)
            {
                // If -1 remove all items.
                if (amount == -1)
                {
                    _inventory[slotNum] = null;
                }
                else
                {
                    _inventory[slotNum].Amount -= amount;

                    if (_inventory[slotNum].Amount <= 0)
                    {
                        _inventory[slotNum] = null;
                    }
                }

                _player.SendInventoryUpdate();
            }
        }

        public void Remove(Item item, int amount = -1)
        {
            for (int i = 0; i < _inventory.Length; i++)
            {
                if (_inventory[i] != null && _inventory[i].Item == item)
                {
                    this.RemoveItem(i, amount);
                    return;
                }
            }
        }

        public void Add(Item item, int amount)
        {

            if (item.Stackable)
            {
                // Does the item exist within inventory already?
                foreach (var invSlot in _inventory)
                {
                    if (invSlot != null && invSlot.Item.GetType() == item.GetType())
                    {
                        invSlot.Amount += amount;

                        _player.SendInventoryUpdate();

                        return;
                    }
                }
            }

            for (int a = 0; a < amount; a++)
            {
                bool placedItem = false;

                for (int i = 0; i < Settings.MaxInventoryItems; i++)
                {
                    if (_inventory[i] == null)
                    {
                        placedItem = true;

                        _inventory[i] = new InventorySlot(item, 1);

                        _player.SendChatMessage(item.Name + " has been added to your inventory!", ChatMessageType.Announcement);

                        break;
                    }
                }

                // Is inventory full?
                if (!placedItem)
                {
                    _player.SendChatMessage("Your inventory is full!", ChatMessageType.Announcement);
                }
            }

            _player.SendInventoryUpdate();
        }
    }
}
