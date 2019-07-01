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
        private IActor<IActorDescriptor> _actor;

        public Rect CollisionArea
        {
            get
            {
                if (_actor?.Descriptor?.CollisionBounds != null)
                {
                    return new Rect(_actor.Descriptor.Position.X + _actor.Descriptor.CollisionBounds.Left, _actor.Descriptor.Position.Y + _actor.Descriptor.CollisionBounds.Top,
                        _actor.Descriptor.CollisionBounds.Width, _actor.Descriptor.CollisionBounds.Height);
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

        public CollisionBody(IActor<IActorDescriptor> actor)
        {
            _actor = actor;
        }

        public bool Collides(CollisionBody collisionBody)
        {
            return (this.CollisionArea.Intersects(collisionBody.CollisionArea));
        }

        public bool Collides(IActor<IActorDescriptor> actor)
        {
            return (this.CollisionArea.Intersects(actor.CollisionBody.CollisionArea));
        }

        public bool Collides(Rect area)
        {
            return (this.CollisionArea.Intersects(area));
        }
    }
}