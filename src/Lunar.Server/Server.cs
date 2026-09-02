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

using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Scripting;
using Lunar.Server.World;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Structure;
using System;
using System.IO;
using System.Threading;
using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Server.Utilities.Commands;
using Lunar.Server.Utilities.Events;
using Lunar.Server.Utilities.Plugin;
using System.Diagnostics;
using Lunar.Server.World.Conversation;
using Lunar.Server.World.Structure.Attribute;
using Lunar.Core.Utilities.Data.Management;
using Microsoft.Extensions.DependencyInjection;

namespace Lunar.Server
{
    public class Server
    {
        public static bool ShutDown { get; set; }

        private IServiceProvider _services;
        private WebCommunicator _webCommunicator;

        private Thread _netThread;
        private Thread _worldThread;

        public Server()
        {
        }

        public void Initalize()
        {
            Console.WriteLine("Firing up engine...");

#if DEV_MODE
            string rootPath = Engine.FindDevRootPath("Server Data");
#else
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
#endif

            // Test and container hook: a directory that contains "Server Data".
            var rootOverride = Environment.GetEnvironmentVariable("LUNAR_DATA_ROOT");
            if (!string.IsNullOrWhiteSpace(rootOverride))
                rootPath = rootOverride;

            Engine.Initialize(rootPath);

            Console.WriteLine("Initalizing server...");

            // Configure logger before settings so the latter can log validation errors at startup.
            var bootstrapLogger = new Logger();
            bootstrapLogger.LogPath = Constants.FILEPATH_LOGS;
            bootstrapLogger.Start();

            Console.WriteLine("Loading server settings...");
            Settings.Initalize(bootstrapLogger);

            bootstrapLogger.SuppressErrors = Settings.SuppressErrors;

            Console.WriteLine($"Log output set to: {bootstrapLogger.LogPath} with error suppression {(bootstrapLogger.SuppressErrors ? "on" : "off")}.");

            Console.WriteLine("Checking file integrity...");
            this.CheckFileIntegrity();

            // Compose the service graph. Constructor-injected dependencies are
            // resolved transitively by the container.
            var services = new ServiceCollection();

            services.AddSingleton(bootstrapLogger);
            services.AddSingleton<NetHandler>(sp => new NetHandler(Settings.GameName, Settings.ServerPort, sp.GetRequiredService<Logger>()));
            services.AddSingleton<TileAttributeActionHandlerFactory>();
            services.AddSingleton<IDataManagerFactory, FSDataFactory>();
            services.AddSingleton<ItemManager>();
            services.AddSingleton<ClassManager>();
            services.AddSingleton<NPCManager>();
            services.AddSingleton<MapManager>();
            services.AddSingleton<PlayerManager>();
            services.AddSingleton<WorldManager>();
            services.AddSingleton<DialogueFactory>();
            services.AddSingleton<DialogueManager>();
            services.AddSingleton<GameEventListener>();
            services.AddSingleton<PluginManager>();
            services.AddSingleton<CommandHandler>();
            services.AddSingleton<ScriptHost>(sp =>
                new ScriptHost(Constants.FILEPATH_SCRIPTS, sp.GetRequiredService<Logger>()));

            // Action handlers (per-tile) are constructed by the factory via
            // ActivatorUtilities; they don't need explicit registration.

            _services = services.BuildServiceProvider();

            // Initalize order matters: data factory first, then content managers
            // (which load their data via the factory), then connections.
            _services.GetRequiredService<IDataManagerFactory>().Initalize();
            _services.GetRequiredService<NetHandler>().Initalize();
            _services.GetRequiredService<ScriptHost>().Initialize();
            _services.GetRequiredService<ItemManager>().Initalize();
            _services.GetRequiredService<ClassManager>().Initalize();
            _services.GetRequiredService<NPCManager>().Initalize();
            _services.GetRequiredService<MapManager>().Initalize();
            _services.GetRequiredService<WorldManager>().Initalize();
            _services.GetRequiredService<PlayerManager>().Initalize();
            _services.GetRequiredService<DialogueManager>().Initalize();
            _services.GetRequiredService<GameEventListener>().Initalize();
            _services.GetRequiredService<PluginManager>().Initalize();
            _services.GetRequiredService<CommandHandler>().Initalize();

            _webCommunicator = new WebCommunicator();
        }

        public void Start()
        {
            _services.GetRequiredService<NetHandler>().Start();

            _webCommunicator.Run();

            this.BeginServerLoop();

            // Machine-readable readiness marker for tooling and tests.
            Console.WriteLine($"Server ready on port {Settings.ServerPort}");
        }

        /// <summary>
        /// Requests shutdown and blocks until both loops have exited and the world has been saved.
        /// </summary>
        public void Stop()
        {
            Server.ShutDown = true;
            _netThread?.Join(TimeSpan.FromSeconds(10));
            _worldThread?.Join(TimeSpan.FromSeconds(10));
        }

        /// <summary>Blocks the calling thread until the loops end (normally via <see cref="Stop"/>).</summary>
        public void WaitForShutdown()
        {
            _netThread?.Join();
            _worldThread?.Join();
        }

        private void BeginServerLoop()
        {
            var netHandler = _services.GetRequiredService<NetHandler>();
            var worldManager = _services.GetRequiredService<WorldManager>();

            _netThread = new Thread(() =>
            {
                var gametime = new GameTime();
                var serverWorldHeartbeat = new ServerHeartbeat(netHandler.Update);

                while (!Server.ShutDown)
                {
                    serverWorldHeartbeat.Update(gametime);
                }
            });

            _worldThread = new Thread(() =>
            {
                var gametime = new GameTime();
                var serverWorldHeartbeat = new ServerHeartbeat(worldManager.Update);

                while (!Server.ShutDown)
                {
                    serverWorldHeartbeat.Update(gametime);
                }

                worldManager.Save();
            });

            _netThread.Start();
            _worldThread.Start();
        }

        private void CheckFileIntegrity()
        {
            Directory.CreateDirectory(Constants.FILEPATH_DATA);

            Directory.CreateDirectory(Constants.FILEPATH_SCRIPTS);

            Directory.CreateDirectory(Constants.FILEPATH_ACCOUNTS);

            Directory.CreateDirectory(Constants.FILEPATH_ITEMS);

            Directory.CreateDirectory(Constants.FILEPATH_LOGS);

            Directory.CreateDirectory(Constants.FILEPATH_MAPS);

            Directory.CreateDirectory(Constants.FILEPATH_NPCS);

            Directory.CreateDirectory(Constants.FILEPATH_ANIMATIONS);

            Directory.CreateDirectory(Constants.FILEPATH_SPELLS);
        }
    }
}