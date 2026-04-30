using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Scripting;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Lunar.Server.World.Conversation
{
    public class DialogueFactory
    {
        private readonly ScriptHost _scriptHost;
        private readonly NetHandler _netHandler;
        private readonly Logger _logger;

        public DialogueFactory(ScriptHost scriptHost, NetHandler netHandler, Logger logger)
        {
            _scriptHost = scriptHost;
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
            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Dialogue",
                    new XAttribute("name", dialogue.Name),
                    new XElement("Script", dialogue.ScriptPath ?? string.Empty),
                    dialogue.Branches.Select(branch =>
                        new XElement("Branch",
                            new XAttribute("name", branch.Name),
                            new XElement("Text", branch.Text),
                            branch.Responses.Select(response =>
                            {
                                var elem = new XElement("Response", response.Text);
                                if (!string.IsNullOrEmpty(response.Function))
                                    elem.Add(new XAttribute("function", response.Function));
                                if (!string.IsNullOrEmpty(response.Next))
                                    elem.Add(new XAttribute("next", response.Next));
                                if (!string.IsNullOrEmpty(response.Condition))
                                    elem.Add(new XAttribute("condition", response.Condition));
                                return elem;
                            })
                        )
                    )
                )
            );

            doc.Save(filePath);
        }

        public Dialogue LoadDialogue(string filePath)
        {
            var doc = XDocument.Load(filePath);
            var root = doc.Root;

            Dialogue dialogue = this.NewDialogue(Path.GetFileNameWithoutExtension(filePath));

            string scriptPath = root.Element("Script")?.Value ?? string.Empty;
            dialogue.ScriptPath = scriptPath;
            dialogue.Script = _scriptHost.CreateDialogueScript(Path.GetFileNameWithoutExtension(filePath));

            foreach (var branchElement in root.Elements("Branch"))
            {
                string branchName = branchElement.Attribute("name")?.Value;
                string text = branchElement.Element("Text")?.Value ?? string.Empty;
                var branch = new DialogueBranch(dialogue, branchName, text, _logger);

                foreach (var responseElement in branchElement.Elements("Response"))
                {
                    var response = new DialogueResponse
                    {
                        Text = responseElement.Value,
                        Next = responseElement.Attribute("next")?.Value,
                        Function = responseElement.Attribute("function")?.Value,
                        Condition = responseElement.Attribute("condition")?.Value
                    };
                    branch.AddResponse(response);
                }

                dialogue.AddBranch(branch);
            }

            return dialogue;
        }
    }
}