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
using Lidgren.Network;
using Lunar.Server.Net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunar.Core.Net;
using Lunar.Core.Utilities;

namespace Lunar.Server.World.Actors
{
    public class PlayerManager : ISubject, IService
    {
        private readonly Dictionary<long, Player> _players;

        public PlayerManager()
        {
            _players = new Dictionary<long, Player>();
        }

        public Player GetPlayer(long uniqueID)
        {
            if (!_players.ContainsKey(uniqueID))
                return null;
            else
                return _players[uniqueID];
        }

      
        public Player GetPlayer(string name)
        {
            return _players.Values.FirstOrDefault(p => p.Name == name);
        }

        public void RemovePlayer(long uniqueID)
        {
            if (_players.ContainsKey(uniqueID))
                _players.Remove(uniqueID);
        }

        public bool LoginPlayer(string username, string password, PlayerConnection connection)
        {
            // Make sure this player isn't already in game.
            if (_players.Values.Any(player => player.Name == username))
            {
                var packet = new Packet(PacketType.LOGIN_FAIL, ChannelType.UNASSIGNED);
                packet.Message.Write("Account already logged in!");
                connection.SendPacket(packet, NetDeliveryMethod.Unreliable);
                connection.Disconnect("byeFelicia");

                return false;
            }

            if (File.Exists(Constants.FILEPATH_ACCOUNTS + username + ".acc"))
            {
                // If we've made it this far, we've confirmed that the requested account is not already logged into.
                // Let's make sure the password they provided us is valid.
                var playerDescriptor = PlayerDescriptor.Load(username);

                // Check to see whether they were lying about that password...
                if (playerDescriptor != null && password == playerDescriptor.Password)
                {
                    // Whoa, they weren't lying!
                    // Let's go ahead and grant them access.
                        

                    // First, we'll add them to the list of online players.
                    var player = new Player(playerDescriptor, connection);
                    _players.Add(player.UniqueID, player);

                    // Now we'll go ahead and tell their client to make whatever preperations that it needs to.
                    // We'll also tell them their super duper unique id.
                    var packet = new Packet(PacketType.LOGIN_SUCCESS, ChannelType.UNASSIGNED);
                    connection.SendPacket(packet, NetDeliveryMethod.Unreliable);

                    this.EventOccured?.Invoke(this, new SubjectEventArgs("playerLogin", new object[] { }));

                    return true;
                }
                else
                {
                    var packet = new Packet(PacketType.LOGIN_FAIL, ChannelType.UNASSIGNED);
                    packet.Message.Write("Incorrect password!");
                    connection.SendPacket(packet, NetDeliveryMethod.Unreliable);
                    connection.Disconnect("byeFelicia");

                    return false;
                }
            }
            else
            {
                var packet = new Packet(PacketType.LOGIN_FAIL, ChannelType.UNASSIGNED);
                packet.Message.Write("Account does not exist!");
                connection.SendPacket(packet, NetDeliveryMethod.Unreliable);
                connection.Disconnect("byeFelicia");

                return false;
            }
        }

        public bool RegisterPlayer(string username, string password, PlayerConnection connection)
        {
            // Make sure this player isn't already registered.
            if (File.Exists(Constants.FILEPATH_ACCOUNTS + username + ".acc"))
            {
                // Notify the requester that the specified username is already registered.
                var packet = new Packet(PacketType.REGISTRATION_FAIL, ChannelType.UNASSIGNED);
                packet.Message.Write("Account already registered!");
                connection.SendPacket(packet, NetDeliveryMethod.Unreliable);
                connection.Disconnect("byeFelicia");

                this.EventOccured?.Invoke(this, new SubjectEventArgs("playerRegister", new object[] { false }));

                return false;
            }

            // Create their player.
            var descriptor = PlayerDescriptor.Create(username, password);
            var player = new Player(descriptor, connection);
            player.Save();

            _players.Add(player.UniqueID, player);

            // Notify them that they successfully registered.
            var successPacket = new Packet(PacketType.REGISTER_SUCCESS, ChannelType.UNASSIGNED);
            player.SendPacket(successPacket, NetDeliveryMethod.Unreliable);

            return true;
        }

        public void Initalize()
        {
        }

        public event EventHandler<SubjectEventArgs> EventOccured;
    }
}