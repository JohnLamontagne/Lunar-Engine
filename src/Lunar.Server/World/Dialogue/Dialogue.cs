using Lunar.Server.Utilities.Scripting;
using System.Collections.Generic;
using Lunar.Server.Net;
using System;
using Lunar.Server.World.Actors;
using Lidgren.Network;
using Lunar.Core.Net;

namespace Lunar.Server.World.Dialogue
{
    public class Dialogue
    {
        private readonly Dictionary<string, ScriptAction> _scriptedResponses;
        private readonly Dictionary<string, Dialogue> _dialogueResponses;
        private  string _text;
        private string _uniqueID;
        private Player _player;

        public Dialogue(string text, Player player)
        {             
            _text = text;
            _player = player;
            _uniqueID = Guid.NewGuid().ToString();

            _scriptedResponses = new Dictionary<string, ScriptAction>();
            _dialogueResponses = new Dictionary<string, Dialogue>();

            Server.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.DIALOGUE_RESP, this.Handle_DialogueResponse);
        }

        public void AddResponse(string response, ScriptAction action)
        {
            _scriptedResponses.Add(response, action);
        }

        public void AddResponse(string response, Dialogue nextDialogue)
        {
            _dialogueResponses.Add(response, nextDialogue);
        }

        public void AddResponse(string response)
        {
            _dialogueResponses.Add(response, null);
        }

        public void Next(string text)
        {
            _text = text;
            this.Start();
        }

        private void Handle_DialogueResponse(PacketReceivedEventArgs args)
        {
            string uniqueID = args.Message.ReadString();

            if (_uniqueID != uniqueID)
                return;

            string response = args.Message.ReadString();

            if (_dialogueResponses.ContainsKey(response))
            {
                var next = _dialogueResponses[response];
                this.ClearResponses();

                if (next != null)
                    next.Start();
                else
                    this.End();
            }
            else if (_scriptedResponses.ContainsKey(response))
            {
                var next = _scriptedResponses[response];
                this.ClearResponses();
                next.Invoke(new ScriptActionArgs(this, _player));
            }
            else
            {
                this.End();
            }
        }

        public void ClearResponses()
        {
            _dialogueResponses.Clear();
            _scriptedResponses.Clear();
        }

        public void Start()
        {
            var packet = new Packet(PacketType.DIALOGUE);
            packet.Message.Write(_uniqueID);
            packet.Message.Write(_text);

            int responses = _scriptedResponses.Count + _dialogueResponses.Count;

            if (responses <= 0)
            {
                this.AddResponse("...");
                responses++;
            }

            packet.Message.Write(responses);

            foreach (var response in _scriptedResponses)
            {
                packet.Message.Write(response.Key);
            }

            foreach (var response in _dialogueResponses)
            {
                packet.Message.Write(response.Key);
            }

            _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
        }

        public void End()
        {
            var packet = new Packet(PacketType.DIALOGUE_END);
            _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
        }
    }
}
