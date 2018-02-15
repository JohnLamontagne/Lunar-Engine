using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics
{
    public class AnimatedSprite : Sprite
    {

        public AnimatedSprite(Texture2D texture) : base(texture)
        {
        }

        public void Next()
        {
            int left = this.SourceRectangle.Left;
            int top = this.SourceRectangle.Top;
            if (this.SourceRectangle.Left + this.SourceRectangle.Width > this.Texture.Width - this.SourceRectangle.Width)
            {
                if (this.SourceRectangle.Top + this.SourceRectangle.Height > this.Texture.Height - this.SourceRectangle.Height)
                {
                    left = 0;
                    top = 0;
                }
                else
                {
                    top = this.SourceRectangle.Top + this.SourceRectangle.Height;
                    left = 0;
                }
            }
            else
                left = this.SourceRectangle.Left + this.SourceRectangle.Width;

            this.SourceRectangle = new Rectangle(left, top, this.SourceRectangle.Width, this.SourceRectangle.Height);
        }
    }
}
