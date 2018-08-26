using System.IO;
using Lunar.Core;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Net;
using Lunar.Server.World.Actors;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Constants = Lunar.Server.Constants;

namespace Lunar.UnitTests.Server
{
    [TestClass]
    public class ServerPlayerTests
    {
        
        public ServerPlayerTests()
        {
            this.BuildDirectories();
            Packet.Initalize(new NetHandler("s", 0000));
        }

        [TestMethod]
        public void TestNewPlayer()
        {
            PlayerDescriptor descriptor = new PlayerDescriptor("test", "test");

            Player player = new Player(descriptor, new TestPlayerConnection());

            Assert.IsTrue(player != null);
        }

        [TestMethod]
        public void TestPlayerLogin()
        {
            PlayerManager playerManager = new PlayerManager();
            playerManager.LoginPlayer("bla", "bla", new TestPlayerConnection());
        }

        [TestMethod]
        public void TestRegisterPlayer()
        {
            PlayerManager playerManager = new PlayerManager();
            var player = playerManager.RegisterPlayer("bla", "bla", new TestPlayerConnection());

            Assert.IsNotNull(player);
        }


        private void BuildDirectories()
        {
            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_DATA))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_DATA);

            if (!Directory.Exists(EngineConstants.FILEPATH_SCRIPTS))
                Directory.CreateDirectory(EngineConstants.FILEPATH_SCRIPTS);

            if (!Directory.Exists(EngineConstants.FILEPATH_ACCOUNTS))
                Directory.CreateDirectory(EngineConstants.FILEPATH_ACCOUNTS);

            if (!Directory.Exists(EngineConstants.FILEPATH_ITEMS))
                Directory.CreateDirectory(EngineConstants.FILEPATH_ITEMS);

            if (!Directory.Exists(EngineConstants.FILEPATH_LOGS))
                Directory.CreateDirectory(EngineConstants.FILEPATH_LOGS);

            if (!Directory.Exists(EngineConstants.FILEPATH_MAPS))
                Directory.CreateDirectory(EngineConstants.FILEPATH_MAPS);

            if (!Directory.Exists(EngineConstants.FILEPATH_NPCS))
                Directory.CreateDirectory(EngineConstants.FILEPATH_NPCS);

            if (!File.Exists(EngineConstants.FILEPATH_ACCOUNTS + "admins.txt"))
                File.Create(EngineConstants.FILEPATH_ACCOUNTS + "admins.txt");
        }

        public class TestPlayerConnection : PlayerConnection
        {
            public TestPlayerConnection()
                : base(null, new NetHandler("bla", 0000))
            {
                
            }
        }
    }

   
}
