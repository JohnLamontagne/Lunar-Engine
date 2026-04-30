using Lunar.Core.Content.Graphics;
using System.IO;

namespace Lunar.Core.World.Structure.Attribute
{
    public class BlockedTileAttribute : TileAttribute
    {
        internal const byte TYPE_ID = 1;

        public override Color Color => new Color(Color.Red, 100);

        protected override byte TypeId => TYPE_ID;

        protected override void WriteData(BinaryWriter writer)
        {
        }

        internal static BlockedTileAttribute ReadData(BinaryReader reader)
        {
            return new BlockedTileAttribute();
        }
    }
}
