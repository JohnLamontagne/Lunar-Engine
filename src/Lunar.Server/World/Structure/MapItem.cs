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
