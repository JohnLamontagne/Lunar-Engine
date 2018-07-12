using System.IO;

namespace Lunar.Core.Content.Graphics
{
    public class AnimationLayerDefinition
    {
        /// <summary>
        /// The width of each frame.
        /// </summary>
        public int FrameWidth { get; set; }

        /// <summary>
        /// The height of each frame.
        /// </summary>
        public int FrameHeight { get; set; }

        /// <summary>
        /// The amount of time each frame will last on screen.
        /// </summary>
        public int FrameTime { get; set; }

        /// <summary>
        /// Whether the animation will reset & continue playing after completion.
        /// </summary>
        public int LoopCount { get; set; }

        /// <summary>
        /// The animation's sprite.
        /// </summary>
        public string TexturePath { get; set; }

        internal AnimationLayerDefinition()
        {

        }
    }
}
