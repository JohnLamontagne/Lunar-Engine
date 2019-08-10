using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Structure
{
    public interface IBaseLayer<out T> where T : IBaseTile<SpriteInfo>
    {
        string Name { get; set; }

        int LayerIndex { get; set; }

        float ZIndex { get; }

        void Resize(Vector dimensions);

        T[,] Tiles { get; }
    }
}