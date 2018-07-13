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