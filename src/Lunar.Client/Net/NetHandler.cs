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
using System;
using System.Collections.Generic;
using System.Diagnostics;
using DeliveryMethod = Lunar.Core.Net.DeliveryMethod;

namespace Lunar.Client.Net
{
    public class NetHandler : ISubject
    {
        private readonly NetManager _netManager;
        private readonly EventBasedNetListener _listener;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _packetHandlers;
        private readonly Queue<(PacketType type, Packet packet)> _packetQueue;
        private readonly Queue<(PacketType type, Packet packet)> _waitingPacketQueue;
        private readonly List<(PacketType type, Packet packet, DeliveryMethod method)> _packetCache;

        private NetPeer _peer;
        private PacketType? _waitingForPacketType;
        private bool _collectAndWaitFor;
        private PacketType? _collectAndWaitPacket;
        private Func<bool> _collectAndWaitFunc;

        public string UniqueID => _peer != null ? _peer.Id.ToString() : string.Empty;

        public bool Connected => _peer != null && _peer.ConnectionState == ConnectionState.Connected;

        public event EventHandler<SubjectEventArgs> EventOccured;
        public event EventHandler Disconnected;

        public NetHandler()
        {
            _packetHandlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();
            _packetQueue = new Queue<(PacketType, Packet)>();
            _waitingPacketQueue = new Queue<(PacketType, Packet)>();
            _packetCache = new List<(PacketType, Packet, DeliveryMethod)>();

            _listener = new EventBasedNetListener();
            _netManager = new NetManager(_listener)
            {
                AutoRecycle = true,
                UnconnectedMessagesEnabled = false
            };

            _listener.PeerConnectedEvent += peer => _peer = peer;
            _listener.PeerDisconnectedEvent += OnPeerDisconnected;
            _listener.NetworkReceiveEvent += OnNetworkReceive;
        }

        public void Connect()
        {
            _netManager.Start();
            string host = Debugger.IsAttached ? "localhost" : Settings.IP;
            _netManager.Connect(host, Settings.Port, Settings.GameName);
        }

        public void Disconnect()
        {
            _peer?.Disconnect();
            _netManager.Stop();
            _peer = null;
        }

        public void Initalize() { }

        /// <summary>
        /// Signals the NetHandler to collect packets and hold them until the specified one packet arrives, at which point
        /// it will process the collected packets in order.
        /// </summary>
        public void CollectAndWaitFor(PacketType packetType)
        {
            _collectAndWaitFor = true;
            _waitingForPacketType = packetType;
        }

        /// <summary>
        /// Collects packets of the specified type and waits until the specified result() evalutes to true, at which
        /// point it will process the filtered packets in order.
        /// </summary>
        public void CollectAndWait(PacketType packetType, Func<bool> when)
        {
            _collectAndWaitPacket = packetType;
            _collectAndWaitFunc = when;
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

        public void SendPacket(PacketType packetType, Packet packet, DeliveryMethod deliveryMethod)
        {
            if (!Connected)
            {
                _packetCache.Add((packetType, packet, deliveryMethod));
                return;
            }
            SendOnPeer(_peer, packetType, packet, deliveryMethod);
        }

        public void ProcessPacketQueue()
        {
            _netManager.PollEvents();

            if (_packetCache.Count > 0 && Connected)
            {
                foreach (var entry in _packetCache)
                    SendOnPeer(_peer, entry.type, entry.packet, entry.method);
                _packetCache.Clear();
            }

            if (_packetQueue.Count == 0)
                return;

            if (_collectAndWaitFor)
            {
                if (_collectAndWaitPacket != null && _collectAndWaitFunc != null && _collectAndWaitFunc())
                {
                    foreach (var queued in _waitingPacketQueue.ToArray())
                        _packetQueue.Enqueue(queued);
                    _waitingPacketQueue.Clear();
                    _collectAndWaitPacket = null;
                    _collectAndWaitFor = false;
                }
                else
                {
                    return;
                }
            }

            while (_packetQueue.Count > 0)
            {
                var entry = _packetQueue.Dequeue();
                if (_packetHandlers.TryGetValue(entry.type, out var handlers))
                {
                    foreach (var handler in handlers)
                    {
                        if (Settings.DisplayNetworkMessages)
                            Console.WriteLine("Handling packet {0} by {1}", entry.type, handler.Method);

                        entry.packet.Position = 0;
                        handler.Invoke(new PacketReceivedEventArgs(entry.type, entry.packet, null));
                    }
                }
                EventOccured?.Invoke(this, new SubjectEventArgs("packetRec" + entry.type, new object[] { entry.packet }));
            }
        }

        private void OnPeerDisconnected(NetPeer peer, DisconnectInfo info)
        {
            _peer = null;
            Disconnected?.Invoke(this, EventArgs.Empty);
        }

        private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, LiteNetLib.DeliveryMethod deliveryMethod)
        {
            if (reader.AvailableBytes < sizeof(short))
                return;

            var packetType = (PacketType)reader.GetShort();
            var payload = reader.GetRemainingBytes();
            var packet = new Packet(payload);

            if (_collectAndWaitFor && _waitingForPacketType == packetType)
            {
                _collectAndWaitFor = false;
                _waitingForPacketType = null;
            }

            if (_collectAndWaitPacket == packetType)
                _waitingPacketQueue.Enqueue((packetType, packet));
            else
                _packetQueue.Enqueue((packetType, packet));
        }

        private static void SendOnPeer(NetPeer peer, PacketType packetType, Packet packet, DeliveryMethod deliveryMethod)
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

        private static LiteNetLib.DeliveryMethod ToLiteNet(DeliveryMethod m) => m switch
        {
            DeliveryMethod.Unreliable => LiteNetLib.DeliveryMethod.Unreliable,
            DeliveryMethod.ReliableUnordered => LiteNetLib.DeliveryMethod.ReliableUnordered,
            DeliveryMethod.ReliableOrdered => LiteNetLib.DeliveryMethod.ReliableOrdered,
            DeliveryMethod.ReliableSequenced => LiteNetLib.DeliveryMethod.ReliableSequenced,
            DeliveryMethod.Sequenced => LiteNetLib.DeliveryMethod.Sequenced,
            _ => LiteNetLib.DeliveryMethod.ReliableOrdered
        };
    }
}
