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
            PlayerModel descriptor = new PlayerModel("test", "test");

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

            if (!File.Exists(Constants.FILEPATH_ACCOUNTS + "admins.txt"))
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