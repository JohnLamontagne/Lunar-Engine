/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
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

            // Create and initalize the NetHandler.
            Server.ServiceLocator.RegisterService(new NetHandler());

            // Create and initalize the game content managers.
            Server.ServiceLocator.RegisterService(new ItemManager());
            Server.ServiceLocator.RegisterService(new NPCManager());
            Server.ServiceLocator.RegisterService(new MapManager());

            Server.ServiceLocator.RegisterService(new WorldManager());
            Server.ServiceLocator.RegisterService(new PlayerManager());

            Server.ServiceLocator.RegisterService(new GameEventListener());

            var pluginManager = new PluginManager();
            pluginManager.Initalize();
            Server.ServiceLocator.RegisterService(pluginManager);

            CommandHandler commandHandler = new CommandHandler();
            Server.ServiceLocator.RegisterService(commandHandler);
            commandHandler.Initalize();

            _webCommunicator = new WebCommunicator();

            //WebCommunicator.SendUDP("127.0.0.1", 41181, WebCommunicator.MessageTypes.Status_Updates, "");
        }

        public void Start()
        {
            Server.ServiceLocator.GetService<NetHandler>().Start();

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
                        Server.ServiceLocator.GetService<NetHandler>().Update();

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