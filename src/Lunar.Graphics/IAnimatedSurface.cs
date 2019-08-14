using Lunar.Core.Content.Graphics;
using System;

namespace Lunar.Graphics
{
    public interface IAnimatedSurface
    {
        void PlayAnimation(BaseAnimation<IAnimationLayer<Sprite>> animation);
    }
}