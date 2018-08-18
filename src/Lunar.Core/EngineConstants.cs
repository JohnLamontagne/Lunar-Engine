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
namespace Lunar.Core
{
    public static class EngineConstants
    {
        /// <summary>
        /// Defines the degree to which the ZIndex increases for successive layers.
        /// </summary>
        public const float PARTS_PER_LAYER = .0001f;

        public const string ITEM_FILE_EXT = ".idat";

        public const string MAP_FILE_EXT = ".mdat";

        public const string ANIM_FILE_EXT = ".adat";

        public const string LUA_FILE_EXT = ".lua";

        public const string NPC_FILE_EXT = ".ndat";

        public const int TILE_WIDTH = 32;

        public const int TILE_HEIGHT = 32;
    }
}
