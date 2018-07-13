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
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class CollisionDescriptor
    {
        private Rect _collisionArea;

        public Rect CollisionArea
        {
            get => _collisionArea;
            set => _collisionArea = value;
        }

        public CollisionDescriptor(Rect collisionArea)
        {
            _collisionArea = collisionArea;
        }

        public bool Collides(IActor actor)
        {
            Rect collisionArea = new Rect((int)(actor.Position.X + actor.CollisionBounds.Left), (int)(actor.Position.Y + actor.CollisionBounds.Top),
                (actor.Position.X + actor.CollisionBounds.Left) + actor.CollisionBounds.Width, (actor.Position.Y + actor.CollisionBounds.Top) + actor.CollisionBounds.Height);

            return (_collisionArea.Intersects(collisionArea));
        }

        public bool Collides(Rect area)
        {
            return (_collisionArea.Intersects(area));
        }
    }
}