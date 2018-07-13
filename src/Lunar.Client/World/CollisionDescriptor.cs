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
using Microsoft.Xna.Framework;
using Lunar.Client.World.Actors;
using Lunar.Core.World;

namespace Lunar.Client.World
{
    public class CollisionDescriptor
    {
        private Rectangle _collisionArea;

        public Rectangle CollisionArea { get { return _collisionArea; } set { _collisionArea = value; } }

        public CollisionDescriptor(Rectangle collisionArea)
        {
            _collisionArea = collisionArea;
        }

        public bool Collides(IActor entity)
        {
            Rectangle collisionArea = new Rectangle((int)(entity.Position.X + entity.CollisionBounds.Left), (int)(entity.Position.Y + entity.CollisionBounds.Top),
               entity.CollisionBounds.Width, entity.CollisionBounds.Height);

            return (_collisionArea.Intersects(collisionArea));
        }

        public bool Collides(Rectangle area)
        {
            return (_collisionArea.Intersects(area));
        }
    }
}
