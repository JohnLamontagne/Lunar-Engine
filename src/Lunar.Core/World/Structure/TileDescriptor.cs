using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Structure
{
    public class TileDescriptor
    {
        public Vector Position { get; }

        public bool Animated { get; set; }

        public int FrameCount { get; set; }

        public bool LightSource { get; set; }

        public int LightRadius { get; set; }

        public Color LightColor { get; set; }

        public bool Teleporter { get; set; }

        public bool Blocked { get; set; }

        public TileAttributes Attribute { get; set; }

        public AttributeData AttributeData { get;  set; }

        public SpriteInfo SpriteInfo { get; set; }

        public TileDescriptor(SpriteInfo sprite)
        {
            this.SpriteInfo = sprite;

            this.Animated = false;
        }

        public TileDescriptor(Vector position)
        {
            this.Position = position;
        }

    }
}
