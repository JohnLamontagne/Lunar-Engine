using Lunar.Core.Content.Graphics;
using System.IO;

namespace Lunar.Core.World.Structure.Attribute
{
    public class PlayerSpawnTileAttribute : TileAttribute
    {
        internal const byte TYPE_ID = 4;

        public override Color Color => new Color(Color.Black, 100);

        protected override byte TypeId => TYPE_ID;

        protected override void WriteData(BinaryWriter writer)
        {
        }

        internal static PlayerSpawnTileAttribute ReadData(BinaryReader reader)
        {
            return new PlayerSpawnTileAttribute();
        }
    }
}
