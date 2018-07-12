using Lunar.Core.Content.Graphics;
using Lunar.Editor.Utilities;
using Lunar.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Editor.Content.Graphics
{
    public class Animation : 
        Lunar.Graphics.Effects.Animation
    {
        public Animation(AnimationDescription description) : 
            base(description)
        {
        }
    }
}
