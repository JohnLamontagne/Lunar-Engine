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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Lunar.Server.Content.Graphics
{
    public class SpriteSheet
    {
        public Sprite Sprite { get; set; }

        public int HorizontalFrames { get; set; }

        public int VerticalFrames { get; set; }

        public int FrameWidth { get; set; }

        public int FrameHeight { get; set; }

        public SpriteSheet(Sprite sprite, int horizontalFrames, int verticalFrames, int frameWidth, int frameHeight)
        {
            this.Sprite = sprite;
            this.HorizontalFrames = horizontalFrames;
            this.VerticalFrames = verticalFrames;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
        }

        public NetBuffer Pack()
        {
            var netBuffer = new NetBuffer();
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write(this.HorizontalFrames);
            netBuffer.Write(this.VerticalFrames);
            netBuffer.Write(this.FrameWidth);
            netBuffer.Write(this.FrameHeight);
            return netBuffer;
        }
    }
}
