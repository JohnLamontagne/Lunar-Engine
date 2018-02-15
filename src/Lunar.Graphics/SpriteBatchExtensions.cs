using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics
{
    public static class SpriteBatchExtensions
    {
        public static void Draw(this SpriteBatch spriteBatch, Sprite sprite)
        {
            spriteBatch.Draw(sprite.Texture, sprite.Position, sprite.SourceRectangle, sprite.Color, sprite.Rotation, Vector2.Zero, sprite.Scale, sprite.Effects, sprite.LayerDepth);
        }
    }
}
