using System.IO;
using Lidgren.Network;
using Lunar.Client.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using Lunar.Client.Utilities.Services;
using Lunar.Graphics;

namespace Lunar.Client.World
{
    public class MapObject
    {
        private readonly Sprite _sprite;
        private bool _animated;
        private long _nextAnimationTime;

        public PointLight Light { get; set; }

        public Sprite Sprite => _sprite;

        public bool Animated { get { return _animated; } }

        public int FrameTime { get; set; }

        public MapObject(Sprite sprite, bool animated = false)
        {
            _sprite = sprite;
            _animated = animated;
        }

        public void Update(GameTime gameTime)
        {
            if (this.Animated)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds >= _nextAnimationTime)
                {
                    (this.Sprite as AnimatedSprite)?.Next();

                    _nextAnimationTime = (long)gameTime.TotalGameTime.TotalMilliseconds + this.FrameTime;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Sprite);
        }

        public static MapObject Unpack(NetBuffer netBuffer)
        {
            // can we view it?
            if (!netBuffer.ReadBoolean())
                return null;

            var textureName = Path.ChangeExtension(netBuffer.ReadString(), null);
            var sourceRectangle = new Rectangle(netBuffer.ReadInt32(), netBuffer.ReadInt32(), netBuffer.ReadInt32(), netBuffer.ReadInt32());
            var color = new Color(new Vector4(netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte()));
            var position = new Vector2(netBuffer.ReadFloat(), netBuffer.ReadFloat());
            var animated = netBuffer.ReadBoolean();
            float zIndex = netBuffer.ReadSingle();
            var frameTime = netBuffer.ReadInt32();

            var sprite =
                new AnimatedSprite(Client.ServiceLocator.GetService<ContentManagerService>().ContentManager.LoadTexture2D(
                    Constants.FILEPATH_ROOT + textureName))
                {
                    SourceRectangle = sourceRectangle,
                    Color = color,
                    Position = position,
                    LayerDepth = zIndex
                };

            var mapObject = new MapObject(sprite, animated) {FrameTime = frameTime};


            // is it a light source?
            if (netBuffer.ReadBoolean() == true)
            {
                var lightColor = new Color(new Vector4(netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte()));
                var radius = netBuffer.ReadFloat();

                PointLight pointLight = new PointLight
                {
                    Position = position,
                    Scale = new Vector2(radius),
                    Color = lightColor,
                    Intensity = .7f,
                };

                Client.ServiceLocator.GetService<LightManagerService>().Component.Lights.Add(pointLight);

                mapObject.Light = pointLight;
            }

            return mapObject;
        }
    }
}
