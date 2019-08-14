/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

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
    public class Animation : BaseAnimation<AnimationLayer>
    {
        private bool _animationPlaying;
        private bool _paused;

        private Vector2 _position;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Animation(BaseAnimation<IAnimationLayer<SpriteInfo>> description)
            : base(description.Name)
        {
            if (description.SurfaceAnimation != null)
                this.SurfaceAnimation = new AnimationLayer(description.SurfaceAnimation);

            if (description.SubSurfaceAnimation != null)
                this.SubSurfaceAnimation = new AnimationLayer(description.SubSurfaceAnimation);
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

        public virtual void DrawSubSurface(SpriteBatch spriteBatch)
        {
            if (_paused)
                return;

            if (_animationPlaying)
            {
                this.SubSurfaceAnimation?.Draw(spriteBatch, this.Position);
            }
        }

        public virtual void DrawSurface(SpriteBatch spriteBatch)
        {
            if (_paused)
                return;

            if (_animationPlaying)
            {
                this.SurfaceAnimation?.Draw(spriteBatch, this.Position);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_paused)
                return;

            if (_animationPlaying)
            {
                this.SubSurfaceAnimation?.Update(gameTime);
                this.SurfaceAnimation?.Update(gameTime);
            }
        }

        public new static Animation Create()
        {
            var animation = new Animation(BaseAnimation<IAnimationLayer<SpriteInfo>>.Create());
            animation.SubSurfaceAnimation = new AnimationLayer()
            animation.SubSurfaceAnimation.TexturePath = "";
            animation.SurfaceAnimation.TexturePath = "";

            return animation;
        }
    }
}