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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server.World.Actors.Actions.Player
{
    class PlayerDropItemAction : IAction<Actors.Player>
    {
        private readonly int _slotNum;

        public PlayerDropItemAction(int slotNum)
        {
            _slotNum = slotNum;
        }

        public void Execute(Actors.Player player)
        {
            if (player.Inventory.GetSlot(_slotNum) != null)
            {
                player.Map.SpawnItem(player.Inventory.GetSlot(_slotNum).Item, player.Descriptor.Position, player.Layer);
                player.Inventory.RemoveItem(_slotNum, 1);
            }
        }
    }
}
