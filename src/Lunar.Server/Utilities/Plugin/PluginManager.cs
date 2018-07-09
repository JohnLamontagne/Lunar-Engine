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
        private List<Plugin> _plugins;

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
                    Assembly.LoadFile(Path.GetFullPath(file));
                }
            }

            Type pluginType = typeof(Plugin);

            // Get all types that implement Plugin
            Type[] pluginTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(p => pluginType.IsAssignableFrom(p) && !pluginType.IsAbstract && p.IsClass)
                .ToArray();

            foreach (var type in pluginTypes)
            {
                var plugin = (Plugin)Activator.CreateInstance(type);
                plugin.Initalize();
                _plugins.Add(plugin);
            }

            Console.WriteLine("Loaded {0} plugins.", _plugins.Count);
        }

        public void Initalize()
        {
            this.LoadPlugins(Constants.FILEPATH_PLUGINS);
        }
    }
}
