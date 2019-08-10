using Lunar.Core.World.Structure.Attribute;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure.Attribute
{
    public class TileAttributePlayerArgs : TileAttributeArgs
    {
        public Player Player { get; }

        public TileAttributePlayerArgs(TileAttribute attribute, Tile tile, Player player)
            : base(attribute, tile)
        {
            this.Player = player;
        }
    }
}