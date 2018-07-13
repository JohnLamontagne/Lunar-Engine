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
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics
{
    public static class SpriteBatchExtensions
    {
        private static Texture2D _texture;

        public static void Initalize(GraphicsDevice graphicsDevice)
        {
            _texture = new Texture2D(graphicsDevice, 1, 1);
            _texture.SetData(new[] { Color.White });
        }

        public static void DrawWireFrameBox(this SpriteBatch spriteBatch, Rectangle area, Color color, int borderWidth)
        {
            spriteBatch.Draw(_texture, new Rectangle(area.Left, area.Top, borderWidth, area.Height), color); // Left
            spriteBatch.Draw(_texture, new Rectangle(area.Right, area.Top, borderWidth, area.Height), color); // Right
            spriteBatch.Draw(_texture, new Rectangle(area.Left, area.Top, area.Width, borderWidth), color); // Top
            spriteBatch.Draw(_texture, new Rectangle(area.Left, area.Bottom, area.Width, borderWidth), color); // Bottom
        }

        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.SourceRectangle, sprite.Color, sprite.Rotation, Vector2.Zero, sprite.Scale, sprite.Effects, sprite.LayerDepth);
        }
    }
}
