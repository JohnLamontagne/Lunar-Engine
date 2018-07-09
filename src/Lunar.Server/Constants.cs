/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using System;

namespace Lunar.Server
{
    public static class Constants
    {
        public static readonly string FILEPATH_DATA = AppDomain.CurrentDomain.BaseDirectory + "/Data/";

        public static readonly string FILEPATH_ACCOUNTS = FILEPATH_DATA + "/Accounts/";

        public static readonly string FILEPATH_SCRIPTS = FILEPATH_DATA + "/Scripts/";

        public static readonly string FILEPATH_PLUGINS = FILEPATH_PLUGINS + "/Plugins/";

        public static readonly string FILEPATH_NPCS = FILEPATH_DATA + "/Npcs/";

        public static readonly string FILEPATH_LOGS = FILEPATH_DATA + "/Logs/";

        public static readonly string FILEPATH_ITEMS = FILEPATH_DATA + "/Items/";

        public static readonly string FILEPATH_MAPS = FILEPATH_DATA + "/Maps/";

        public const int MAP_ITEM_WIDTH = 32;

        public const int MAP_ITEM_HEIGHT = 32;
    }
}