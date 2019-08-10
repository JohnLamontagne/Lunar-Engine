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

namespace Lunar.Core.World.Structure
{
    public class BaseTile<T> : IBaseTile<T> where T : SpriteInfo
    {
        public Vector Position { get; set; }

        public bool Animated { get; set; }

        public int FrameCount { get; set; }

        public bool LightSource { get; set; }

        public int LightRadius { get; set; }

        public Color LightColor { get; set; }

        public bool Teleporter { get; set; }

        public bool Blocked { get; set; }

        public Attribute.TileAttribute Attribute { get; set; }

        public virtual T Sprite { get; set; }

        protected BaseTile()
        {
            this.Attribute = null;
        }

        public BaseTile(T sprite)
            : this()
        {
            this.Sprite = sprite;

            this.Animated = false;
        }

        public BaseTile(Vector position)
            : this()
        {
            this.Position = position;
        }
    }
}