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

using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Structure;
using Lunar.Server.Utilities;

namespace Lunar.Server.World
{
    public class WorldManager : IService
    {
        private readonly WorldDictionary<string, Map> _maps;

        public WorldManager(NetHandler netHandler)
        {
            netHandler.AddPacketHandler(PacketType.LOGIN, this.Handle_PlayerLogin);
            netHandler.AddPacketHandler(PacketType.REGISTER, this.Handle_PlayerRegister);
            netHandler.AddPacketHandler(PacketType.PLAYER_MSG, this.Handle_PlayerMessage);
            netHandler.AddPacketHandler(PacketType.QUIT_GAME, this.Handle_QuitGame);
            netHandler.ConnectionLost += Player_Connection_Lost;

            _maps = new WorldDictionary<string, Map>();
        }

        private void Handle_QuitGame(PacketReceivedEventArgs args)
        {
            args.Connection.Disconnect("bye");
        }

        private void Player_Connection_Lost(object sender, ConnectionEventArgs e)
        {
            Player player = Server.ServiceLocator.GetService<PlayerManager>().GetPlayer(e.Connection.RemoteUniqueIdentifier);

            if (player == null)
                return;
      
            player.LeaveGame();
            Server.ServiceLocator.GetService<PlayerManager>().RemovePlayer(player.UniqueID);
        }

        private void Handle_PlayerMessage(PacketReceivedEventArgs args)
        {
            Player player = Server.ServiceLocator.GetService<PlayerManager>().GetPlayer(args.Connection.UniqueIdentifier);

            // Make sure the sender is online.
            if (player == null) return;

            var message = player.Name + ": " + args.Message.ReadString();

            player.Map.SendChatMessage(message, ChatMessageType.Regular);
        }

        private void JoinGame(Player player)
        {
            if (!_maps.ContainsKey(player.MapID))
            {
                this.AddMap(player.MapID, Server.ServiceLocator.GetService<MapManager>().GetMap(player.MapID));
            }

            player.JoinGame(_maps[player.MapID]);
        }

        private void Handle_PlayerRegister(PacketReceivedEventArgs args)
        {
            // Get requested username.
            var username = args.Message.ReadString();

            // Get specified password hash.
            var password = args.Message.ReadString();

            PlayerConnection senderConn = args.Connection;

            bool registerSuccess = Server.ServiceLocator.GetService<PlayerManager>().RegisterPlayer(username, password, senderConn);

            if (registerSuccess)
            {
                var player = Server.ServiceLocator.GetService<PlayerManager>().GetPlayer(senderConn.UniqueIdentifier);

                this.JoinGame(player);
            }
        }

        private void Handle_PlayerLogin(PacketReceivedEventArgs args)
        {
            // Get requested username.
            string username = args.Message.ReadString();

            // Get specified password hash.
            string password = args.Message.ReadString();

            PlayerConnection senderConn = args.Connection;

            var loginSuccess = Server.ServiceLocator.GetService<PlayerManager>().LoginPlayer(username, password, senderConn);

            if (loginSuccess)
            {
                var player = Server.ServiceLocator.GetService<PlayerManager>().GetPlayer(senderConn.UniqueIdentifier);

                this.JoinGame(player);
            }
        }

        public Map GetMap(string id)
        {
            if (!_maps.ContainsKey(id))
            {
                _maps.Add(id, Server.ServiceLocator.GetService<MapManager>().GetMap(id));
            }

            return _maps[id];
        }

        public void AddMap(string id, Map map)
        {
            _maps.Add(id, map);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var map in _maps)
            {
                map.Update(gameTime);
            }
        }

        public void Initalize()
        {
        }
    }
}
