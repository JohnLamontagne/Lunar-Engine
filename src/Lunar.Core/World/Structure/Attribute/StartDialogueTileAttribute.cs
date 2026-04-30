using Lunar.Core.Content.Graphics;
using System.IO;

namespace Lunar.Core.World.Structure.Attribute
{
    public class StartDialogueTileAttribute : TileAttribute
    {
        internal const byte TYPE_ID = 5;

        public override Color Color => new Color(255, 255, 100, 100);

        public string DialogueName { get; }
        public string BranchName { get; }

        public StartDialogueTileAttribute(string dialogueName, string branchName)
        {
            this.DialogueName = dialogueName;
            this.BranchName = branchName;
        }

        protected override byte TypeId => TYPE_ID;

        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(DialogueName ?? string.Empty);
            writer.Write(BranchName ?? string.Empty);
        }

        internal static StartDialogueTileAttribute ReadData(BinaryReader reader)
        {
            var dialogueName = reader.ReadString();
            var branchName = reader.ReadString();
            return new StartDialogueTileAttribute(dialogueName, branchName);
        }
    }
}
