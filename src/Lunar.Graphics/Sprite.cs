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
using Color = Lunar.Core.Content.Graphics.Color;

namespace Lunar.Graphics
{
    public class Sprite : SpriteInfo
    {
        private Texture2D _texture;
        private SpriteEffects _spriteEffects;

        public Texture2D Texture
        {
            get => _texture;
            set => _texture = value;
        }

        public SpriteEffects Effects
        {
            get => _spriteEffects;
            set => _spriteEffects = value;
        }

        public Sprite(Texture2D texture)
            : base(texture.Name)
        {
            _texture = texture;
            this.Effects = SpriteEffects.None;

            this.Transform.Color = Microsoft.Xna.Framework.Color.White;
            this.Transform.Rect = this.Texture.Bounds;
            this.Transform.Position = Vector2.Zero;
            this.Transform.Rotation = 0f;
            this.Transform.Scale = 1f;
            this.Transform.LayerDepth = 0f;
        }
    }
}