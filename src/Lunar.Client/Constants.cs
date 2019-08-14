
#define DEV_MODE


using Lunar.Core;
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
namespace Lunar.Client
{
    public static class Constants
    {
        public const int CLICK_TIMER_DELAY = 500;

        public const int MAX_INVENTORY = 30;

        public const int INV_SLOTS_PER_ROW = 5;

        public const int INVENTORY_OFFSET_X = 40;
        public const int INVENTORY_OFFSET_Y = 50;

        public const int INV_SLOT_OFFSET = 68;

        public const int DIALOGUE_SEP_X = 20;

        /// <summary>
        /// Minimum amount of time that the loading screen should display
        /// </summary>
        public const long MIN_LOAD_TIME = 1000;

        public static readonly string FILEPATH_DATA;
        public static readonly string FILEPATH_PLUGINS;
        public static readonly string FILEPATH_SHADERS;
        public static readonly string FILEPATH_GFX;
        public static readonly string FILEPATH_SFX;
        public static readonly string FILEPATH_MUSIC;


        static Constants()
        {
            FILEPATH_DATA = Engine.ROOT_PATH + "/Client Data/";
            FILEPATH_PLUGINS = FILEPATH_DATA + "/plugins/";
            FILEPATH_SHADERS = FILEPATH_DATA + "/shaders/";
            FILEPATH_GFX = FILEPATH_DATA + "gfx/";
            FILEPATH_SFX = FILEPATH_DATA + "sfx/";
            FILEPATH_MUSIC = FILEPATH_DATA + "music/";
        }

    }
}