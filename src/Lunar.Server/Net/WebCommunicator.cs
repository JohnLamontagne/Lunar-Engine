using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Lunar.Server.Net
{
    public class WebCommunicator
    {
        public enum MessageTypes
        {
            Status_Updates = 0
        }

        public WebCommunicator()
        {
        }

        public void Run()
        {
            return;

            new Thread(() =>
            {
                int counter = 0;
                while (true)
                {
                    Thread.Sleep(50);
                    counter++;
                }
            }).Start();
        }

        public static void SendUDP(string hostNameOrAddress, int destinationPort, MessageTypes msgType, string data)
        {
            IPAddress destination = Dns.GetHostAddresses(hostNameOrAddress)[0];
            IPEndPoint endPoint = new IPEndPoint(destination, destinationPort);
            byte[] buffer = Encoding.ASCII.GetBytes(((int)msgType).ToString());
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            socket.SendTo(buffer, endPoint);

            socket.Close();
            System.Console.WriteLine("Sent: " + data);
        }
    }
}