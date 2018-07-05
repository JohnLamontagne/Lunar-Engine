/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lidgren.Network;
using System;
using System.Collections.Generic;
using Lunar.Core.Net;
using Lunar.Core.Utilities;

namespace Lunar.Server.Net
{
    public class NetHandler : IService
    {
        private readonly NetServer _netServer;
        private readonly Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>> _packetHandlers;

        public event EventHandler<ConnectionEventArgs> ConnectionReceived;

        public event EventHandler<ConnectionEventArgs> ConnectionLost;

        public NetHandler()
        {
            _packetHandlers = new Dictionary<PacketType, List<Action<PacketReceivedEventArgs>>>();

            var config = new NetPeerConfiguration(Settings.GameName) { Port = Settings.ServerPort };
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
                                handler.Invoke(new PacketReceivedEventArgs(message, message.SenderConnection));
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
                                this.ConnectionReceived?.Invoke(this, new ConnectionEventArgs(message.SenderConnection));
                                break;

                            case NetConnectionStatus.Disconnected:
                                Console.WriteLine("Connection with {0} lost.", message.SenderEndPoint.ToString());
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

        public void Initalize()
        {
           
        }
    }
}