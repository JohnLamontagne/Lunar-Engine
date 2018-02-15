using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using QuakeConsole;
using Lunar.Client.Net;
using Lunar.Core.Net;

namespace Lunar.Client.Utilities
{
    public class CommandInterpreter : ICommandInterpreter
    {
        private static readonly string[] CommandAndArgumentSeparator = { " " };
        private static readonly string[] InstructionSeparator = { ";" };
        private const StringComparison StringComparisonMethod = StringComparison.OrdinalIgnoreCase;

        private readonly ManualInterpreter _manualInterpreter;

        public CommandInterpreter()
        {
            _manualInterpreter = new ManualInterpreter();

            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.AVAILABLE_COMMANDS, this.Handle_AvailableCommands);
        }

        private void Handle_AvailableCommands(PacketReceivedEventArgs args)
        {
            int commandCount = args.Message.ReadInt32();

            for (int i = 0; i < commandCount; i++)
            {
                string commandName = args.Message.ReadString();

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

            if (Client.ServiceLocator.GetService<NetHandler>().Connected)
            { 
                string[] instructions = input.Split(InstructionSeparator, StringSplitOptions.RemoveEmptyEntries);

                foreach (var instruction in instructions)
                {
                    string[] inputSplit = instruction.Trim().Split(CommandAndArgumentSeparator, StringSplitOptions.RemoveEmptyEntries);
                    if (inputSplit.Length == 0) return;

                    string command = inputSplit[0];
                    string[] commandArgs = inputSplit.Skip(1).ToArray();

                    var packet = new Packet(PacketType.CLIENT_COMMAND);
                    packet.Message.Write(command);
                    packet.Message.Write(commandArgs.Length);

                    foreach (var arg in commandArgs)
                        packet.Message.Write(arg);

                    Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
                }
            }
        }
    }
}
