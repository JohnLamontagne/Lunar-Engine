using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Lunar.Server.World.Conversation
{
    public class DialogueFactory
    {
        private readonly ScriptManager _scriptManager;
        private readonly NetHandler _netHandler;
        private readonly Logger _logger;

        public DialogueFactory(ScriptManager scriptManager, NetHandler netHandler, Logger logger)
        {
            _scriptManager = scriptManager;
            _netHandler = netHandler;
            _logger = logger;
        }

        private Dialogue NewDialogue(string name) => new Dialogue(name, _netHandler, _logger);

        public Dialogue Create(string filePath)
        {
            var dialogue = this.NewDialogue(Path.GetFileNameWithoutExtension(filePath));

            dialogue.AddBranch(new DialogueBranch(dialogue, "Branch1", "Enter your branch dialogue text here...", _logger));
            dialogue.Branches[0].AddResponse(new DialogueResponse() { Text = "Enter your response text here... " });

            this.Save(dialogue, filePath);

            return dialogue;
        }

        public void Save(Dialogue dialogue, string filePath)
        {
            var branches = new List<object>();
            foreach (var branch in dialogue.Branches)
            {
                var responses = new List<object>();
                foreach (var response in branch.Responses)
                {
                    var responseObj = new Dictionary<string, object>
                    {
                        { "text", response.Text }
                    };

                    if (!string.IsNullOrEmpty(response.Function))
                        responseObj["function"] = response.Function;

                    if (!string.IsNullOrEmpty(response.Next))
                        responseObj["next"] = response.Next;

                    if (!string.IsNullOrEmpty(response.Condition))
                        responseObj["condition"] = response.Condition;

                    responses.Add(responseObj);
                }

                var branchObj = new Dictionary<string, object>
                {
                    { "name", branch.Name },
                    { "text", branch.Text },
                    { "responses", responses }
                };

                branches.Add(branchObj);
            }

            var json = new Dictionary<string, object>
            {
                { "name", dialogue.Name },
                { "script", dialogue.ScriptPath },
                { "branches", branches }
            };

            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(json, options);
            File.WriteAllText(filePath, jsonString);
        }

        public Dialogue LoadDialogue(string filePath)
        {
            var jsonString = File.ReadAllText(filePath);
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            string dialogueName = root.GetProperty("name").GetString();
            Dialogue dialogue = this.NewDialogue(Path.GetFileNameWithoutExtension(filePath));

            string scriptPath = root.GetProperty("script").GetString();
            dialogue.ScriptPath = scriptPath;

            if (File.Exists(Constants.FILEPATH_DATA + "/" + scriptPath))
            {
                dialogue.Script = _scriptManager.CreateScript(Constants.FILEPATH_DATA + "/" + scriptPath);
            }

            foreach (var branchElement in root.GetProperty("branches").EnumerateArray())
            {
                string text = branchElement.GetProperty("text").GetString();
                string branchName = branchElement.GetProperty("name").GetString();
                var branch = new DialogueBranch(dialogue, branchName, text, _logger);

                foreach (var responseElement in branchElement.GetProperty("responses").EnumerateArray())
                {
                    var response = new DialogueResponse();

                    response.Text = responseElement.GetProperty("text").GetString();

                    if (responseElement.TryGetProperty("next", out var nextProp))
                        response.Next = nextProp.GetString();

                    if (responseElement.TryGetProperty("function", out var funcProp))
                        response.Function = funcProp.GetString();

                    if (responseElement.TryGetProperty("condition", out var condProp))
                        response.Condition = condProp.GetString();

                    branch.AddResponse(response);
                }

                dialogue.AddBranch(branch);
            }

            return dialogue;
        }
    }
}