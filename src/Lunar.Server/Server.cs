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
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
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

namespace Lunar.Server
{
    public class Server
    {
        private static ServiceLocator _serviceLocator;

        public static bool ShutDown { get; set; }

        private WebCommunicator _webCommunicator;

        private Thread _netThread;
        private Thread _worldThread;

        private NetHandler _netHandler;

        public static ServiceLocator ServiceLocator { get { return _serviceLocator = _serviceLocator ?? new ServiceLocator(); } }

        public Server()
        {
        }

        public void Initalize()
        {
            Console.WriteLine("Initalizing server...");

            // Point the logger towards the current directory
            Logger.LogPath = Constants.FILEPATH_LOGS;

            Console.WriteLine("Log output set to: " + Logger.LogPath);

            Console.WriteLine("Loading server settings...");
            Settings.Initalize();

            Console.WriteLine("Checking file integrity...");
            this.CheckFileIntegrity();

            Server.ServiceLocator.Register(new ScriptManager());

            _netHandler = new NetHandler(Settings.GameName, Settings.ServerPort);
            Packet.Initalize(_netHandler);

            // Register the data loader factories
            Server.ServiceLocator.Register(new FSDataFactory());

            // Create and initalize the game content managers.
            Server.ServiceLocator.Register(new ItemManager());
            Server.ServiceLocator.Register(new NPCManager());
            Server.ServiceLocator.Register(new MapManager());

            Server.ServiceLocator.Register(new WorldManager(_netHandler));
            Server.ServiceLocator.Register(new PlayerManager());

            Server.ServiceLocator.Register(new GameEventListener());

            var pluginManager = new PluginManager();
            pluginManager.Initalize();
            Server.ServiceLocator.Register(pluginManager);

            CommandHandler commandHandler = new CommandHandler(_netHandler);
            Server.ServiceLocator.Register(commandHandler);
            commandHandler.Initalize();

            _webCommunicator = new WebCommunicator();

            //WebCommunicator.SendUDP("127.0.0.1", 41181, WebCommunicator.MessageTypes.Status_Updates, "");
        }

        public void Start()
        {
            _netHandler.Start();

            _webCommunicator.Run();

            this.BeginServerLoop();
        }

        private void BeginServerLoop()
        {
            _netThread = new Thread(() =>
            {
                float millisecondsPerUpdate = 1000f / Settings.TickRate;
                float nextUpdateTime = 0;
                var gameTime = new GameTime();

                gameTime.Start();

                while (!Server.ShutDown)
                {
                    if (gameTime.TotalElapsedTime >= nextUpdateTime)
                    {
                        _netHandler.Update();

                        nextUpdateTime = gameTime.TotalElapsedTime + millisecondsPerUpdate;

                        gameTime.Update();
                    }
                }
            });

            _worldThread = new Thread(() =>
            {
                float millisecondsPerUpdate = 1000f / Settings.TickRate;
                float nextUpdateTime = 0;
                var gameTime = new GameTime();

                gameTime.Start();

                while (!Server.ShutDown)
                {
                    if (gameTime.TotalElapsedTime >= nextUpdateTime)
                    {
                        Server.ServiceLocator.Get<WorldManager>().Update(gameTime);


                        nextUpdateTime = gameTime.TotalElapsedTime + millisecondsPerUpdate;

                        gameTime.Update();
                    }
                }

                // Save the game world
                Server.ServiceLocator.Get<WorldManager>().Save();
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
        }
    }
}