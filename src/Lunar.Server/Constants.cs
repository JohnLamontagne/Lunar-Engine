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

        public static readonly string FILEPATH_NPCS = FILEPATH_DATA + "/Npcs/";

        public static readonly string FILEPATH_LOGS = FILEPATH_DATA + "/Logs/";

        public static readonly string FILEPATH_ITEMS = FILEPATH_DATA + "/Items/";

        public static readonly string FILEPATH_MAPS = FILEPATH_DATA + "/Maps/";

        public const int DUNGEON_LEVEL_RANGE = 10;

        public const int DUNGEON_DESPAWN_TIME = 360000;

        public const string GAME_NAME = "Lunar Engine";

        public const string WELCOME_MSG = "Welcome to " + GAME_NAME;

        public const int SERVER_PORT = 25566;

        public const int MAX_MATCH_PLAYERS = 4;

        public const uint TICK_RATE = 60;

        public const int MAX_INVENTORY = 30;

        public const int TILE_SIZE = 32;

        public const int NPC_REST_PERIOD = 400;

        public const string STARTER_MAP = "Default";

        public const int MAP_ITEM_WIDTH = 32;

        public const int MAP_ITEM_HEIGHT = 32;
    }
}