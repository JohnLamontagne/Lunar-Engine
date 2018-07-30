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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Lidgren.Network;
using Lunar.Core.Net;
using Lunar.Core.Utilities;

namespace Lunar.Client.Net
{
    public class NetHandler : ISubject, IService
    {
        private readonly NetClient _client;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _packetHandlers;

        /// <summary>
        /// Used in the event of the connection not yet being established or connection lost.
        /// Packets will be sent upon establishment/reestablishment of the client-server connection.
        /// </summary>
        private readonly List<Tuple<NetOutgoingMessage, NetDeliveryMethod, ChannelType>> _packetCache;

        public long UniqueID => _client.UniqueIdentifier;

        public bool Connected => _client.ConnectionStatus == NetConnectionStatus.Connected;

        public NetHandler()
        {
            _packetHandlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();
            _packetCache = new List<Tuple<NetOutgoingMessage, NetDeliveryMethod, ChannelType>>();

            NetPeerConfiguration config = new NetPeerConfiguration(Settings.GameName);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _client = new NetClient(config);
            _client.Start();
        }

        public void Connect()
        {
            _client.Connect(Debugger.IsAttached ? "localhost" : Settings.IP, Settings.Port);

        }

        public void Disconnect()
        {
            _client.Disconnect("cleanLogout");
        }

        public void Update()
        {
            NetIncomingMessage message;
            while ((message = _client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // Get the packet type.
                        PacketType packetType = (PacketType)message.ReadInt16();
                        var resetTo = message.Position;

                        if (_packetHandlers.ContainsKey(packetType))
                        {
                            foreach (var handler in _packetHandlers[packetType])
                            {
                                if (Settings.DisplayNetworkMessages)
                                    Console.WriteLine("Handling packet {0} by {1}", packetType.ToString(), handler.Method.ToString());

                                handler.Invoke(new PacketReceivedEventArgs(message));
                                message.Position = sizeof(short) * 8;
                            }
                        }

                        this.EventOccured?.Invoke(this, new SubjectEventArgs("packetRec" + packetType.ToString(), new object[] { message }));

                        break;

                    case NetIncomingMessageType.DiscoveryResponse:
                        _client.Connect(message.SenderEndPoint);
                        break;

                    case NetIncomingMessageType.DebugMessage:
                        if (Settings.DisplayNetworkMessages)
                            Console.WriteLine(message.ReadString());
                        break;

                    case NetIncomingMessageType.WarningMessage:
                        Console.WriteLine(message.ReadString());
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        switch (message.SenderConnection.Status)
                        {
                            case NetConnectionStatus.Disconnected:
                                this.Disconnected?.Invoke(this, new EventArgs());
                                break;
                        }

                        break;
                }

                _client.Recycle(message);
            }

            if (_packetCache.Count > 0)
            {
                this.SendPacketCache();
            }
        }

        public void SendPacketCache()
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
            {
                foreach (var t in _packetCache)
                {
                    _client.SendMessage(t.Item1, t.Item2, (int)t.Item3);
                }

                _packetCache.Clear();
            }
        }

        public void SendMessage(NetOutgoingMessage message, NetDeliveryMethod method, ChannelType channelType)
        {
            if (_client.ConnectionStatus == NetConnectionStatus.Connected)
                _client.SendMessage(message, method, (int)channelType);
            else
            {
                // For some reason, the connection was lost or not established yet. Cache the packet to be sent later.
                _packetCache.Add(new Tuple<NetOutgoingMessage, NetDeliveryMethod, ChannelType>(message, method, channelType));
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
            _packetHandlers[packetType].Remove(handler);
        }

        public NetOutgoingMessage ConstructMessage()
        {
            return _client.CreateMessage();
        }

        public void Initalize()
        {
            throw new NotImplementedException();
        }

        public event EventHandler<SubjectEventArgs> EventOccured;

        public event EventHandler Disconnected;
    }
}