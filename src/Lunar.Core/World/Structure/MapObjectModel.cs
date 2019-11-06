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
    public class MapObjectModel
    {
        public Vector Position { get; set; }
        public SpriteInfo Sprite { get; set; }
        public bool Interactable { get; set; }
        public LayerModel<TileModel<SpriteInfo>> Layer { get; set; }
        public bool Animated { get; set; }
        public int FrameTime { get; set; }
        public LightInformation LightInformation { get; set; }
    }
}