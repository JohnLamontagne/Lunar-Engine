using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Structure
{
    public class MapObjectDescriptor
    {
        public Vector Position { get; set; }
        public SpriteInfo Sprite { get; set; }
        public bool Interactable { get; set; }
        public LayerDescriptor Layer { get; set; }
        public bool Animated { get; set; }
        public int FrameTime { get; set; }
        public LightInformation LightInformation { get; set; }
    }
}
