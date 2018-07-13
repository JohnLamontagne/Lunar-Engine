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
    public class Sprite
    {
        private Texture2D _texture;
        private Rectangle _srcRectangle;
        private Vector2 _position;
        private Color _color;
        private float _rotation;
        private float _scale;
        private SpriteEffects _spriteEffects;
        private float _layerDepth;

        public Texture2D Texture
        {
            get => _texture;
            set => _texture = value;
        }

        public Rectangle SourceRectangle
        {
            get =>_srcRectangle;
            set => _srcRectangle = value;
        }

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public Color Color
        {
            get => _color;
            set => _color = value;
        }

        public float Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }

        public float Scale
        {
            get => _scale;
            set => _scale = value;
        }

        public SpriteEffects Effects
        {
            get => _spriteEffects;
            set => _spriteEffects = value;
        }

        public float LayerDepth
        {
            get => _layerDepth;
            set => _layerDepth = value;
        }

        public Sprite(Texture2D texture)
        {
            _texture = texture;

            this.Color = Color.White;
            this.SourceRectangle = this.Texture.Bounds;
            this.Position = Vector2.Zero;
            this.Effects = SpriteEffects.None;
            this.Rotation = 0f;
            this.Scale = 1f;
            this.LayerDepth = 0f;
        }

    }
}
