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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace Lunar.Client
{
    public static class Settings
    {
        private static readonly string _filePath = Constants.FILEPATH_DATA + "config.xml";

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
        }

        private static void CreateConfig()
        {
            var xml = new XElement("Config",
                new XElement("General",
                    new XElement("Game_Name", "Lunar Engine"),
                    new XElement("IP", "127.0.0.1"),
                    new XElement("Port", 25566),
                    new XElement("Website", "https://www.rpgorigin.com")
                ),
                new XElement("Display",
                    new XElement("Resolution_X", 1600),
                    new XElement("Resolution_Y", 900)
                ),
                new XElement("Advanced",
                    new XElement("DisplayNetworkMessages", true)
                )
            );
            xml.Save(_filePath);
        }

        private static void LoadConfig()
        {
            if (!File.Exists(_filePath))
                CreateConfig();

            try
            {
                var doc = XDocument.Load(_filePath);

                var generalSettings = doc.Elements("Config").Elements("General");
                Settings.GameName = generalSettings.Elements("Game_Name").FirstOrDefault().Value;
                Settings.Website = generalSettings.Elements("Website").FirstOrDefault().Value;
                Settings.Port = int.Parse(generalSettings.Elements("Port").FirstOrDefault().Value);
                Settings.IP = generalSettings.Elements("IP").FirstOrDefault().Value;


                var displaySettings = doc.Elements("Config").Elements("Display");
                Settings.ResolutionX = int.Parse(displaySettings.Elements("Resolution_X").FirstOrDefault().Value);
                Settings.ResolutionY = int.Parse(displaySettings.Elements("Resolution_Y").FirstOrDefault().Value);

                var advancedSettings = doc.Elements("Config").Elements("Advanced");
                Settings.DisplayNetworkMessages = bool.Parse(advancedSettings.Elements("DisplayNetworkMessages").FirstOrDefault().Value);


            }
            catch (IndexOutOfRangeException ex)
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