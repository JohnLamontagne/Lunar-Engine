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

using LiteNetLib;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using DeliveryMethod = Lunar.Core.Net.DeliveryMethod;

namespace Lunar.Server.Net
{
    public class PlayerConnection : IConnection
    {
        private readonly NetPeer _peer;
        private readonly NetHandler _netHandler;
        private readonly Dictionary<Action<PacketReceivedEventArgs>, Action<PacketReceivedEventArgs>> _handlerFilters;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _handlers;

        public long UniqueIdentifier => _peer.Id;

        public Player Player { get; set; }

        private readonly Logger _logger;

        public PlayerConnection(NetPeer peer, NetHandler netHandler, Logger logger)
        {
            _peer = peer;
            _netHandler = netHandler;
            _logger = logger;
            _handlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();
            _handlerFilters = new Dictionary<Action<PacketReceivedEventArgs>, Action<PacketReceivedEventArgs>>();
        }

        public void AddPacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            Action<PacketReceivedEventArgs> filtered = args =>
            {
                if (args.Connection.UniqueIdentifier == this.UniqueIdentifier)
                    handler.Invoke(args);
            };

            if (!_handlers.TryGetValue(packetType, out var list))
            {
                list = new List<Action<PacketReceivedEventArgs>>();
                _handlers[packetType] = list;
            }
            list.Add(handler);

            _netHandler.AddPacketHandler(packetType, filtered);
            _handlerFilters[handler] = filtered;
        }

        public void RemovePacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            if (_handlerFilters.TryGetValue(handler, out var filtered))
            {
                _netHandler.RemovePacketHandler(packetType, filtered);
                _handlerFilters.Remove(handler);
            }
            if (_handlers.TryGetValue(packetType, out var list))
                list.Remove(handler);
        }

        public void SendPacket(PacketType packetType, Packet packet, DeliveryMethod deliveryMethod)
        {
            if (_peer == null || _peer.ConnectionState != ConnectionState.Connected)
            {
                _logger.LogEvent("Invalid player connection socket.", LogTypes.ERROR, new Exception("Invalid player connection socket."));
                return;
            }
            NetHandler.SendOnPeer(_peer, packetType, packet, deliveryMethod);
        }

        public void Disconnect(string reason = "")
        {
            if (_peer == null)
            {
                _logger.LogEvent("Invalid player connection socket.", LogTypes.ERROR, new Exception("Invalid player connection socket."));
                return;
            }
            _peer.Disconnect();
        }
    }
}
