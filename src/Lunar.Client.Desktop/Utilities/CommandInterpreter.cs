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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lunar.Client.Utilities
{
    public class CommandInterpreter
    {
        private static readonly string[] CommandAndArgumentSeparator = { " " };
        private static readonly string[] InstructionSeparator = { ";" };
        private static readonly string[] LocalCommands = { "help", "clear" };

        private readonly NetHandler _netHandler;
        private readonly HashSet<string> _availableCommands = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private List<string> _autocompleteMatches = new List<string>();
        private string _autocompletePrefix;
        private int _autocompleteIndex = -1;

        public CommandInterpreter(NetHandler netHandler)
        {
            _netHandler = netHandler;

            netHandler.AddPacketHandler(PacketType.AVAILABLE_COMMANDS, this.Handle_AvailableCommands);
        }

        private void Handle_AvailableCommands(PacketReceivedEventArgs args)
        {
            int commandCount = args.Packet.ReadInt32();

            for (int i = 0; i < commandCount; i++)
            {
                string commandName = args.Packet.ReadString();

                _availableCommands.Add(commandName);
            }
        }

        public string Autocomplete(string input, bool forward)
        {
            var prefix = (input ?? string.Empty).Trim();
            if (prefix.Contains(" "))
                return input;

            if (!string.Equals(_autocompletePrefix, prefix, StringComparison.OrdinalIgnoreCase))
            {
                var allCommands = new HashSet<string>(_availableCommands, StringComparer.OrdinalIgnoreCase);
                foreach (var localCommand in LocalCommands)
                    allCommands.Add(localCommand);

                _autocompleteMatches = allCommands
                    .Where(command => command.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                    .OrderBy(command => command, StringComparer.OrdinalIgnoreCase)
                    .ToList();

                _autocompletePrefix = prefix;
                _autocompleteIndex = -1;
            }

            if (_autocompleteMatches.Count == 0)
                return input;

            _autocompleteIndex = forward
                ? (_autocompleteIndex + 1) % _autocompleteMatches.Count
                : (_autocompleteIndex - 1 + _autocompleteMatches.Count) % _autocompleteMatches.Count;

            return _autocompleteMatches[_autocompleteIndex] + " ";
        }

        public bool Execute(string input, Action<string> appendLine)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            bool clearRequested = false;
            string[] instructions = input.Split(InstructionSeparator, StringSplitOptions.RemoveEmptyEntries);

            foreach (var instruction in instructions)
            {
                string[] inputSplit = instruction.Trim().Split(CommandAndArgumentSeparator, StringSplitOptions.RemoveEmptyEntries);
                if (inputSplit.Length == 0)
                    continue;

                string command = inputSplit[0];
                string[] commandArgs = inputSplit.Skip(1).ToArray();

                if (string.Equals(command, "clear", StringComparison.OrdinalIgnoreCase))
                {
                    clearRequested = true;
                    continue;
                }

                if (string.Equals(command, "help", StringComparison.OrdinalIgnoreCase))
                {
                    var knownCommands = _availableCommands
                        .Concat(LocalCommands)
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                        .ToArray();

                    appendLine($"Commands: {string.Join(", ", knownCommands)}");
                    continue;
                }

                if (!_netHandler.Connected)
                {
                    appendLine("Not connected to server.");
                    continue;
                }

                var packet = new Packet();
                packet.Write(command);
                packet.Write(commandArgs.Length);

                foreach (var arg in commandArgs)
                    packet.Write(arg);

                _netHandler.SendPacket(PacketType.CLIENT_COMMAND, packet, DeliveryMethod.ReliableOrdered);
            }

            return clearRequested;
        }
    }
}