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
using Lunar.Server.Utilities.Commands;
using Lunar.Server.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Lunar.Server.World.Actors
{
    public class PlayerManager : ISubject
    {
        private readonly Dictionary<string, Player> _players;

        private IDataManager<PlayerModel> _playerDataManager;

        private readonly IServiceProvider _services;

        public PlayerManager(IDataManagerFactory dataManagerFactory, IServiceProvider services)
        {
            _players = new Dictionary<string, Player>();
            _services = services;

            _playerDataManager = dataManagerFactory.Create<PlayerModel>(new FSDataFactoryArguments(Constants.FILEPATH_ACCOUNTS));
        }

        private Player CreatePlayer(PlayerModel descriptor, PlayerConnection connection)
        {
            return new Player(
                descriptor,
                connection,
                _services.GetRequiredService<ScriptHost>(),
                _services.GetRequiredService<CommandHandler>(),
                _services.GetRequiredService<Logger>());
        }

        private void AddPlayer(Player player)
        {
            _players.Add(player.UniqueID, player);

            player.LeftGame += (sender, args) =>
            {
                _playerDataManager.Save(player.Descriptor, null);
            };
        }

        public Player GetPlayer(string uniqueID)
        {
            if (!_players.ContainsKey(uniqueID))
                return null;
            else
                return _players[uniqueID];
        }

        public void Save()
        {
            foreach (var player in _players.Values)
                _playerDataManager.Save(player.Descriptor, null);
        }

        public Player GetPlayerByName(string name)
        {
            return _players.Values.FirstOrDefault(p => p.Descriptor.Name == name);
        }

        public void RemovePlayer(string uniqueID)
        {
            if (_players.ContainsKey(uniqueID))
                _players.Remove(uniqueID);
        }

        public bool LoginPlayer(string username, string password, PlayerConnection connection)
        {
            // Make sure this player isn't already in game.
            if (_players.Values.Any(player => player.Descriptor.Name == username))
            {
                var packet = new Packet();
                packet.Write("Account already logged in!");
                connection.SendPacket(PacketType.LOGIN_FAIL, packet, DeliveryMethod.ReliableOrdered);

                return false;
            }

            // If we've made it this far, we've confirmed that the requested account is not already logged into.
            // Let's make sure the password they provided us is valid.
            var playerDescriptor = _playerDataManager.Load(new PlayerDataArguments(username));

            if (playerDescriptor == null)
            {
                // The account doesn't exist!
                var packet = new Packet();
                packet.Write("Account does not exist!");
                connection.SendPacket(PacketType.LOGIN_FAIL, packet, DeliveryMethod.ReliableOrdered);

                return false;
            }

            // Check to see whether they were lying about that password...
            if (SecurePasswordHasher.Verify(password, playerDescriptor.Password))
            {
                // Whoa, they weren't lying!
                // Let's go ahead and grant them access.

                // First, we'll add them to the list of online players.
                var player = this.CreatePlayer(playerDescriptor, connection);
                this.AddPlayer(player);

                if (Settings.UserPermissions.ContainsKey(player.Descriptor.Name))
                    player.Descriptor.Role = Settings.UserPermissions[player.Descriptor.Name];

                // Now we'll go ahead and tell their client to make whatever preperations that it needs to.
                // We'll also tell them their super duper unique id.
                // The client learns its server-side identity here; peer ids are assigned independently
                // on each side of the connection, so it must never assume they match.
                var packet = new Packet();
                packet.Write(player.UniqueID);
                connection.SendPacket(PacketType.LOGIN_SUCCESS, packet, DeliveryMethod.ReliableOrdered);

                this.EventOccured?.Invoke(this, new SubjectEventArgs("playerLogin", new object[] { }));

                return true;
            }
            else
            {
                var packet = new Packet();
                packet.Write("Incorrect password!");
                connection.SendPacket(PacketType.LOGIN_FAIL, packet, DeliveryMethod.ReliableOrdered);

                return false;
            }
        }

        public bool RegisterPlayer(string username, string password, PlayerConnection connection)
        {
            password = SecurePasswordHasher.Hash(password);

            if (_playerDataManager.Exists(new PlayerDataArguments(username)))
            {
                var packet = new Packet();
                packet.Write("Account already exists!");
                connection.SendPacket(PacketType.LOGIN_FAIL, packet, DeliveryMethod.ReliableOrdered);

                return false;
            }

            // Create their player.
            var descriptor = PlayerModel.Create(username, password);
            descriptor.MapID = Settings.StartingMap;
            descriptor.Role = Settings.DefaultRole;
            var player = this.CreatePlayer(descriptor, connection);
            _playerDataManager.Save(player.Descriptor, null);

            this.AddPlayer(player);

            // Notify them that they successfully registered.
            var successPacket = new Packet();
            successPacket.Write(player.UniqueID);
            player.NetworkComponent.SendPacket(PacketType.REGISTER_SUCCESS, successPacket, DeliveryMethod.ReliableOrdered);

            return true;
        }

        public void Initalize()
        {
        }

        public event EventHandler<SubjectEventArgs> EventOccured;
    }
}