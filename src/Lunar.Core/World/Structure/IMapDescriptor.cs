using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;
using System.Collections.Generic;

namespace Lunar.Core.World.Structure
{
    public interface IMapDescriptor<out T> : IContentDescriptor where T : ILayerDescriptor<ITileDescriptor<SpriteInfo>>
    {
        string Name { get; set; }

        List<string> TilesetPaths { get; }

        T GetLayer(string name);

        IReadOnlyCollection<T> Layers { get; }

        bool Dark { get; set; }

        Vector Dimensions { get; }

        bool LayerExists(string name);

        void AddLayer(string name, ILayerDescriptor<ITileDescriptor<SpriteInfo>> layer);

        void RemoveLayer(string name);
    }
}