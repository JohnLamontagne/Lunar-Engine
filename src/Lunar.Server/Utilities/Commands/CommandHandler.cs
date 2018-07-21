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
using System;
using System.Collections.Generic;
using Lidgren.Network;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;

namespace Lunar.Server.Utilities.Commands
{
    public class CommandHandler : IService
    {
        private readonly Dictionary<string, List<ScriptAction>> _scriptedCommandHandlers;
        private Script _script;

        public CommandHandler(NetHandler netHandler)
        {
            netHandler.AddPacketHandler(PacketType.CLIENT_COMMAND, this.Handle_ClientCommand);

            _scriptedCommandHandlers = new Dictionary<string, List<ScriptAction>>();
        }

        public void AddHandler(string command, ScriptAction action)
        {
            if (!_scriptedCommandHandlers.ContainsKey(command))
                _scriptedCommandHandlers.Add(command, new List<ScriptAction>());

            _scriptedCommandHandlers[command].Add(action);
        }

        private void LoadScript()
        {
            _script = new Script(Constants.FILEPATH_SCRIPTS + "command_handler.lua");

            _script.ScriptChanged += Script_ScriptChanged;
        }

        private void Script_ScriptChanged(object sender, EventArgs e)
        {
            _scriptedCommandHandlers.Clear();

            _script.ReExecute();
        }

        private void Handle_ClientCommand(PacketReceivedEventArgs args)
        {
            string command = args.Message.ReadString();

            int cArgsLength = args.Message.ReadInt32();

            string[] cArgs = new string[cArgsLength];

            for (int i = 0; i < cArgsLength; i++)
            {
                cArgs[i] = args.Message.ReadString();
            }

            if (_scriptedCommandHandlers.ContainsKey(command))
            {
                // Get the player
                var player = Server.ServiceLocator.GetService<PlayerManager>().GetPlayer(args.Connection.UniqueIdentifier);

                _scriptedCommandHandlers[command].ForEach(a => a.Invoke(new ScriptActionArgs(this, player, cArgs)));
            }
        }

        public void Initalize()
        {
            this.LoadScript();
        }

        public NetBuffer Pack()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(_scriptedCommandHandlers.Keys.Count);
            foreach (var commmand in _scriptedCommandHandlers.Keys)
            {
                netBuffer.Write(commmand);
            }

            return netBuffer;
        }
    }
}
