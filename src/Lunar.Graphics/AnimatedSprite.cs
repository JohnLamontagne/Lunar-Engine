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
