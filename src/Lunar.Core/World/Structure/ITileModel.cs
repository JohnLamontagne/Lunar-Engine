using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure.Attribute;

namespace Lunar.Core.World.Structure
{
    public interface ITileModel<out T> where T : SpriteInfo
    {
        Vector Position { get; set; }

        bool Animated { get; set; }

        int FrameCount { get; set; }

        bool LightSource { get; set; }

        int LightRadius { get; set; }

        Color LightColor { get; set; }

        bool Teleporter { get; set; }

        bool Blocked { get; set; }

        TileAttribute Attribute { get; set; }

        T Sprite { get; }
    }
}