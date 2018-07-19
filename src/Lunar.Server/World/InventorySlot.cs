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
using Lunar.Server.World;

namespace Lunar.Server.World
{
    public class InventorySlot
    {
        private Item _item;
        private int _amount;

        public Item Item { get { return _item; } }

        public int Amount { get { return _amount; } set { _amount = value; } }

        public InventorySlot(Item item, int amount)
        {
            _item = item;
            this.Amount = amount;
        }
    }
}
