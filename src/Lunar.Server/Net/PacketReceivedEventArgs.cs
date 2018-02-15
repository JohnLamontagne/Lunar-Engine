/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lidgren.Network;
using System;

namespace Lunar.Server.Net
{
    public class PacketReceivedEventArgs : EventArgs
    {
        private readonly NetIncomingMessage _message;
        private readonly NetConnection _connection;

        public NetIncomingMessage Message { get { return _message; } }

        public NetConnection Connection { get { return _connection; } }

        public PacketReceivedEventArgs(NetIncomingMessage message, NetConnection connection)
        {
            _message = message;
            _connection = connection;
        }
    }
}