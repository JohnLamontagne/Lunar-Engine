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
using System.IO;
using Lidgren.Network;
using Lunar.Client.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using Lunar.Client.Utilities.Services;
using Lunar.Core;
using Lunar.Core.World;

namespace Lunar.Client.World
{
    public class Tile
    {
        private long _nextAnimationTime;
        private Vector2 _position;
        private Texture2D _sprite;
        private Rectangle _sourceRectangle;

        public bool Animated { get; set; }

        public bool LightSource { get; set; }

        public bool Teleporter { get; set; }

        public int PlayerDamage { get; set; }

        public int FrameCount { get; set; }

        public Color Color { get; set; }

        public float ZIndex { get; set; }

        public Tile(Texture2D sprite, Rectangle sourceRectangle, Vector2 position)
        {
            _sprite = sprite;
            _sourceRectangle = sourceRectangle;
            _position = position;

            this.Animated = false;
        }

        public Tile()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_sprite != null)
                spriteBatch.Draw(_sprite, _position, _sourceRectangle, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, this.ZIndex);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds >= _nextAnimationTime && this.Animated)
            {

                _nextAnimationTime = (long)gameTime.TotalGameTime.TotalMilliseconds + 300;
            }
        }

        public static Tile Unpack(NetBuffer netBuffer)
        {
            // check for a null tile
            if (netBuffer.ReadBoolean() == false)
            {
                return null;
            }


                // Check for blank tile?
            if (netBuffer.ReadBoolean())
            {
                return new Tile();
            }
            else
            {
                var animated = netBuffer.ReadBoolean();
                var lightSource = netBuffer.ReadBoolean();
                var lightRadius = netBuffer.ReadInt32();
                var lightColor = new Color(new Vector4(netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte(),
                    netBuffer.ReadByte()));
                var teleporter = netBuffer.ReadBoolean();

                var tilesetPath = Constants.FILEPATH_DATA + netBuffer.ReadString();

                var sprite = Client.ServiceLocator.GetService<ContentManagerService>().ContentManager
                    .LoadTexture2D(tilesetPath);

                Color color = new Color(new Vector4(netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte(),
                    netBuffer.ReadByte()));
                Rectangle sourceRectangle = new Rectangle(netBuffer.ReadInt32(), netBuffer.ReadInt32(),
                    netBuffer.ReadInt32(), netBuffer.ReadInt32());
                Vector2 position = new Vector2(netBuffer.ReadFloat(), netBuffer.ReadFloat());

                var frameCount = netBuffer.ReadInt32();

                var tile = new Tile(sprite, sourceRectangle, position)
                {
                    Animated = animated,
                    LightSource = lightSource,
                    Teleporter = teleporter,
                    Color = color,
                    FrameCount = frameCount,

                };

                if (lightSource)
                {
                    PointLight pointLight = new PointLight();
                    pointLight.Color = lightColor;
                    pointLight.Radius = lightRadius;
                    pointLight.Position = new Vector2(position.X - (lightRadius / 2f) + (EngineConstants.TILE_WIDTH / 2f),
                        position.Y - (lightRadius / 2f) + (EngineConstants.TILE_HEIGHT / 2f));
                    Client.ServiceLocator.GetService<LightManagerService>().Component.Lights.Add(pointLight);
                }

                return tile;
            }
        }
    }
}