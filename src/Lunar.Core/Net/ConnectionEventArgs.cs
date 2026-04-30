using System;

namespace Lunar.Core.Net
{
    public class ConnectionEventArgs : EventArgs
    {
        public IConnection Connection { get; }

        public ConnectionEventArgs(IConnection connection)
        {
            Connection = connection;
        }
    }
}
