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

        public const string ACC_FILE_EXT = ".acc";

        public const int TILE_WIDTH = 32;

        public const int TILE_HEIGHT = 32;

        public static readonly string FILEPATH_DATA = AppDomain.CurrentDomain.BaseDirectory + "/Data/";

        /// <summary>
        /// Location of the accounts directory (if using file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_ACCOUNTS = EngineConstants.FILEPATH_DATA + "/Accounts/";

        public static readonly string FILEPATH_SCRIPTS = EngineConstants.FILEPATH_DATA + "/Scripts/";

        public static readonly string FILEPATH_PLUGINS = EngineConstants.FILEPATH_DATA + "/Plugins/";

        /// <summary>
        /// Location of the NPCs directory (if using the file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_NPCS = EngineConstants.FILEPATH_DATA + "/Npcs/";

        public static readonly string FILEPATH_LOGS = EngineConstants.FILEPATH_DATA + "/Logs/";

        /// <summary>
        /// Location of the items directory (if using the file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_ITEMS = EngineConstants.FILEPATH_DATA + "/Items/";

        /// <summary>
        /// Location of the mapss directory (if using the file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_MAPS = EngineConstants.FILEPATH_DATA + "/Maps/";
    }
}
