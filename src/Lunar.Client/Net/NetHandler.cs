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

using Lidgren.Network;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Lunar.Client.Net
{
    public class NetHandler : ISubject, IService
    {
        private readonly NetClient _client;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _packetHandlers;

        private Thread _incomingMessageThread;
        private bool _stop;

        private PacketType? _waitingForPacketType;

        /// <summary>
        /// Used to buffer packets for processing.
        /// </summary>
        private readonly Queue<Tuple<PacketType, PacketReceivedEventArgs>> _packetQueue;

        private readonly Queue<Tuple<PacketType, PacketReceivedEventArgs>> _waitingPacketQueue;

        /// <summary>
        /// Used in the event of the connection not yet being established or connection lost.
        /// Packets will be sent upon establishment/reestablishment of the client-server connection.
        /// </summary>
        private readonly List<Tuple<NetOutgoingMessage, NetDeliveryMethod, ChannelType>> _packetCache;

        public long UniqueID => _client.UniqueIdentifier;

        public bool Connected => _client.ConnectionStatus == NetConnectionStatus.Connected;

        /// <summary>
        /// When enabled, packets will be collected and stored but not executed until false again.
        /// </summary>
        private bool _collectAndWaitFor;

        private PacketType? _collectAndWaitPacket;
        private Func<bool> _collectAndWaitFunc;

        public NetHandler()
        {
            _packetHandlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();
            _packetCache = new List<Tuple<NetOutgoingMessage, NetDeliveryMethod, ChannelType>>();
            _packetQueue = new Queue<Tuple<PacketType, PacketReceivedEventArgs>>();
            _waitingPacketQueue = new Queue<Tuple<PacketType, PacketReceivedEventArgs>>();

            NetPeerConfiguration config = new NetPeerConfiguration(Settings.GameName);
            config.EnableMessageType(NetIncomingMessageType.Data);
            config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
            _client = new NetClient(config);
            _client.Start();

            this._collectAndWaitFor = false;
            _waitingForPacketType = null;
        }

        /// <summary>
        /// Signals the NetHandler to collect packets and hold them until the specified one packet arrives, at which point
        /// it will process the collected packets in order.
        /// </summary>
        /// <param name="packetType"></param>
        public void CollectAndWaitFor(PacketType packetType)
        {
            _collectAndWaitFor = true;
            _waitingForPacketType = packetType;
        }

        /// <summary>
        /// Collects packets of the specified type and waits until the specified result() evalutes to true, at which
        /// point it will process the filtered packets in order.
        /// </summary>
        /// <param name="packetType"></param>
        public void CollectAndWait(PacketType packetType, Func<bool> when)
        {
            _collectAndWaitPacket = packetType;
            _collectAndWaitFunc = when;
        }

        public void Connect()
        {
            _client.Connect(Debugger.IsAttached ? "localhost" : Settings.IP, Settings.Port);
            this.Start();
        }

        private void Start()
        {
            _stop = false;

            if (_incomingMessageThread != null)
                return;

            _incomingMessageThread = new Thread(() =>
            {
                while (!_stop)
                {
                    this.Update();
                }

                _incomingMessageThread = null;
            });

            _incomingMessageThread.Start();
        }

        public void Disconnect()
        {
            _client.Disconnect("cleanLogout");
            _stop = true;
            _incomingMessageThread = null;
        }

        public void ProcessPacketQueue()
        {
            if (_packetQueue.Count <= 0)
                return;

            if (_collectAndWaitFor)
            {
                if (_collectAndWaitPacket != null && _collectAndWaitFunc())
                {
                    foreach (var packet in _waitingPacketQueue.ToArray())
                    {
                        _packetQueue.Enqueue(packet);
                    }
                    _collectAndWaitPacket = null;
                    _collectAndWaitFor = false;
                }
                else
                {
                    return;
                }
            }

            do
            {
                var packet = _packetQueue.Dequeue();
                PacketType packetType = packet.Item1;
                PacketReceivedEventArgs args = packet.Item2;

                if (_packetHandlers.ContainsKey(packetType))
                {
                    foreach (var handler in _packetHandlers[packetType])
                    {
                        if (Settings.DisplayNetworkMessages)
                            Console.WriteLine("Handling packet {0} by {1}", packetType.ToString(), handler.Method.ToString());

                        handler.Invoke(args);
                    }
                }

                this.EventOccured?.Invoke(this, new SubjectEventArgs("packetRec" + packetType.ToString(), new object[] { args.Message }));
            } while (_packetQueue.Count > 0);
        }

        private void Update()
        {
            NetIncomingMessage message;
            while ((message = _client.ReadMessage()) != null)
            {
                switch (message.MessageType)
                {
                    case NetIncomingMessageType.Data:
                        // Get the packet type.
                        PacketType packetType = (PacketType)message.ReadInt16();

                        if (_collectAndWaitFor && _waitingForPacketType == packetType)
                        {
                            // The packet meets the criteria and we can break out of waiting.
                            _collectAndWaitFor = false;
                            _waitingForPacketType = null;
                        }

                        if (_collectAndWaitPacket == packetType)
                        {
                            _waitingPacketQueue.Enqueue(new Tuple<PacketType, PacketReceivedEventArgs>(packetType, new PacketReceivedEventArgs(message)));
                        }
                        else
                        {
                            _packetQueue.Enqueue(new Tuple<PacketType, PacketReceivedEventArgs>(packetType, new PacketReceivedEventArgs(message)));
                        }

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
                                _stop = true;
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