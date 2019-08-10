using Lunar.Core.World.Structure.Attribute;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Structure.Attribute
{
    public class TileAttributeUpdateArgs : TileAttributeArgs
    {
        public GameTime GameTime { get; }

        public TileAttributeUpdateArgs(TileAttribute attribute, GameTime gameTime, Tile tile)
            : base(attribute, tile)
        {
            this.GameTime = gameTime;
        }
    }
}