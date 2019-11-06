using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;
using System.Collections.Generic;

namespace Lunar.Core.World.Structure
{
    public interface IMapModel<out T> : IContentModel where T : ILayerModel<ITileModel<SpriteInfo>>
    {
        string Name { get; set; }

        List<string> TilesetPaths { get; }

        T GetLayer(string name);

        IReadOnlyCollection<T> Layers { get; }

        bool Dark { get; set; }

        Vector Dimensions { get; }

        bool LayerExists(string name);

        void AddLayer(string name, ILayerModel<ITileModel<SpriteInfo>> layer);

        void RemoveLayer(string name);
    }
}