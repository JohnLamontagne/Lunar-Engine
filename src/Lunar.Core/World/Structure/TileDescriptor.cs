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
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure.TileAttribute;

namespace Lunar.Core.World.Structure
{
    public class TileDescriptor
    {
        public Vector Position { get; set; }

        public bool Animated { get; set; }

        public int FrameCount { get; set; }

        public bool LightSource { get; set; }

        public int LightRadius { get; set; }

        public Color LightColor { get; set; }

        public bool Teleporter { get; set; }

        public bool Blocked { get; set; }

        public TileAttributes Attribute { get; set; }

        public AttributeData AttributeData { get; set; }

        public SpriteInfo SpriteInfo { get; set; }

        private TileDescriptor()
        {
            this.AttributeData = new AttributeData();
        }

        public TileDescriptor(SpriteInfo sprite)
            : this()
        {
            this.SpriteInfo = sprite;

            this.Animated = false;
        }

        public TileDescriptor(Vector position)
            : this()
        {
            this.Position = position;
        }
    }
}