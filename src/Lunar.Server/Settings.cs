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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
using Lunar.Core;
using Lunar.Core.Utilities;

namespace Lunar.Server
{
    public static class Settings
    {
        private static readonly string _filePathConfig = Constants.FILEPATH_DATA + "config.json";
        private static readonly string _filePathExperience = Constants.FILEPATH_DATA + "experience.conf";
        private static readonly string _filePathUserPermissions = Constants.FILEPATH_DATA + "user_permissions.json";

        public static string GameName { get; private set; }

        public static int ServerPort { get; private set; }

        public static string WelcomeMessage { get; private set; }

        public static string StartingMap { get; private set; }

        public static int MaxInventoryItems { get; private set; }

        public static int TickRate { get; private set; }

        public static int TileSize { get; private set; }

        public static int NPCRestPeriod { get; private set; }

        public static int MapItemWidth { get; private set; }

        public static int MapItemHeight { get; private set; }

        public static Dictionary<string, Role> Roles { get; private set; }

        public static Role DefaultRole { get; private set; }

        public static int MaxLevel { get; private set; }

        public static int[] ExperienceThreshhold { get; private set; }

        public static Dictionary<string, Role> UserPermissions { get; private set; }

        public static bool SuppressErrors { get; private set; }

        private static Logger _logger;

        public static void Initalize(Logger logger)
        {
            _logger = logger;
            LoadConfig();
            ApplyEnvironmentOverrides();
            LoadExperienceChart();
            LoadUserPermissions();
        }

        private static void CreateConfig()
        {
            var json = new
            {
                general = new
                {
                    port = 25566,
                    gameName = "Lunar Engine",
                    welcomeMessage = "Welcome to Lunar Engine!"
                },
                gameplay = new
                {
                    startingMap = "default",
                    maxInventorySlots = 30,
                    npcRestPeriod = 400,
                    maxLevel = 100
                },
                advanced = new
                {
                    tickRate = 60,
                    tileSize = 32,
                    mapItemWidth = 32,
                    mapItemHeight = 32,
                    suppressErrors = true
                },
                roles = new
                {
                    user = 0,
                    admin = 1
                },
                defaultRole = "user"
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(json, options);
            File.WriteAllText(_filePathConfig, jsonString);
        }

        /// <summary>
        /// Test and container hook: LUNAR_SERVER_PORT overrides the port from config.json when set.
        /// </summary>
        private static void ApplyEnvironmentOverrides()
        {
            if (int.TryParse(Environment.GetEnvironmentVariable("LUNAR_SERVER_PORT"), out var port) && port > 0)
                Settings.ServerPort = port;
        }

        private static void LoadConfig()
        {
            if (!File.Exists(_filePathConfig))
                CreateConfig();

            try
            {
                var jsonString = File.ReadAllText(_filePathConfig);
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                var generalSettings = root.GetProperty("general");
                Settings.ServerPort = generalSettings.GetProperty("port").GetInt32();
                Settings.GameName = generalSettings.GetProperty("gameName").GetString();
                Settings.WelcomeMessage = generalSettings.GetProperty("welcomeMessage").GetString();

                var gameplaySettings = root.GetProperty("gameplay");
                Settings.StartingMap = gameplaySettings.GetProperty("startingMap").GetString();
                Settings.MaxInventoryItems = gameplaySettings.GetProperty("maxInventorySlots").GetInt32();
                Settings.NPCRestPeriod = gameplaySettings.GetProperty("npcRestPeriod").GetInt32();
                Settings.MaxLevel = gameplaySettings.GetProperty("maxLevel").GetInt32();

                var advancedSettings = root.GetProperty("advanced");
                Settings.TickRate = advancedSettings.GetProperty("tickRate").GetInt32();
                Settings.TileSize = advancedSettings.GetProperty("tileSize").GetInt32();
                Settings.MapItemWidth = advancedSettings.GetProperty("mapItemWidth").GetInt32();
                Settings.MapItemHeight = advancedSettings.GetProperty("mapItemHeight").GetInt32();
                Settings.SuppressErrors = advancedSettings.GetProperty("suppressErrors").GetBoolean();

                Settings.Roles = new Dictionary<string, Role>();
                var rolesSettings = root.GetProperty("roles");
                foreach (var roleProp in rolesSettings.EnumerateObject())
                {
                    Settings.Roles.Add(roleProp.Name.ToLower(), new Role(roleProp.Name.ToLower(), roleProp.Value.GetInt32()));
                }

                string defaultRole = root.GetProperty("defaultRole").GetString();
                Settings.DefaultRole = Settings.Roles[defaultRole] ?? Role.Default;
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is NullReferenceException || ex is JsonException)
            {
                Console.WriteLine("The server config file appears to be corrupted!");
                Console.Write("Would you like to restore the configuration to its original state? [y/n]");

                if (Console.ReadLine() == "y")
                {
                    CreateConfig();
                    LoadConfig();
                }
                else
                {
                    Console.WriteLine("Terminating server...");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
        }

        private static void LoadUserPermissions()
        {
            UserPermissions = new Dictionary<string, Role>();

            if (!File.Exists(_filePathUserPermissions))
            {
                _logger.LogEvent($"Could not load user permissions: file does not exist at {_filePathUserPermissions}!", LogTypes.ERROR);
                return;
            }

            try
            {
                var jsonString = File.ReadAllText(_filePathUserPermissions);
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                foreach (var permission in root.GetProperty("permissions").EnumerateArray())
                {
                    string userName = permission.GetProperty("name").GetString();
                    string roleName = permission.GetProperty("role").GetString().ToLower();

                    Role role = Settings.Roles[roleName] ?? Role.Default;

                    if (UserPermissions.ContainsKey(userName))
                        UserPermissions[userName] = role;
                    else
                        UserPermissions.Add(userName, role);
                }
            }
            catch (Exception ex)
            {
                _logger.LogEvent($"Could not load user permissions: {ex.Message}", LogTypes.ERROR, ex);
            }
        }

        private static void LoadExperienceChart()
        {
            Console.WriteLine("Loading experience chart...");

            var lines = File.ReadAllLines(_filePathExperience);
            Settings.ExperienceThreshhold = new int[Settings.MaxLevel];
            int i = 0;
            foreach (var line in lines)
            {
                if (i >= Settings.ExperienceThreshhold.Length)
                {
                    _logger.LogEvent("Experience chart exceeds maximum level!", LogTypes.ERROR, new Exception("Experience chart exceeds maximum level!"));
                    return;
                }

                int.TryParse(line, out int xp);

                Settings.ExperienceThreshhold[i++] = xp;
            }

            Console.WriteLine($"Loaded experience config for {Settings.MaxLevel} levels.");
        }
    }
}