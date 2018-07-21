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

namespace Lunar.Server.Net
{
    public class PacketReceivedEventArgs : EventArgs
    {
        private readonly NetIncomingMessage _message;
        private readonly PlayerConnection _connection;

        public NetIncomingMessage Message => _message;

        public PlayerConnection Connection => _connection;

        public PacketReceivedEventArgs(NetIncomingMessage message, PlayerConnection connection)
        {
            _message = message;
            _connection = connection;
        }
    }
}