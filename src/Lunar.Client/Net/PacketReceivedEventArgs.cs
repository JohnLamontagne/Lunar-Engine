using System;
using Lidgren.Network;

namespace Lunar.Client.Net
{
    public class PacketReceivedEventArgs : EventArgs
    {
        private readonly NetIncomingMessage _message;

        public NetIncomingMessage Message { get { return _message; } }

        public PacketReceivedEventArgs(NetIncomingMessage message)
        {
            _message = message;
        }
    }
}