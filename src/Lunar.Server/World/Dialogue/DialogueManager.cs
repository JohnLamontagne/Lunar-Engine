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
        private DialogueFactory _dialogueFactory;

        public DialogueManager()
        {
            _dialogues = new Dictionary<string, Dialogue>();
            _dialogueFactory = new DialogueFactory();
        }

        public void Initalize()
        {
            this.LoadDialogues();
        }

        private void LoadDialogues()
        {
            Console.WriteLine("Loading Dialogue...");

            var directoryInfo = new DirectoryInfo(Constants.FILEPATH_DIALOGUE);
            FileInfo[] files = directoryInfo.GetFiles("*" + EngineConstants.DIALOGUE_FILE_EXT);

            foreach (var file in files)
            {
                var dialogue = _dialogueFactory.LoadDialogue(file.FullName);

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
    }
}