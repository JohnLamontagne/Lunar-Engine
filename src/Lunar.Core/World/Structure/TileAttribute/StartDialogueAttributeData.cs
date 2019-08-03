using System;

namespace Lunar.Core.World.Structure.TileAttribute
{
    [Serializable]
    public class StartDialogueAttributeData : AttributeData
    {
        public string DialogueName { get; }
        public string BranchName { get; }

        public StartDialogueAttributeData(string dialogueName, string branchName)
        {
            this.DialogueName = dialogueName;
            this.BranchName = branchName;
        }
    }
}