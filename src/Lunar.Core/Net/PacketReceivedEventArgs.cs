using System;

namespace Lunar.Core.Net
{
    public class PacketReceivedEventArgs : EventArgs
    {
        public PacketType PacketType { get; }
        public Packet Packet { get; }
        public IConnection Connection { get; }

        public PacketReceivedEventArgs(PacketType packetType, Packet packet, IConnection connection)
        {
            PacketType = packetType;
            Packet = packet;
            Connection = connection;
        }
    }
}
