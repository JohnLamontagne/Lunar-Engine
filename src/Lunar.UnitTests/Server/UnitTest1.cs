using System.IO;
using Lidgren.Network;
using Lunar.Client;
using Lunar.Core.Utilities;
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

            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_SCRIPTS))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_SCRIPTS);

            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_ACCOUNTS))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_ACCOUNTS);

            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_ITEMS))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_ITEMS);

            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_LOGS))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_LOGS);

            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_MAPS))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_MAPS);

            if (!Directory.Exists(Lunar.Server.Constants.FILEPATH_NPCS))
                Directory.CreateDirectory(Lunar.Server.Constants.FILEPATH_NPCS);

            if (!File.Exists(Lunar.Server.Constants.FILEPATH_ACCOUNTS + "admins.txt"))
                File.Create(Constants.FILEPATH_ACCOUNTS + "admins.txt");
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
