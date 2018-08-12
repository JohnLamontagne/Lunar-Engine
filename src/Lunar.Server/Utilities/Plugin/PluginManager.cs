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
using System.Reflection;
using Lunar.Core.Utilities;

namespace Lunar.Server.Utilities.Plugin
{
    public class PluginManager : IService
    {
        private readonly List<Plugin> _plugins;

        public PluginManager()
        {
            _plugins = new List<Plugin>();
        }

        private void LoadPlugins(string pluginDirectory)
        {
            Console.WriteLine("Loading plugins...");

            if (!Directory.Exists(pluginDirectory))
                Directory.CreateDirectory(pluginDirectory);

            string[] files = Directory.GetFiles(pluginDirectory);
            foreach (string file in files)
            {
                if (file.EndsWith(".dll"))
                {
                    this.LoadPlugin(file);
                }
            }

            Console.WriteLine("Loaded {0} plugins.", _plugins.Count);
        }

        private void LoadPlugin(string path)
        {
            try
            {
                Assembly.LoadFile(Path.GetFullPath(path));

                Type pluginType = typeof(Plugin);

                // Get all types that implement Plugin
                Type[] pluginTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(p => pluginType.IsAssignableFrom(p) && !pluginType.IsAbstract && p.IsClass)
                    .ToArray();

                foreach (var type in pluginTypes)
                {
                    var plugin = (Plugin) Activator.CreateInstance(type);
                    plugin.Initalize();
                    _plugins.Add(plugin);
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Could not load plugin: {ex.Message}", LogTypes.ERROR, Environment.StackTrace);
            }
           
        }

        public void Initalize()
        {
            this.LoadPlugins(Constants.FILEPATH_PLUGINS);
        }
    }
}
