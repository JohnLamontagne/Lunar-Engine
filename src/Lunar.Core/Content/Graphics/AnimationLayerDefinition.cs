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
using System.IO;

namespace Lunar.Core.Content.Graphics
{
    public class AnimationLayerDefinition
    {
        /// <summary>
        /// The width of each frame.
        /// </summary>
        public int FrameWidth { get; set; }

        /// <summary>
        /// The height of each frame.
        /// </summary>
        public int FrameHeight { get; set; }

        /// <summary>
        /// The amount of time each frame will last on screen.
        /// </summary>
        public int FrameTime { get; set; }

        /// <summary>
        /// Whether the animation will reset & continue playing after completion.
        /// </summary>
        public int LoopCount { get; set; }

        /// <summary>
        /// The animation's sprite.
        /// </summary>
        public string TexturePath { get; set; }

        internal AnimationLayerDefinition()
        {

        }
    }
}
