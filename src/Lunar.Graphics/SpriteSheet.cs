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
    public class SpriteSheet
    {
        private readonly Sprite _sprite;
        private Rectangle _frameRectangle;
        private Rectangle _destionationRect;
        private int _horizontalFrameIndex;
        private int _verticalFrameIndex;
        private readonly int _verticalFrames;
        private readonly int _horizontalFrames;
        private readonly int _frameWidth;
        private readonly int _frameHeight;

        public Sprite Sprite => _sprite;

        public Vector2 Position
        {
            get => new Vector2(_destionationRect.X, _destionationRect.Y);
            set => _destionationRect = new Rectangle((int)value.X, (int)value.Y, _frameWidth, _frameHeight);
        }

        public Vector2 FrameSize => new Vector2(_frameWidth, _frameHeight);

        public int HorizontalFrameIndex
        {
            get => _horizontalFrameIndex;
            set
            {
                if ((value < 0) || value >= _horizontalFrames) value = 0;

                _horizontalFrameIndex = value;

                _frameRectangle.X = _horizontalFrameIndex * _frameWidth;
            }
        }

        public int VerticalFrameIndex
        {
            get => _verticalFrameIndex;
            set
            {
                if ((value < 0) || value >= _verticalFrames) value = 0;

                _verticalFrameIndex = value;

                _frameRectangle.Y = _verticalFrameIndex * _frameHeight;
            }
        }

        public SpriteSheet(Sprite sprite, int horizontalFrames, int verticalFrames, int frameWidth, int frameHeight)
        {
            _sprite = sprite;

            _frameRectangle = new Rectangle(0, 0, frameWidth, frameHeight);
            _frameWidth = frameWidth;
            _frameHeight = frameHeight;
            _horizontalFrames = horizontalFrames;
            _verticalFrames = verticalFrames;

            this.Position = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Sprite.Texture, _destionationRect, _frameRectangle, this.Sprite.Color, this.Sprite.Rotation, Vector2.Zero, this.Sprite.Effects, this.Sprite.LayerDepth);
        }
    }
}
