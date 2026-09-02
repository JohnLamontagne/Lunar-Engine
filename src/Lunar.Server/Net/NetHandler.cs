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
using LiteNetLib.Utils;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities;
using System;
using System.Collections.Generic;
using DeliveryMethod = Lunar.Core.Net.DeliveryMethod;

namespace Lunar.Server.Net
{
    public class NetHandler
    {
        private readonly string _connectionKey;
        private readonly int _port;
        private readonly NetManager _netManager;
        private readonly EventBasedNetListener _listener;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _packetHandlers;
        private readonly Dictionary<int, PlayerConnection> _connections;

        public event EventHandler<ConnectionEventArgs> ConnectionReceived;
        public event EventHandler<ConnectionEventArgs> ConnectionLost;

        private readonly Logger _logger;

        public NetHandler(string gameName, int port, Logger logger)
        {
            _connectionKey = gameName;
            _port = port;
            _logger = logger;
            _packetHandlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();
            _connections = new Dictionary<int, PlayerConnection>();

            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener)
            {
                AutoRecycle = true,
                UnconnectedMessagesEnabled = false,
                BroadcastReceiveEnabled = false,
                DisconnectTimeout = 60_000
            };

            _listener.ConnectionRequestEvent += OnConnectionRequest;
            _listener.PeerConnectedEvent += OnPeerConnected;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
        }

        public void Start()
        {
            _netManager.Start(_port);
        }

        public void Stop()
        {
            _netManager.Stop();
        }

        public void Update(GameTime gameTime)
        {
            _netManager.PollEvents();
        }

        public void AddPacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            if (!_packetHandlers.TryGetValue(packetType, out var handlers))
            {
                handlers = new List<Action<PacketReceivedEventArgs>>();
                _packetHandlers[packetType] = handlers;
            }
            handlers.Add(handler);
        }

        public void RemovePacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            if (_packetHandlers.TryGetValue(packetType, out var handlers))
                handlers.Remove(handler);
        }

        internal static DeliveryMethod ToInternal(LiteNetLib.DeliveryMethod m) => m switch
        {
            LiteNetLib.DeliveryMethod.Unreliable => DeliveryMethod.Unreliable,
            LiteNetLib.DeliveryMethod.ReliableUnordered => DeliveryMethod.ReliableUnordered,
            LiteNetLib.DeliveryMethod.ReliableOrdered => DeliveryMethod.ReliableOrdered,
            LiteNetLib.DeliveryMethod.ReliableSequenced => DeliveryMethod.ReliableSequenced,
            LiteNetLib.DeliveryMethod.Sequenced => DeliveryMethod.Sequenced,
            _ => DeliveryMethod.ReliableOrdered
        };

        internal static LiteNetLib.DeliveryMethod ToLiteNet(DeliveryMethod m) => m switch
        {
            DeliveryMethod.Unreliable => LiteNetLib.DeliveryMethod.Unreliable,
            DeliveryMethod.ReliableUnordered => LiteNetLib.DeliveryMethod.ReliableUnordered,
            DeliveryMethod.ReliableOrdered => LiteNetLib.DeliveryMethod.ReliableOrdered,
            DeliveryMethod.ReliableSequenced => LiteNetLib.DeliveryMethod.ReliableSequenced,
            DeliveryMethod.Sequenced => LiteNetLib.DeliveryMethod.Sequenced,
            _ => LiteNetLib.DeliveryMethod.ReliableOrdered
        };

        internal static void SendOnPeer(NetPeer peer, PacketType packetType, Packet packet, DeliveryMethod deliveryMethod)
        {
            var writer = new NetDataWriter();
            writer.Put((short)packetType);
            if (packet != null)
            {
                var payload = packet.ToArray();
                if (payload.Length > 0)
                    writer.Put(payload);
            }
            peer.Send(writer, ToLiteNet(deliveryMethod));
        }

        public void Initalize()
        {
        }

        private void OnConnectionRequest(ConnectionRequest request)
        {
            request.AcceptIfKey(_connectionKey);
        }

        private void OnPeerConnected(NetPeer peer)
        {
            Console.WriteLine("Established connection with: {0}.", peer);
            var connection = new PlayerConnection(peer, this, _logger);
            _connections[peer.Id] = connection;
            this.ConnectionReceived?.Invoke(this, new ConnectionEventArgs(connection));
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine("Connection with {0} lost ({1}).", peer, disconnectInfo.Reason);
            if (_connections.TryGetValue(peer.Id, out var connection))
            {
                _connections.Remove(peer.Id);
                this.ConnectionLost?.Invoke(this, new ConnectionEventArgs(connection));
            }
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, LiteNetLib.DeliveryMethod deliveryMethod)
        {
            if (reader.AvailableBytes < sizeof(short))
                return;

            var packetType = (PacketType)reader.GetShort();
            var payload = reader.GetRemainingBytes();

            if (!_packetHandlers.TryGetValue(packetType, out var handlers) || handlers.Count == 0)
                return;

            if (!_connections.TryGetValue(peer.Id, out var connection))
                return;

            for (int i = 0; i < handlers.Count; i++)
            {
                using var packet = new Packet(payload);
                var args = new PacketReceivedEventArgs(packetType, packet, connection);

                try
                {
                    handlers[i].Invoke(args);
                }
                catch (Exception ex)
                {
                    // A faulty handler must never take the whole server down. Log it, drop the
                    // offending connection, and keep serving everyone else.
                    _logger.LogEvent($"Unhandled exception in {packetType} handler for peer {peer.Id}: {ex}", LogTypes.ERROR, ex);
                    Console.WriteLine($"Error handling {packetType} from peer {peer.Id}: {ex.GetType().Name}: {ex.Message}");
                    try { connection.Disconnect("serverError"); } catch { /* peer may already be gone */ }
                    return;
                }
            }
        }
    }
}
