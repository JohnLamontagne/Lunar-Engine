using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics.Effects
{
    public class AnimationLayer : IAnimationLayer<Sprite>
    {
        private double _nextFrameTime;
        private Rectangle _srcRect;

        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
        public int FrameTime { get; set; }
        public int LoopCount { get; set; }
        public string TexturePath { get => this.Sprite.TextureName; set { } }

        public Sprite Sprite { get; set; }

        public AnimationLayer()
        {
        }

        public AnimationLayer(IAnimationLayer<SpriteInfo> descriptor)
        {
            this.FrameWidth = descriptor.FrameWidth;
            this.FrameHeight = descriptor.FrameHeight;
            this.FrameTime = descriptor.FrameTime;
            this.LoopCount = descriptor.LoopCount;

            this.Sprite = new Sprite(Engine.Services.Get<ContentManagerService>().ContentManager
                .LoadTexture2D(Engine.ROOT_PATH + descriptor.TexturePath));
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(this.Sprite.Texture, position, _srcRect, Microsoft.Xna.Framework.Color.White);
        }

        public void Update(GameTime gameTime)
        {
            if (this.Sprite != null && _nextFrameTime <= gameTime.TotalGameTime.TotalMilliseconds)
            {
                int left = _srcRect.Left;
                int top = _srcRect.Top;

                if (left + this.FrameWidth >= this.Sprite.Texture.Width)
                {
                    if (_srcRect.Top + this.FrameHeight >= this.Sprite.Texture.Height)
                    {
                        left = 0;
                        top = 0;
                    }
                    else
                    {
                        top = _srcRect.Top + this.FrameHeight;
                        left = 0;
                    }
                }
                else
                    left = _srcRect.Left + this.FrameWidth;

                _srcRect = new Rectangle(left, top, this.FrameWidth, this.FrameHeight);

                _nextFrameTime = gameTime.TotalGameTime.TotalMilliseconds + this.FrameTime;
            }
            else
            {
                _nextFrameTime = 0;
                _srcRect = new Rectangle(0, 0, this.FrameWidth, this.FrameHeight);
            }
        }
    }
}