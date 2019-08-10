using System;
using Lunar.Core.Content.Graphics;

namespace Lunar.Core.World.Structure.Attribute
{
    [Serializable]
    public class StartDialogueTileAttribute : TileAttribute
    {
        public override Color Color => new Color(255, 255, 100, 100);

        public string DialogueName { get; }
        public string BranchName { get; }

        public StartDialogueTileAttribute(string dialogueName, string branchName)
        {
            this.DialogueName = dialogueName;
            this.BranchName = branchName;
        }
    }
}