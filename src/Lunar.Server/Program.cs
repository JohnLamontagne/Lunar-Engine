/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.Runtime.InteropServices;

namespace Lunar.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server();
            server.Initalize();
            server.Start();

            // Ctrl+C in a terminal and SIGTERM from a container runtime or test harness both
            // stop the loops cleanly so the world is saved before exit.
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                Console.WriteLine("Shutdown requested (Ctrl+C)...");
                server.Stop();
            };

            using var sigterm = PosixSignalRegistration.Create(PosixSignal.SIGTERM, ctx =>
            {
                ctx.Cancel = true;
                Console.WriteLine("Shutdown requested (SIGTERM)...");
                server.Stop();
            });

            server.WaitForShutdown();
            Console.WriteLine("Server stopped.");
        }
    }
}
