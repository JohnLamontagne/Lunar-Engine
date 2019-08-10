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
            int left = this.Transform.Rect.X;
            int top = this.Transform.Rect.Y;
            if (this.Transform.Rect.X + this.Transform.Rect.Width > this.Texture.Width - this.Transform.Rect.Width)
            {
                if (this.Transform.Rect.Y + this.Transform.Rect.Height > this.Texture.Height - this.Transform.Rect.Height)
                {
                    left = 0;
                    top = 0;
                }
                else
                {
                    top = this.Transform.Rect.Y + this.Transform.Rect.Height;
                    left = 0;
                }
            }
            else
                left = this.Transform.Rect.X + this.Transform.Rect.Width;

            this.Transform.Rect = this.Transform.Rect.MoveTo(left, top);
        }
    }
}