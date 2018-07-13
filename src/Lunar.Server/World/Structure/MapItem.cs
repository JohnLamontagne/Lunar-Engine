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
using Lunar.Core.Utilities.Data;
using Lunar.Core.World;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class MapItem
    {
        private readonly Item _item;
        private CollisionDescriptor _collisionDescriptor;
        private Vector _position;
        private Layer _layer;

        public Item Item { get { return _item; } }

        public int Amount { get; set; }

        public Vector Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                _collisionDescriptor = new CollisionDescriptor(new Rect((int)_position.X, (int)_position.Y, Constants.MAP_ITEM_WIDTH, Constants.MAP_ITEM_HEIGHT));
            }
        }

        public Layer Layer { get => _layer; set => _layer = value; }

        public MapItem(Item item, int amount)
        {
            _item = item;
            this.Amount = amount;
        }

        public bool WithinReachOf(IActor actor)
        {
            return (_collisionDescriptor.Collides(actor));
        }
    }
}
