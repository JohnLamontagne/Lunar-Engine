/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

namespace Lunar.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var server = new Server();
            server.Initalize();
            server.Start();
        }
    }
}