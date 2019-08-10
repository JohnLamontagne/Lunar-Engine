using Lunar.Core.Content.Graphics;
using System;

namespace Lunar.Core.World.Structure.Attribute
{
    [Serializable]
    public class BlockedTileAttribute : TileAttribute
    {
        public override Color Color => new Color(Color.Red, 100);
    }
}