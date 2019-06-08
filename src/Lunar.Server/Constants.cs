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
#define DEV_MODE

using System;
using Lunar.Core;

namespace Lunar.Server
{
    public static class Constants
    {
        public const int MAP_ITEM_WIDTH = 32;

        public const int MAP_ITEM_HEIGHT = 32;

        public const int MAX_QUEUED_ACTIONS = 10;

        public const int ACTIONS_PER_SECOND = 5;


#if DEV_MODE
        public static readonly string FILEPATH_DATA = AppDomain.CurrentDomain.BaseDirectory + "../../Server Data/";
#else
        public static readonly string FILEPATH_DATA = AppDomain.CurrentDomain.BaseDirectory + "/Server Data/";
#endif

        public static readonly string FILEPATH_WORLD = Constants.FILEPATH_DATA + "/World/";

        /// <summary>
        /// Location of the accounts directory (if using file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_ACCOUNTS = Constants.FILEPATH_WORLD + "/Accounts/";

        /// <summary>
        /// Location of the NPCs directory (if using the file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_NPCS = Constants.FILEPATH_WORLD + "/Npcs/";

        public static readonly string FILEPATH_LOGS = Constants.FILEPATH_WORLD + "/Logs/";

        /// <summary>
        /// Location of the items directory (if using the file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_ITEMS = Constants.FILEPATH_WORLD + "/Items/";

        /// <summary>
        /// Location of the mapss directory (if using the file-system storage). SERVER ONLY
        /// </summary>
        public static readonly string FILEPATH_MAPS = Constants.FILEPATH_WORLD + "/Maps/";

        public static readonly string FILEPATH_SCRIPTS = Constants.FILEPATH_WORLD + "/Scripts/";

        public static readonly string FILEPATH_PLUGINS = Constants.FILEPATH_WORLD + "/Plugins/";

        public static readonly string FILEPATH_ANIMATIONS = Constants.FILEPATH_WORLD + "/Animations/";
    }
}