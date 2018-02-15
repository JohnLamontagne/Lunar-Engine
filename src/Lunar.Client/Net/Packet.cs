using Lidgren.Network;
using Lunar.Core.Net;

namespace Lunar.Client.Net
{
    public class Packet
    {
        private NetOutgoingMessage _message;

        public NetOutgoingMessage Message { get { return _message; } }

        public Packet(PacketType packetType)
        {
            _message = Client.ServiceLocator.GetService<NetHandler>().ConstructMessage();
            _message.Write((short)packetType);
        }
    }
}