using Lidgren.Network;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;

namespace Lunar.Server.World.Dialogue
{
    public class DialogueBranch
    {
        private string _uniqueID;
        private Dialogue _dialogue;
        private Script _script;

        private Dictionary<string, DialogueResponse> _responses;

        public event EventHandler Ended;

        public string Text { get; }

        public string Name { get; }

        public DialogueBranch(Dialogue dialogue, string name, string text)
        {
            _responses = new Dictionary<string, DialogueResponse>();
            _dialogue = dialogue;

            this.Name = name;
            this.Text = text;
        }

        public void AddResponse(DialogueResponse response)
        {
            _responses.Add(response.Text, response);
        }

        private void Handle_DialogueResponse(PacketReceivedEventArgs args)
        {
            string responseName = args.Message.ReadString();
            var player = args.Connection.Player;

            if (_responses.ContainsKey(responseName))
            {
                var response = _responses[responseName];

                if (string.IsNullOrEmpty(response.Next) && string.IsNullOrEmpty(response.Function))
                {
                    this.End(player);
                }

                if (response.IsScripted)
                {
                    _script.Invoke(response.Function, new DialogueArgs(_dialogue, player));
                }
                else
                {
                    _dialogue.Play(response.Next);
                }
            }
            else
            {
                this.End(player);
            }
        }

        public void Begin(Player player)
        {
            var packet = new Packet(PacketType.DIALOGUE, ChannelType.UNASSIGNED);
            packet.Message.Write(_uniqueID);
            packet.Message.Write(this.Text);

            List<string> displayableResponses = new List<string>();
            // Determine which responses can be displayed by any existing conditions.
            foreach (var response in _responses.Values)
            {
                if (!string.IsNullOrEmpty(response.Condition))
                {
                    var displayable = _dialogue.Script?.Invoke<bool>(response.Condition, new DialogueArgs(_dialogue, player));

                    if (!displayable.HasValue)
                    {
                        Engine.Services.Get<Logger>().LogEvent($"Script for response {response.Text} in dialogue {_dialogue.Name} invalid!", LogTypes.ERROR);
                    }
                    else if (displayable.Value)
                    {
                        displayableResponses.Add(response.Text);
                    }
                }
                else
                {
                    displayableResponses.Add(response.Text);
                }
            }

            packet.Message.Write(displayableResponses.Count);

            if (displayableResponses.Count <= 0)
            {
                packet.Message.Write("...");
            }
            else
            {
                foreach (var response in displayableResponses)
                    packet.Message.Write(response);
            }

            player.NetworkComponent.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.DIALOGUE_RESP, this.Handle_DialogueResponse);
        }

        public void End(Player player)
        {
            var packet = new Packet(PacketType.DIALOGUE_END, ChannelType.UNASSIGNED);
            packet.Message.Write(_uniqueID);
            player.NetworkComponent.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
            player.NetworkComponent.Connection.RemovePacketHandler(PacketType.DIALOGUE_RESP, this.Handle_DialogueResponse);

            this.Ended?.Invoke(this, new EventArgs());
        }
    }
}