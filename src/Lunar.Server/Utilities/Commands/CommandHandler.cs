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
using System.Collections.Generic;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.Scripting;
using Lunar.Server.Scripting.Api;
using Lunar.Server.World.Actors;

namespace Lunar.Server.Utilities.Commands
{
    public class CommandHandler
    {
        private readonly Dictionary<string, Action<CommandContext>> _handlers =
            new Dictionary<string, Action<CommandContext>>(StringComparer.OrdinalIgnoreCase);

        private readonly ScriptHost _scriptHost;
        private readonly PlayerManager _playerManager;
        private readonly Logger _logger;

        public CommandHandler(NetHandler netHandler, ScriptHost scriptHost, PlayerManager playerManager, Logger logger)
        {
            _scriptHost = scriptHost;
            _playerManager = playerManager;
            _logger = logger;

            netHandler.AddPacketHandler(PacketType.CLIENT_COMMAND, this.Handle_ClientCommand);

            // Built-in /reload command (admin only)
            _handlers["reload"] = ctx =>
            {
                if (ctx.Player.Descriptor.Role == null || ctx.Player.Descriptor.Role.Level < 1)
                {
                    ctx.Player.NetworkComponent.SendChatMessage("Permission denied.", ChatMessageType.Announcement);
                    return;
                }
                _scriptHost.Reload();
                ctx.Player.NetworkComponent.SendChatMessage("Scripts reloaded.", ChatMessageType.Announcement);
            };

            scriptHost.ReloadCompleted += (_, __) => this.LoadCommandScripts();
        }

        private void LoadCommandScripts()
        {
            // Remove all non-built-in handlers (everything except "reload")
            var builtIns = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "reload" };
            var toRemove = new List<string>();
            foreach (var key in _handlers.Keys)
            {
                if (!builtIns.Contains(key))
                    toRemove.Add(key);
            }
            foreach (var key in toRemove)
                _handlers.Remove(key);

            var registrar = new CommandRegistrar(_handlers);
            foreach (var type in _scriptHost.Registry.CommandScripts)
            {
                try
                {
                    var script = (CommandScript)Activator.CreateInstance(type);
                    script.Register(registrar);
                }
                catch (Exception ex)
                {
                    _logger.LogEvent($"Error registering command script {type.Name}: {ex.Message}", LogTypes.ERROR, ex);
                }
            }
        }

        private void Handle_ClientCommand(PacketReceivedEventArgs args)
        {
            string command = args.Packet.ReadString();

            int cArgsLength = args.Packet.ReadInt32();
            string[] cArgs = new string[cArgsLength];
            for (int i = 0; i < cArgsLength; i++)
                cArgs[i] = args.Packet.ReadString();

            if (_handlers.TryGetValue(command, out var handler))
            {
                var player = _playerManager.GetPlayer(args.Connection.UniqueIdentifier.ToString());
                if (player == null) return;

                try
                {
                    handler(new CommandContext(player, cArgs));
                }
                catch (Exception ex)
                {
                    _logger.LogEvent($"Error handling command '{command}': {ex.Message}", LogTypes.ERROR, ex);
                }
            }
        }

        public void Initalize()
        {
            this.LoadCommandScripts();
        }

        public Packet Pack()
        {
            var packet = new Packet();
            packet.Write(_handlers.Keys.Count);
            foreach (var command in _handlers.Keys)
                packet.Write(command);
            return packet;
        }

        private sealed class CommandRegistrar : ICommandRegistrar
        {
            private readonly Dictionary<string, Action<CommandContext>> _handlers;

            public CommandRegistrar(Dictionary<string, Action<CommandContext>> handlers)
            {
                _handlers = handlers;
            }

            public void Add(string command, Action<CommandContext> handler)
            {
                _handlers[command] = handler;
            }
        }
    }
}
