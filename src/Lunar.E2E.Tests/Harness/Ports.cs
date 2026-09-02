using System.Net;
using System.Net.Sockets;

namespace Lunar.E2E.Tests.Harness
{
    public static class Ports
    {
        public static int FreeTcpPort()
        {
            using var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            return ((IPEndPoint)listener.LocalEndpoint).Port;
        }

        public static int FreeUdpPort()
        {
            using var socket = new UdpClient(new IPEndPoint(IPAddress.Loopback, 0));
            return ((IPEndPoint)socket.Client.LocalEndPoint).Port;
        }
    }
}
