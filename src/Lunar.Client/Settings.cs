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
using System.IO;
using System.Text.Json;
using System.Threading;

namespace Lunar.Client
{
    public static class Settings
    { 
        public static string GameName { get; set; }

        public static string Website { get; set; }

        public static int ResolutionX { get; set; }
        public static int ResolutionY { get; set; }

        public static string IP { get; set; }
        public static int Port { get; set; }

        public static bool DisplayNetworkMessages { get; set; }

        public static void Initalize()
        {
            LoadConfig();
            ApplyEnvironmentOverrides();
        }

        /// <summary>
        /// Test and container hooks: LUNAR_SERVER_HOST, LUNAR_SERVER_PORT and LUNAR_RESOLUTION (WxH)
        /// override the values from config.json when set.
        /// </summary>
        private static void ApplyEnvironmentOverrides()
        {
            var host = Environment.GetEnvironmentVariable("LUNAR_SERVER_HOST");
            if (!string.IsNullOrWhiteSpace(host))
                Settings.IP = host.Trim();

            if (int.TryParse(Environment.GetEnvironmentVariable("LUNAR_SERVER_PORT"), out var port) && port > 0)
                Settings.Port = port;

            var resolution = Environment.GetEnvironmentVariable("LUNAR_RESOLUTION");
            if (!string.IsNullOrWhiteSpace(resolution))
            {
                var parts = resolution.ToLowerInvariant().Split('x');
                if (parts.Length == 2 && int.TryParse(parts[0], out var w) && int.TryParse(parts[1], out var h) && w > 0 && h > 0)
                {
                    Settings.ResolutionX = w;
                    Settings.ResolutionY = h;
                }
            }
        }

        private static void CreateConfig()
        {
            var json = new
            {
                general = new
                {
                    gameName = "Lunar Engine",
                    ip = "127.0.0.1",
                    port = 25566,
                    website = "https://www.rpgorigin.com"
                },
                display = new
                {
                    resolutionX = 1600,
                    resolutionY = 900
                },
                advanced = new
                {
                    displayNetworkMessages = true
                }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(json, options);
            File.WriteAllText(Constants.FILEPATH_DATA + "config.json", jsonString);
        }

        private static void LoadConfig()
        {
            if (!File.Exists(Constants.FILEPATH_DATA + "config.json"))
                CreateConfig();

            try
            {
                var jsonString = File.ReadAllText(Constants.FILEPATH_DATA + "config.json");
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                var generalSettings = root.GetProperty("general");
                Settings.GameName = generalSettings.GetProperty("gameName").GetString();
                Settings.Website = generalSettings.GetProperty("website").GetString();
                Settings.Port = generalSettings.GetProperty("port").GetInt32();
                Settings.IP = generalSettings.GetProperty("ip").GetString();

                var displaySettings = root.GetProperty("display");
                Settings.ResolutionX = displaySettings.GetProperty("resolutionX").GetInt32();
                Settings.ResolutionY = displaySettings.GetProperty("resolutionY").GetInt32();

                var advancedSettings = root.GetProperty("advanced");
                Settings.DisplayNetworkMessages = advancedSettings.GetProperty("displayNetworkMessages").GetBoolean();
            }
            catch (Exception ex) when (ex is IndexOutOfRangeException || ex is JsonException)
            {
                Console.WriteLine("The client config file appears to be corrupted!");
                Console.Write("Would you like to restore the configuration to its original state? [y/n]");

                if (Console.ReadLine() == "y")
                {
                    CreateConfig();
                }
                else
                {
                    Console.WriteLine("Terminating server...");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                }
            }
        }
    }
}