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
using System;
using System.Collections.Generic;
using Lunar.Core.Net;

namespace Lunar.Server.Net
{
    public class NetHandler
    {
        private readonly NetServer _netServer;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _packetHandlers;
        private readonly Dictionary<long, PlayerConnection> _connections;

        public event EventHandler<ConnectionEventArgs> ConnectionReceived;

        public event EventHandler<ConnectionEventArgs> ConnectionLost;

        public NetHandler(string gameName, int port)
        {
            _packetHandlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();
            _connections = new Dictionary<long, PlayerConnection>();

            var config = new NetPeerConfiguration(gameName) { Port = port };
            config.DisableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            config.DisableMessageType(NetIncomingMessageType.Receipt);
            config.DisableMessageType(NetIncomingMessageType.UnconnectedData);
            config.DisableMessageType(NetIncomingMessageType.DiscoveryRequest);
            config.DisableMessageType(NetIncomingMessageType.DiscoveryResponse);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            config.AcceptIncomingConnections = true;
            config.ConnectionTimeout = 5;

            config.EnableUPnP = false;

            _netServer = new NetServer(config);
        }

        public void Update()
        {
            NetIncomingMessage message;
            while ((message = _netServer.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // Get the packet type.
                        var packetType = (PacketType)message.ReadInt16();

                        if (_packetHandlers.ContainsKey(packetType))
                        {
                            foreach (var handler in _packetHandlers[packetType])
                            {
                                handler.Invoke(new PacketReceivedEventArgs(message, _connections[message.SenderConnection.RemoteUniqueIdentifier]));
                                // Reset the read position.
                                message.Position = 0;
                                message.ReadInt16();
                            }
                        }

                        break;

                    case NetIncomingMessageType.ConnectionApproval:
                        message.SenderConnection.Approve();
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch (message.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Connected:
                                Console.WriteLine("Established connection with: {0}.", message.SenderEndPoint.ToString());
                                _connections.Add(message.SenderConnection.RemoteUniqueIdentifier, new PlayerConnection(message.SenderConnection, this));
                                this.ConnectionReceived?.Invoke(this, new ConnectionEventArgs(message.SenderConnection));
                                break;

                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine("Connection with {0} lost.", message.SenderEndPoint.ToString());
                                _connections.Remove(message.SenderConnection.RemoteUniqueIdentifier);
                                this.ConnectionLost?.Invoke(this, new ConnectionEventArgs(message.SenderConnection));
                                break;
                        }
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                }

                _netServer.Recycle(message);
            }
        }

        public void AddPacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            if (!_packetHandlers.ContainsKey(packetType))
                _packetHandlers.Add(packetType, new List<Action<PacketReceivedEventArgs>>());

            _packetHandlers[packetType].Add(handler);
        }

        public void RemovePacketHandler(PacketType packetType, Action<PacketReceivedEventArgs> handler)
        {
            _packetHandlers[packetType]?.Remove(handler);
        }

        public NetOutgoingMessage ConstructMessage()
        {
            return _netServer.CreateMessage();
        }

        public void Start()
        {
            _netServer.Start();
        }
    }
}