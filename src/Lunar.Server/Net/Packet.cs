/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lidgren.Network;
using System;
using Lunar.Core.Net;

namespace Lunar.Server.Net
{
    public class Packet
    {
        private NetOutgoingMessage _message;

        public NetOutgoingMessage Message {get { return _message; } }

        public Packet(PacketType packetType)
        {
            _message = Server.ServiceLocator.GetService<NetHandler>().ConstructMessage();
            _message.Write((short)packetType);
        }

        /// <summary>
        /// Resets the packet so that it can be sent to another connection.
        /// </summary>
        public void Reset()
        {
            var saved = new byte[_message.LengthBytes];
            Buffer.BlockCopy(_message.Data, 0, saved, 0, _message.LengthBytes);
            var savedBitLength = _message.LengthBits;

            _message = Server.ServiceLocator.GetService<NetHandler>().ConstructMessage();
            _message.Write(saved);
        }
    }
}