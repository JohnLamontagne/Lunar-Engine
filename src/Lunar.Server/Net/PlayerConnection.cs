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
using System;
using System.Collections.Generic;
using Lidgren.Network;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities;

namespace Lunar.Server.Net
{
    public class PlayerConnection
    {
        private NetConnection _netConnection;
        private NetHandler _netHandler;
        private Dictionary<PacketType, Action<PacketReceivedEventArgs>> _handlers;

        public long UniqueIdentifier => _netConnection.RemoteUniqueIdentifier;

        public PlayerConnection(NetConnection netConnection, NetHandler netHandler)
        {
            _netConnection = netConnection;
            _netHandler = netHandler;
            _handlers = new Dictionary<PacketType, Action<PacketReceivedEventArgs>>();
        }

        public void AddPacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            Action<PacketReceivedEventArgs> mHandler = args => 
            {
                if (args.Connection.UniqueIdentifier == _netConnection.RemoteUniqueIdentifier)
                {
                    handler.Invoke(args);
                }
            };

            _handlers.Add(packetType, mHandler);

            _netHandler.AddPacketHandler(packetType, mHandler);
        }

        public void RemovePacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            _netHandler.RemovePacketHandler(packetType, _handlers[packetType]);
            _handlers.Remove(packetType);
        }

        public void SendPacket(Packet packet, NetDeliveryMethod method)
        {
            if (_netConnection == null)
            {
                Logger.LogEvent($"Invalid player connection socket.", LogTypes.ERROR, new Exception($"Invalid player connection socket."));
                return;
            }

            _netConnection.SendMessage(packet.Message, method, (int)packet.Channel);
        }

        public void Disconnect(string byeMessage = "")
        {
            if (_netConnection == null)
            {
                Logger.LogEvent($"Invalid player connection socket.", LogTypes.ERROR, new Exception($"Invalid player connection socket."));
                return;
            }

            _netConnection.Disconnect(byeMessage);
        }
 
    }
}
