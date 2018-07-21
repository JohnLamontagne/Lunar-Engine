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
using Lunar.Core.Net;

namespace Lunar.Server.Net
{
    public class Packet
    {
        private static NetHandler _netHandler;

        private NetOutgoingMessage _message;

        public NetOutgoingMessage Message => _message;

        public ChannelType Channel { get; }

        /// <summary>
        /// Initalizes with a specified NetHandler object, enabling the creation of packets.
        /// </summary>
        /// <param name="netHandler"></param>
        public static void Initalize(NetHandler netHandler)
        {
            _netHandler = netHandler;
        }

        /// <summary>
        /// Must be initalized with a valid NetHandler object before packets can be created through new()
        /// </summary>
        /// <param name="packetType"></param>
        /// <param name="channel"></param>
        public Packet(PacketType packetType, ChannelType channel)
        {
            _message = _netHandler.ConstructMessage();
            _message.Write((short)packetType);

            this.Channel = channel;
        }

        /// <summary>
        /// Resets the packet so that it can be sent to another connection.
        /// </summary>
        public void Reset()
        {
            var saved = new byte[_message.LengthBytes];
            Buffer.BlockCopy(_message.Data, 0, saved, 0, _message.LengthBytes);

            _message = _netHandler.ConstructMessage();
            _message.Write(saved);
        }
    }
}