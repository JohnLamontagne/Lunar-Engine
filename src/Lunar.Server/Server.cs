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
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Structure;
using System;
using System.IO;
using System.Threading;
using Lunar.Core.Utilities;
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

            Console.WriteLine("Loading server settings...");
            Settings.Initalize();

            Console.WriteLine("Checking file integrity...");
            this.CheckFileIntegrity();

            Server.ServiceLocator.RegisterService(new ScriptManager());

            _netHandler = new NetHandler(Settings.GameName, Settings.ServerPort);
            Packet.Initalize(_netHandler);

            // Create and initalize the game content managers.
            Server.ServiceLocator.RegisterService(new ItemManager());
            Server.ServiceLocator.RegisterService(new NPCManager());
            Server.ServiceLocator.RegisterService(new MapManager());

            Server.ServiceLocator.RegisterService(new WorldManager(_netHandler));
            Server.ServiceLocator.RegisterService(new PlayerManager());

            Server.ServiceLocator.RegisterService(new GameEventListener());

            var pluginManager = new PluginManager();
            pluginManager.Initalize();
            Server.ServiceLocator.RegisterService(pluginManager);

            CommandHandler commandHandler = new CommandHandler(_netHandler);
            Server.ServiceLocator.RegisterService(commandHandler);
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
                        Server.ServiceLocator.GetService<WorldManager>().Update(gameTime);

                        GameTimerManager.Instance.Update(gameTime);

                        nextUpdateTime = gameTime.TotalElapsedTime + millisecondsPerUpdate;

                        gameTime.Update();
                    }
                }
            });

            _netThread.Start();
            _worldThread.Start();

        }

        private void CheckFileIntegrity()
        {
            if (!Directory.Exists(Constants.FILEPATH_DATA))
                Directory.CreateDirectory(Constants.FILEPATH_DATA);

            if (!Directory.Exists(Constants.FILEPATH_SCRIPTS))
                Directory.CreateDirectory(Constants.FILEPATH_SCRIPTS);

            if (!Directory.Exists(Constants.FILEPATH_ACCOUNTS))
                Directory.CreateDirectory(Constants.FILEPATH_ACCOUNTS);

            if (!Directory.Exists(Constants.FILEPATH_ITEMS))
                Directory.CreateDirectory(Constants.FILEPATH_ITEMS);

            if (!Directory.Exists(Constants.FILEPATH_LOGS))
                Directory.CreateDirectory(Constants.FILEPATH_LOGS);

            if (!Directory.Exists(Constants.FILEPATH_MAPS))
                Directory.CreateDirectory(Constants.FILEPATH_MAPS);

            if (!Directory.Exists(Constants.FILEPATH_NPCS))
                Directory.CreateDirectory(Constants.FILEPATH_NPCS);
        }
    }
}