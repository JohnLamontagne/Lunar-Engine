using System.Collections.Generic;
using System.IO;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.World.Structure
{
    public class MapDescriptor : IDataDescriptor
    {
        private readonly Dictionary<string, LayerDescriptor> _layers;
        private List<string> _tilesetPaths;

        public string Name { get; set; }

        public Vector Dimensions { get; private set; }

        public Rect Bounds { get; set; }

        public bool Dark { get; set; }

        public Dictionary<string, LayerDescriptor> Layers => _layers;
        public IEnumerable<string> TilesetPaths => _tilesetPaths;

        private MapDescriptor()
        {
            _layers = new Dictionary<string, LayerDescriptor>();
            _tilesetPaths = new List<string>();
        }

        public MapDescriptor(Vector dimensions, string name)
        : this()
        {
            this.Dimensions = dimensions;
            this.Name = name;

            this.Bounds = new Rect(0, 0, this.Dimensions.X, this.Dimensions.Y);
        }
    }
}
