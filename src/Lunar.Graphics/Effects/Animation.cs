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
using Lunar.Core.Content.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics.Effects
{
    public abstract class Animation
    {
        public Sprite SurfaceSprite { get; set; }
        public Sprite SubSurfaceSprite { get; set; }

        private bool _animationPlaying;
        private bool _paused;
        private double _nextSurfaceFrameTime;
        private double _nextSubSurfaceFrameTime;

        private Rectangle _surfaceFrameRectangle;
        private Rectangle _subSurfaceFrameRectangle;
        private Vector2 _position;

        private AnimationDescription _definition;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        protected Animation(AnimationDescription description)
        {
            _definition = description;
            _nextSurfaceFrameTime = 0;
            _nextSubSurfaceFrameTime = 0;
            _surfaceFrameRectangle = new Rectangle();
            _subSurfaceFrameRectangle = new Rectangle();
        }

        public virtual void Play()
        {
            _animationPlaying = true;
            _paused = false;
        }

        public virtual void Stop()
        {
            _animationPlaying = false;
            _paused = false;
        }

        public virtual void Pause()
        {   
            _paused = true;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_paused)
                return;

            if (_animationPlaying)
            {
                if (this.SubSurfaceSprite != null)
                {
                    spriteBatch.Draw(this.SubSurfaceSprite.Texture, this.Position, _subSurfaceFrameRectangle, Microsoft.Xna.Framework.Color.White);
                }

                if (this.SurfaceSprite != null)
                {
                    spriteBatch.Draw(this.SurfaceSprite.Texture, this.Position, _surfaceFrameRectangle, Microsoft.Xna.Framework.Color.White);
                }
            }
        }

        public virtual void Update(GameTime gameTime)
        {
            if (_paused)
                return;

            if (_animationPlaying)
            {
                if (this.SubSurfaceSprite != null && _nextSubSurfaceFrameTime <= gameTime.TotalGameTime.TotalMilliseconds)
                {
                    int left = _subSurfaceFrameRectangle.Left;
                    int top = _subSurfaceFrameRectangle.Top;

                    if (left + _definition.SubSurfaceAnimation.FrameWidth >= this.SubSurfaceSprite.Texture.Width)
                    {
                        if (_subSurfaceFrameRectangle.Top + _definition.SubSurfaceAnimation.FrameHeight >= this.SubSurfaceSprite.Texture.Height)
                        {
                            left = 0;
                            top = 0;
                        }
                        else
                        {
                            top = _subSurfaceFrameRectangle.Top + _definition.SubSurfaceAnimation.FrameHeight;
                            left = 0;
                        }
                    }
                    else
                        left = _subSurfaceFrameRectangle.Left + _definition.SubSurfaceAnimation.FrameWidth;

                    _subSurfaceFrameRectangle = new Rectangle(left, top, _definition.SubSurfaceAnimation.FrameWidth, _definition.SubSurfaceAnimation.FrameHeight);

                    _nextSubSurfaceFrameTime = gameTime.TotalGameTime.TotalMilliseconds + _definition.SubSurfaceAnimation.FrameTime;
                }

                if (this.SurfaceSprite != null &&  _nextSurfaceFrameTime <= gameTime.TotalGameTime.TotalMilliseconds)
                {
                    int left = _surfaceFrameRectangle.Left;
                    int top = _surfaceFrameRectangle.Top;

                    if (left + _definition.SurfaceAnimation.FrameWidth >= this.SurfaceSprite.Texture.Width)
                    {
                        if (_surfaceFrameRectangle.Top + _definition.SurfaceAnimation.FrameHeight >= this.SurfaceSprite.Texture.Height)
                        {
                            left = 0;
                            top = 0;
                        }
                        else
                        {
                            top = _surfaceFrameRectangle.Top + _definition.SurfaceAnimation.FrameHeight;
                            left = 0;
                        }
                    }
                    else
                        left = _surfaceFrameRectangle.Left + _definition.SurfaceAnimation.FrameWidth;

                    _surfaceFrameRectangle = new Rectangle(left, top, _definition.SurfaceAnimation.FrameWidth, _definition.SurfaceAnimation.FrameHeight);

                    _nextSurfaceFrameTime = gameTime.TotalGameTime.TotalMilliseconds + _definition.SurfaceAnimation.FrameTime;
                }
            }
            else
            {
                _nextSurfaceFrameTime = 0;
                _subSurfaceFrameRectangle = new Rectangle(0, 0, _definition.SubSurfaceAnimation.FrameWidth, _definition.SubSurfaceAnimation.FrameHeight);
                _surfaceFrameRectangle = new Rectangle(0, 0, _definition.SurfaceAnimation.FrameWidth, _definition.SurfaceAnimation.FrameHeight);
            }
        }
    }
}
