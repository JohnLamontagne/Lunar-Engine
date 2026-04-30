using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lunar.Server.World.Conversation
{
    public class DialogueBranch
    {
        private Dictionary<string, DialogueResponse> _responses;

        public string Text { get; set; }

        public string Name { get; }

        public Dialogue Dialogue { get; }

        public List<DialogueResponse> Responses => _responses.Values.ToList();

        private readonly Logger _logger;

        public DialogueBranch(Dialogue dialogue, string name, string text, Logger logger)
        {
            _responses = new Dictionary<string, DialogueResponse>();
            _logger = logger;

            this.Name = name;
            this.Text = text;
            this.Dialogue = dialogue;
        }

        public void AddResponse(DialogueResponse response)
        {
            _responses.Add(response.UniqueID.ToString(), response);
        }

        public void RemoveResponse(DialogueResponse response)
        {
            _responses.Remove(response.UniqueID.ToString());
        }

        public void OnResponse(string responseID, Player player)
        {
            if (_responses.ContainsKey(responseID))
            {
                var response = _responses[responseID];

                if (string.IsNullOrEmpty(response.Next) && string.IsNullOrEmpty(response.Function))
                {
                    this.End(player);
                    return;
                }

                if (response.IsScripted)
                {
                    InvokeScriptMethod(response.Function, this.Dialogue, player);
                }
                else
                {
                    this.Dialogue.Play(response.Next, player);
                }
            }
        }

        private void InvokeScriptMethod(string methodName, Dialogue dialogue, Player player)
        {
            var script = dialogue.Script;
            if (script == null) return;
            var method = script.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                _logger.LogEvent($"Dialogue script method '{methodName}' not found on {script.GetType().Name}.", LogTypes.ERROR);
                return;
            }
            try { method.Invoke(script, new object[] { dialogue, player }); }
            catch (Exception ex) { _logger.LogEvent($"Error invoking dialogue script method '{methodName}': {ex.Message}", LogTypes.ERROR, ex); }
        }

        private bool? InvokeScriptCondition(string methodName, Dialogue dialogue, Player player)
        {
            var script = dialogue.Script;
            if (script == null) return null;
            var method = script.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
            if (method == null)
            {
                _logger.LogEvent($"Dialogue script condition '{methodName}' not found on {script.GetType().Name}.", LogTypes.ERROR);
                return null;
            }
            try { return (bool)method.Invoke(script, new object[] { dialogue, player }); }
            catch (Exception ex)
            {
                _logger.LogEvent($"Error invoking dialogue script condition '{methodName}': {ex.Message}", LogTypes.ERROR, ex);
                return null;
            }
        }

        private void End(Player player)
        {
            this.Dialogue.End(player);
        }

        public void Begin(Player player)
        {
            var packet = new Packet();
            packet.Write(this.Name);
            packet.Write(this.Text);

            List<DialogueResponse> displayableResponses = new List<DialogueResponse>();
            // Determine which responses can be displayed by any existing conditions.
            foreach (var response in _responses.Values)
            {
                if (!string.IsNullOrEmpty(response.Condition))
                {
                    var displayable = InvokeScriptCondition(response.Condition, this.Dialogue, player);

                    if (!displayable.HasValue)
                    {
                        _logger.LogEvent($"Script for response {response.Text} in dialogue {this.Dialogue.Name} invalid!", LogTypes.ERROR);
                    }
                    else if (displayable.Value)
                    {
                        displayableResponses.Add(response);
                    }
                }
                else
                {
                    displayableResponses.Add(response);
                }
            }

            packet.Write(displayableResponses.Count);

            if (displayableResponses.Count <= 0)
            {
                packet.Write("...");
                packet.Write("");
            }
            else
            {
                foreach (var response in displayableResponses)
                {
                    packet.Write(response.Text);
                    packet.Write(response.UniqueID.ToString());
                }
            }

            player.NetworkComponent.SendPacket(PacketType.DIALOGUE, packet, DeliveryMethod.ReliableOrdered);
        }
    }
}
