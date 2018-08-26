using System.Collections.Generic;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Structure
{
    public class LayerDescriptor
    {
        public string Name { get; private set; }

        public int LayerIndex { get; set; }

        public TileDescriptor[,] Tiles { get; }

        public LayerDescriptor(Vector dimensions, string layerName, int lIndex)
        {
            this.Tiles = new TileDescriptor[(int)dimensions.X, (int)dimensions.Y];

            this.Name = layerName;
            this.LayerIndex = lIndex;
        }

    }
}
