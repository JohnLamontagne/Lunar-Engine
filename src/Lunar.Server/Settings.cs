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
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Data;
using Lunar.Server.Utilities.Data.FileSystem;

namespace Lunar.Server
{
    public static class Settings
    {
        private static readonly string _filePathConfig = Constants.FILEPATH_DATA + "config.xml";
        private static readonly string _filePathExperience = Constants.FILEPATH_DATA + "experience.conf";
        private static readonly string _filePathUserPermissions = Constants. FILEPATH_DATA + "user_permissions.xml";

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

        public static string IronPythonLibsDirectory { get; private set; }

        public static bool SuppressErrors { get; private set; }

        public static void Initalize()
        {
            LoadConfig();
            LoadExperienceChart();
            LoadUserPermissions();
        }

        private static void CreateConfig()
        {
            var xml = new XElement("Config",
                new XElement("General",
                    new XElement("Port", 25566),
                    new XElement("Game_Name", "Lunar Engine"),
                    new XElement("Welcome_Message", "Welcome to Lunar Engine!")
                ),
                new XElement("Gameplay",
                    new XElement("Starting_Map", "default"),
                    new XElement("Max_Inventory_Slots", 30),
                    new XElement("NPC_Rest_Period", 400),
                    new XElement("Max_Level", 100)
                ),
                new XElement("Advanced",
                    new XElement("Tick_Rate", 60),
                    new XElement("Tile_Size", 32),
                    new XElement("Map_Item_Width", 32),
                    new XElement("Map_Item_Height", 32),
                    new XElement("Iron_Python_Libs_Dir", "C:/Program Files/IronPython 2.7/Lib"),
                    new XElement("Suppress_Errors", "true")
                ),
                new XElement("Roles",
                    new XElement("User", 0),
                    new XElement("Admin", 1)
                ),
                new XElement("Default_Role", "User")
            );
            xml.Save(_filePathConfig);
        }

        private static void LoadConfig()
        {
            if (!File.Exists(_filePathConfig))
                CreateConfig();

            try
            {
                var doc = XDocument.Load(_filePathConfig);

                var generalSettings = doc.Elements("Config").Elements("General");
                Settings.ServerPort = int.Parse(generalSettings.Elements("Port").FirstOrDefault().Value);
                Settings.GameName = generalSettings.Elements("Game_Name").FirstOrDefault().Value;
                Settings.WelcomeMessage = generalSettings.Elements("Welcome_Message").FirstOrDefault().Value;

                var gameplaySettings = doc.Elements("Config").Elements("Gameplay");
                Settings.StartingMap = gameplaySettings.Elements("Starting_Map").FirstOrDefault().Value;
                Settings.MaxInventoryItems = int.Parse(gameplaySettings.Elements("Max_Inventory_Slots").FirstOrDefault().Value);
                Settings.NPCRestPeriod = int.Parse(gameplaySettings.Elements("NPC_Rest_Period").FirstOrDefault().Value);
                Settings.MaxLevel = int.Parse(gameplaySettings.Elements("Max_Level").FirstOrDefault().Value);

                var advancedSettings = doc.Elements("Config").Elements("Advanced");
                Settings.TickRate = int.Parse(advancedSettings.Elements("Tick_Rate").FirstOrDefault().Value);
                Settings.TileSize = int.Parse(advancedSettings.Elements("Tile_Size").FirstOrDefault().Value);
                Settings.MapItemWidth = int.Parse(advancedSettings.Elements("Map_Item_Width").FirstOrDefault().Value);
                Settings.MapItemHeight = int.Parse(advancedSettings.Elements("Map_Item_Height").FirstOrDefault().Value);
                Settings.IronPythonLibsDirectory = advancedSettings.Elements("Iron_Python_Libs_Dir").FirstOrDefault().Value;
                Settings.SuppressErrors = bool.Parse(advancedSettings.Elements("Suppress_Errors").FirstOrDefault().Value);

                // Get the roles
                Settings.Roles = new Dictionary<string, Role>();
                var roleSettings = doc.Elements("Config").Elements("Roles").FirstOrDefault();
                foreach (var role in roleSettings.Elements())
                {
                    Settings.Roles.Add(role.Name.ToString(), new Role(role.Name.ToString(), int.Parse(role.Value)));
                }

                string defaultRole = doc.Elements("Config").Elements("Default_Role").FirstOrDefault().Value.ToString();

                Settings.DefaultRole = Settings.Roles[defaultRole] ?? Role.Default;
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is NullReferenceException)
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
                Logger.LogEvent($"Could not load user permissions: file does not exist at {_filePathUserPermissions}!", LogTypes.ERROR);
                return;
            }
                

            try
            {
                var doc = XDocument.Load(_filePathUserPermissions);

                foreach (var element in doc.Elements("Permissions").Elements())
                {
                    string userName = element.Attribute("name").Value;
                    string roleName = element.Attribute("role").Value;

                    Role role = Settings.Roles[roleName] ?? Role.Default;

                    if (UserPermissions.ContainsKey(userName))
                        UserPermissions[userName] = role;
                    else
                        UserPermissions.Add(userName, role);
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Could not load user permissions: {ex.Message}", LogTypes.ERROR, ex);
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
                    Logger.LogEvent("Experience chart exceeds maximum level!", LogTypes.ERROR, new Exception("Experience chart exceeds maximum level!"));
                    return;
                }

                int.TryParse(line, out int xp);

                Settings.ExperienceThreshhold[i++] = xp;
            }

            Console.WriteLine($"Loaded experience config for {Settings.MaxLevel} levels.");
        }
    }
}
