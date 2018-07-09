using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;

namespace Lunar.Server.Content.Graphics
{
    public class SpriteSheet
    {
        public Sprite Sprite { get; set; }

        public int HorizontalFrames { get; set; }

        public int VerticalFrames { get; set; }

        public int FrameWidth { get; set; }

        public int FrameHeight { get; set; }

        public SpriteSheet(Sprite sprite, int horizontalFrames, int verticalFrames, int frameWidth, int frameHeight)
        {
            this.Sprite = sprite;
            this.HorizontalFrames = horizontalFrames;
            this.VerticalFrames = verticalFrames;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
        }

        public NetBuffer Pack()
        {
            var netBuffer = new NetBuffer();
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write(this.HorizontalFrames);
            netBuffer.Write(this.VerticalFrames);
            netBuffer.Write(this.FrameWidth);
            netBuffer.Write(this.FrameHeight);
            return netBuffer;
        }
    }
}
