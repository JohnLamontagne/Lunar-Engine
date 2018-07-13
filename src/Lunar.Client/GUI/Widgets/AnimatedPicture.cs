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

namespace Lunar.Client.GUI.Widgets
{
    public class AnimatedPicture : Picture
    {
        private int _frameTime;
        private Vector2 _frameSize;
        private Rectangle _srcRect;
        private double _nextFrameTime;
        private float _rotation;

        public int ZOrder { get; set; }

        public float FrameRotation { get; set; }

        public int FrameTime
        {
            get => _frameTime;
            set => _frameTime = value;
        }

        public AnimatedPicture(Texture2D sprite, int frameTime, Vector2 frameSize)
            : base(sprite)
        {
            this.FrameTime = frameTime;
            _frameSize = frameSize;

            _srcRect = new Rectangle(0, 0, (int)frameSize.X, (int)frameSize.Y);

            this.FrameRotation = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            // Check whether this is the first frame.
            if (_nextFrameTime <= 0)
            {
                _nextFrameTime = gameTime.TotalGameTime.TotalMilliseconds + _frameTime;
                return;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds >= _nextFrameTime)
            {
                int left = _srcRect.Left;
                int top = _srcRect.Top;

                if (_srcRect.Left >= (this.Sprite.Width - _frameSize.X))
                {
                    left = 0;

                    if (_srcRect.Top < (this.Sprite.Height - _frameSize.Y))
                    {
                        top = (int)(_srcRect.Top + _frameSize.Y);
                    }
                    else
                    {
                        top = 0;
                    }
                }
                else
                    left = (int)(_srcRect.Left + _frameSize.X);

                _srcRect = new Rectangle(left, top, _srcRect.Width, _srcRect.Height);

                _nextFrameTime = gameTime.TotalGameTime.TotalMilliseconds + _frameTime;

                _rotation += this.FrameRotation;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            spriteBatch.Draw(this.Sprite, this.Position, _srcRect, Color.White, _rotation, new Vector2(_srcRect.Width / 2f, _srcRect.Height / 2f), 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);
        }
    }
}
