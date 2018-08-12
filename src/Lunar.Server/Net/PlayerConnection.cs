using System;
using System.Collections.Generic;
using Lidgren.Network;
using Lunar.Core.Net;
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
                Logger.LogEvent($"Invalid player connection socket!!", LogTypes.ERROR, Environment.StackTrace);
                return;
            }

            _netConnection.SendMessage(packet.Message, method, (int)packet.Channel);
        }

        public void Disconnect(string byeMessage = "")
        {
            if (_netConnection == null)
            {
                Logger.LogEvent($"Invalid player connection socket at!", LogTypes.ERROR, Environment.StackTrace);
                return;
            }

            _netConnection.Disconnect(byeMessage);
        }
 
    }
}
