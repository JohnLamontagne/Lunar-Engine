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
