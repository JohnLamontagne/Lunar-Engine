using Lunar.Core.Content.Graphics;
using System;

namespace Lunar.Core.World.Structure.Attribute
{
    [Serializable]
    public class PlayerSpawnTileAttribute : TileAttribute
    {
        public override Color Color => new Color(Color.Black, 100);
    }
}