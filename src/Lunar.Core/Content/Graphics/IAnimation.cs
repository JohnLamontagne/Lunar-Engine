using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Content.Graphics
{
    public interface IAnimation<out T> : IContentDescriptor where T : IAnimationLayer<SpriteInfo>
    {
    }
}