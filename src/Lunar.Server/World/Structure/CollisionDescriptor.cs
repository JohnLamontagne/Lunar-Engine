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

using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class CollisionBody
    {
        private Rect _collisionArea;
        private IActor _actor;

        public Rect CollisionArea
        {
            get
            {
                if (_actor?.CollisionBounds != null)
                {
                    return new Rect(_actor.Position.X + _actor.CollisionBounds.X, _actor.Position.Y + _actor.CollisionBounds.Y,
                        _actor.CollisionBounds.Width, _actor.CollisionBounds.Height);
                }
                else
                {
                    return _collisionArea;
                }
            }
        }

        public CollisionBody(Rect collisionArea)
        {
            _collisionArea = collisionArea;
        }

        public CollisionBody(IActor actor)
        {
            _actor = actor;
        }

        public bool Collides(CollisionBody collisionBody)
        {
            return (this.CollisionArea.Intersects(collisionBody.CollisionArea));
        }

        public bool Collides(IActor actor)
        {
            return (this.CollisionArea.Intersects(actor.CollisionBody.CollisionArea));
        }

        public bool Collides(CollisionBody collisionBody, Vector reach)
        {
            Rect reachArea = new Rect(this.CollisionArea.X, this.CollisionArea.Y, this.CollisionArea.Width + reach.X, this.CollisionArea.Height + reach.Y);

            return (reachArea.Intersects(collisionBody.CollisionArea));
        }

        public bool Collides(IActor actor, Vector reach)
        {
            Rect reachArea = new Rect(this.CollisionArea.X, this.CollisionArea.Y, this.CollisionArea.Width + reach.X, this.CollisionArea.Height + reach.Y);

            return (reachArea.Intersects(actor.CollisionBody.CollisionArea));
        }

        public bool Collides(Rect area)
        {
            return (this.CollisionArea.Intersects(area));
        }
    }
}