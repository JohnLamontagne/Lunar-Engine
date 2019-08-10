using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;
using System.Collections.Generic;

namespace Lunar.Core.World.Structure
{
    public interface IBaseMap<out T> : IContentDescriptor where T : IBaseLayer<IBaseTile<SpriteInfo>>
    {
        string Name { get; set; }

        List<string> TilesetPaths { get; }

        T GetLayer(string name);

        IReadOnlyCollection<T> Layers { get; }

        bool Dark { get; set; }

        Vector Dimensions { get; }

        bool LayerExists(string name);

        void AddLayer(string name, IBaseLayer<IBaseTile<SpriteInfo>> layer);

        void RemoveLayer(string name);
    }
}