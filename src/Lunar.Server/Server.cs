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
using System.Diagnostics;
using Lunar.Server.World.Dialogue;

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

        public Server()
        {
        }

        public void Initalize()
        {
            Console.WriteLine("Firing up engine...");

            Engine.Initialize();

            Console.WriteLine("Initalizing server...");

            Console.WriteLine("Loading server settings...");
            Settings.Initalize();

            Engine.Services.Get<Logger>().SuppressErrors = Settings.SuppressErrors;

            // Point the logger towards the current directory
            Engine.Services.Get<Logger>().LogPath = Constants.FILEPATH_LOGS;

            Engine.Services.Get<Logger>().Start();

            Console.WriteLine($"Log output set to: {Engine.Services.Get<Logger>().LogPath} with error suppression {(Engine.Services.Get<Logger>().SuppressErrors ? "on" : "off")}.");

            Console.WriteLine("Checking file integrity...");
            this.CheckFileIntegrity();

            Engine.Services.Register(new ScriptManager());

            _netHandler = new NetHandler(Settings.GameName, Settings.ServerPort);
            Packet.Initalize(_netHandler);

            // Register the data loader factories
            Engine.Services.Register(new FSDataFactory());

            // Create and initalize the game content managers.
            var itemManager = new ItemManager();
            Engine.Services.Register(itemManager);
            itemManager.Initalize();

            var npcManager = new NPCManager();
            Engine.Services.Register(npcManager);
            npcManager.Initalize();

            var mapManager = new MapManager();
            Engine.Services.Register(mapManager);
            mapManager.Initalize();

            var worldManager = new WorldManager(_netHandler);
            Engine.Services.Register(worldManager);
            worldManager.Initalize();

            var playerManager = new PlayerManager();
            Engine.Services.Register(playerManager);
            playerManager.Initalize();

            var dialogueManager = new DialogueManager();
            Engine.Services.Register(dialogueManager);
            dialogueManager.Initalize();

            var gameEventListener = new GameEventListener();
            Engine.Services.Register(gameEventListener);
            gameEventListener.Initalize();

            var pluginManager = new PluginManager();
            pluginManager.Initalize();
            Engine.Services.Register(pluginManager);

            CommandHandler commandHandler = new CommandHandler(_netHandler);
            Engine.Services.Register(commandHandler);
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
                var gametime = new GameTime();
                var serverWorldHeartbeat = new ServerHeartbeat(_netHandler.Update);

                while (!Server.ShutDown)
                {
                    serverWorldHeartbeat.Update(gametime);
                }
            });

            _worldThread = new Thread(() =>
            {
                var gametime = new GameTime();
                var serverWorldHeartbeat = new ServerHeartbeat(Engine.Services.Get<WorldManager>().Update);

                while (!Server.ShutDown)
                {
                    serverWorldHeartbeat.Update(gametime);
                }

                // Save the game world
                Engine.Services.Get<WorldManager>().Save();
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