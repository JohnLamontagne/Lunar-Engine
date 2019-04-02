/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

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
