using Lunar.Core.Utilities.Data;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class CollisionDescriptor
    {
        private Rect _collisionArea;

        public Rect CollisionArea { get { return _collisionArea; } set { _collisionArea = value; } }

        public CollisionDescriptor(Rect collisionArea)
        {
            _collisionArea = collisionArea;
        }

        public bool Collides(IActor actor)
        {
            Rect collisionArea = new Rect((int)(actor.Position.X + actor.CollisionBounds.Left), (int)(actor.Position.Y + actor.CollisionBounds.Top),
                actor.CollisionBounds.Width, actor.CollisionBounds.Height);

            return (_collisionArea.Intersects(collisionArea));
        }

        public bool Collides(Rect area)
        {
            return (_collisionArea.Intersects(area));
        }
    }
}