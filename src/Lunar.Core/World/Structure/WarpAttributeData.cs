using System;

namespace Lunar.Core.World.Structure
{
    [Serializable]
    public class WarpAttributeData : AttributeData
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string WarpMap { get; set; }

        public string LayerName { get; set; }

        public WarpAttributeData(int x, int y, string warpMap, string layerName)
        {
            this.X = x;
            this.Y = y;
            this.WarpMap = warpMap;
            this.LayerName = layerName;
        }
    }
}
