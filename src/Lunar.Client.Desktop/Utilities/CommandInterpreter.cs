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

using Lunar.Client.Net;
using Lunar.Core;
using Lunar.Core.Net;
using QuakeConsole;
using System;
using System.Linq;

namespace Lunar.Client.Utilities
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private static readonly string[] CommandAndArgumentSeparator = { " " };
        private static readonly string[] InstructionSeparator = { ";" };
        private const StringComparison StringComparisonMethod = StringComparison.OrdinalIgnoreCase;

        private readonly ManualInterpreter _manualInterpreter;
        private readonly NetHandler _netHandler;

        public CommandInterpreter(NetHandler netHandler)
        {
            _manualInterpreter = new ManualInterpreter();
            _netHandler = netHandler;

            netHandler.AddPacketHandler(PacketType.AVAILABLE_COMMANDS, this.Handle_AvailableCommands);
        }

        private void Handle_AvailableCommands(PacketReceivedEventArgs args)
        {
            int commandCount = args.Packet.ReadInt32();

            for (int i = 0; i < commandCount; i++)
            {
                string commandName = args.Packet.ReadString();

                _manualInterpreter.RegisterCommand(commandName, (delegate (string[] strings) { }));
            }
        }

        public void Autocomplete(IConsoleInput input, bool forward)
        {
            _manualInterpreter.Autocomplete(input, forward);
        }

        public void Execute(IConsoleOutput output, string input)
        {
            _manualInterpreter.Execute(output, input);

            if (_netHandler.Connected)
            {
                string[] instructions = input.Split(InstructionSeparator, StringSplitOptions.RemoveEmptyEntries);

                foreach (var instruction in instructions)
                {
                    string[] inputSplit = instruction.Trim().Split(CommandAndArgumentSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (inputSplit.Length == 0) return;

                    string command = inputSplit[0];
                    string[] commandArgs = inputSplit.Skip(1).ToArray();

                    var packet = new Packet();
                    packet.Write(command);
                    packet.Write(commandArgs.Length);

                    foreach (var arg in commandArgs)
                        packet.Write(arg);

                    _netHandler.SendPacket(PacketType.CLIENT_COMMAND, packet, DeliveryMethod.ReliableOrdered);
                }
            }
        }
    }
}