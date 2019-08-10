using Lunar.Core.World.Structure.Attribute;

namespace Lunar.Server.World.Structure.Attribute
{
    public class TileAttributeArgs : ITileAttributeArgs
    {
        public TileAttribute Attribute { get; }

        public Tile Tile { get; }

        public TileAttributeArgs(TileAttribute attribute, Tile tile)
        {
            this.Attribute = attribute;
            this.Tile = tile;
        }
    }
}