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

        public bool LoginPlayer(string username, string password, NetConnection netConnection)
        {
            // Make sure this player isn't already in game.
            if (_players.Values.Any(player => player.Name == username))
            {
                var packet = new Packet(PacketType.LOGIN_FAIL);
                packet.Message.Write("Account already logged in!");
                netConnection.SendMessage(packet.Message, NetDeliveryMethod.Unreliable, (int)ChannelType.UNASSIGNED);
                netConnection.Disconnect("byeFelicia");

                return false;
            }

            if (File.Exists(Constants.FILEPATH_ACCOUNTS + username + ".acc"))
            {
                // If we've made it this far, we've confirmed that the requested account is not already logged into.
                // Let's make sure the password they provided us is valid.
                var playerDescriptor = PlayerDescriptor.Load(username);

                // Check to see whether they were lying about that password...
                if (password == playerDescriptor.Password)
                {
                    // Whoa, they weren't lying!
                    // Let's go ahead and grant them access.

                    // Check whether they are an administrator (usernamed contained within the admin file)
                    if (File.ReadAllLines(Constants.FILEPATH_ACCOUNTS + "admins.txt").Contains(username))
                        playerDescriptor.Admin = true;
                        

                    // First, we'll add them to the list of online players.
                    _players.Add(netConnection.RemoteUniqueIdentifier, new Player(playerDescriptor, netConnection));

                    // Now we'll go ahead and tell their client to make whatever preperations that it needs to.
                    // We'll also tell them their super duper unique id.
                    var packet = new Packet(PacketType.LOGIN_SUCCESS);
                    netConnection.SendMessage(packet.Message, NetDeliveryMethod.Unreliable,
                        (int) ChannelType.UNASSIGNED);

                    this.EventOccured?.Invoke(this, new SubjectEventArgs("playerLogin", new object[] { }));

                    return true;
                }
                else
                {
                    var packet = new Packet(PacketType.LOGIN_FAIL);
                    packet.Message.Write("Incorrect password!");
                    netConnection.SendMessage(packet.Message, NetDeliveryMethod.Unreliable, (int)ChannelType.UNASSIGNED);
                    netConnection.Disconnect("byeFelicia");

                    return false;
                }
            }
            else
            {
                var packet = new Packet(PacketType.LOGIN_FAIL);
                packet.Message.Write("Account does not exist!");
                netConnection.SendMessage(packet.Message, NetDeliveryMethod.Unreliable, (int)ChannelType.UNASSIGNED);
                netConnection.Disconnect("byeFelicia");

                return false;
            }

            return false;
        }

        public bool RegisterPlayer(string username, string password, NetConnection netConnection)
        {
            // Make sure this player isn't already registered.
            if (File.Exists(Constants.FILEPATH_ACCOUNTS + username + ".acc"))
            {
                // Notify the requester that the specified username is already registered.
                var packet = new Packet(PacketType.REGISTRATION_FAIL);
                packet.Message.Write("Account already registered!");
                netConnection.SendMessage(packet.Message, NetDeliveryMethod.Unreliable, (int)ChannelType.UNASSIGNED);
                netConnection.Disconnect("byeFelicia");

                this.EventOccured?.Invoke(this, new SubjectEventArgs("playerRegister", new object[] { false }));

                return false;
            }

            // Create their player.
            var descriptor = PlayerDescriptor.Create(username, password);
            var player = new Player(descriptor, netConnection);
            player.Save();

            _players.Add(netConnection.RemoteUniqueIdentifier, player);

            // Notify them that they successfully registered.
            var successPacket = new Packet(PacketType.REGISTER_SUCCESS);
            netConnection.SendMessage(successPacket.Message, NetDeliveryMethod.Unreliable, (int)ChannelType.UNASSIGNED);

            return true;
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SubjectEventArgs> EventOccured;
    }
}