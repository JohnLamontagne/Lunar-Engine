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
