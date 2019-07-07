using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Lunar.Server.World.Dialogue
{
    public class DialogueManager : IService
    {
        private Dictionary<string, Dialogue> _dialogues;

        public DialogueManager()
        {
            _dialogues = new Dictionary<string, Dialogue>();
        }

        public void Initalize()
        {
            this.LoadDialogues();
        }

        private void LoadDialogues()
        {
            Console.WriteLine("Loading Dialogue...");

            var directoryInfo = new DirectoryInfo(Constants.FILEPATH_DIALOGUE);
            FileInfo[] files = directoryInfo.GetFiles("*.xml");

            foreach (var file in files)
            {
                var dialogue = this.LoadDialogue(file.FullName);

                if (_dialogues.ContainsKey(dialogue.Name))
                {
                    Engine.Services.Get<Logger>().LogEvent($"Unable to load dialogue {file.Name} " +
                                    $"as dialogue named {dialogue.Name} already exists!", LogTypes.ERROR);
                    continue;
                }

                _dialogues.Add(dialogue.Name, dialogue);
            }

            Console.WriteLine($"Loaded {files.Length} dialogues.");
        }

        private Dialogue LoadDialogue(string filePath)
        {
            var doc = XDocument.Load(filePath);

            var dialogueNode = doc.Element("Dialogue");
            string dialogueName = dialogueNode.Attribute("name").Value.ToString();

            Dialogue dialogue = new Dialogue(dialogueName);

            string scriptPath = dialogueNode.Element("Script")?.Value;

            if (File.Exists(scriptPath))
            {
                dialogue.Script = Engine.Services.Get<ScriptManager>().CreateScript(scriptPath);
            }

            var branchNodes = dialogueNode.Elements("Branch");

            foreach (var branchNode in branchNodes)
            {
                string text = branchNode.Element("Text").Value;
                string branchName = branchNode.Attribute("name")?.Value;
                var branch = new DialogueBranch(dialogue, branchName, text);

                var responseNodes = branchNode.Elements("Response");

                foreach (var responseNode in responseNodes)
                {
                    var response = new DialogueResponse();

                    response.Text = responseNode.Value;
                    response.Next = responseNode.Attribute("next")?.Value;
                    response.Function = responseNode.Attribute("function")?.Value;
                    response.Condition = responseNode.Attribute("condition")?.Value;

                    branch.AddResponse(response);
                }

                dialogue.AddBranch(branch);
            }

            return dialogue;
        }
    }
}