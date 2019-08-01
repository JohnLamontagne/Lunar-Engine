using Lunar.Core;
using Lunar.Server.Utilities.Scripting;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Lunar.Server.World.Conversation
{
    public class DialogueFactory
    {
        public Dialogue Create(string filePath)
        {
            var dialogue = new Dialogue(Path.GetFileNameWithoutExtension(filePath));

            dialogue.AddBranch(new DialogueBranch(dialogue, "Branch1", "Enter your branch dialogue text here..."));
            dialogue.Branches[0].AddResponse(new DialogueResponse() { Text = "Enter your response text here... " });

            this.Save(dialogue, filePath);

            return dialogue;
        }

        public void Save(Dialogue dialogue, string filePath)
        {
            List<XElement> branches = new List<XElement>();
            foreach (var branch in dialogue.Branches)
            {
                List<XElement> responses = new List<XElement>();
                foreach (var response in branch.Responses)
                {
                    XElement rElement = new XElement("Response", response.Text);

                    if (!string.IsNullOrEmpty(response.Function))
                        rElement.SetAttributeValue("function", response.Function);

                    if (!string.IsNullOrEmpty(response.Next))
                        rElement.SetAttributeValue("next", response.Next);

                    if (!string.IsNullOrEmpty(response.Condition))
                        rElement.SetAttributeValue("condition", response.Condition);

                    responses.Add(rElement);
                }

                branches.Add(new XElement("Branch",
                                new XAttribute("name", branch.Name),
                                new XElement("Text", branch.Text),
                                responses.ToArray()
                    )
                );
            }

            var xml = new XElement("Dialogue",
                new XAttribute("name", dialogue.Name),
                new XElement("Script", dialogue.ScriptPath),
                branches.ToArray()
            );
            xml.Save(filePath);
        }

        public Dialogue LoadDialogue(string filePath)
        {
            var doc = XDocument.Load(filePath);

            var dialogueNode = doc.Element("Dialogue");
            string dialogueName = dialogueNode.Attribute("name").Value.ToString();

            Dialogue dialogue = new Dialogue(Path.GetFileNameWithoutExtension(filePath));

            string scriptPath = dialogueNode.Element("Script")?.Value;
            dialogue.ScriptPath = scriptPath;

            if (File.Exists(Constants.FILEPATH_DATA + "/" + scriptPath))
            {
                dialogue.Script = Engine.Services.Get<ScriptManager>().CreateScript(Constants.FILEPATH_DATA + "/" + scriptPath);
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