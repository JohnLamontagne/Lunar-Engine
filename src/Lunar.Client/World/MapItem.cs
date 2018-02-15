using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.Utilities.Services;
using Lunar.Client.World.Actors;
using Lunar.Core.World;
using Lunar.Graphics;

namespace Lunar.Client.World
{
    public class MapItem
    {
        private CollisionDescriptor _collisionDescriptor;
        private string _name;
        private Sprite _sprite;
        
        public int Amount { get; set; }

        public string Name => _name;

        public Sprite Sprite => _sprite;

        public Vector2 Position => _sprite.Position;

        public MapItem()
        {
        }

        public void Unpack(NetBuffer netBuffer, Layer layer)
        {
            var position = new Vector2(netBuffer.ReadFloat(), netBuffer.ReadFloat());
            _name = netBuffer.ReadString();

            var textureName = netBuffer.ReadString();
            _sprite = new Sprite(Client.ServiceLocator.GetService<ContentManagerService>()
                .ContentManager.Load<Texture2D>(Constants.FILEPATH_GFX + "/Items/" + textureName))
            {
                Position = position,
                LayerDepth = layer.ZIndex + .001f, // the .001f makes it so that the item spawns above the map layer, but just below actors
            };

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }

        public bool WithinReachOf(IActor actor)
        {
            return (_collisionDescriptor.Collides(actor));
        }
    }
}
