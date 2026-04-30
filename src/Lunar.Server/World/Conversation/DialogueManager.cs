using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities.Scripting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Lunar.Server.World.Conversation
{
    public class DialogueManager : IService
    {
        private readonly Dictionary<string, Dialogue> _dialogues;
        private readonly DialogueFactory _dialogueFactory;
        private readonly Logger _logger;

        public DialogueManager(DialogueFactory dialogueFactory, Logger logger)
        {
            _dialogues = new Dictionary<string, Dialogue>();
            _dialogueFactory = dialogueFactory;
            _logger = logger;
        }

        public void Initalize()
        {
            this.LoadDialogues();
        }

        public Dialogue Get(string dialogueName)
        {
            _dialogues.TryGetValue(dialogueName, out Dialogue dialogue);
            return dialogue;
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
                    _logger.LogEvent($"Unable to load dialogue {file.Name} " +
                                    $"as dialogue named {dialogue.Name} already exists!", LogTypes.ERROR);
                    continue;
                }

                _dialogues.Add(dialogue.Name, dialogue);
            }

            Console.WriteLine($"Loaded {files.Length} dialogues.");
        }
    }
}