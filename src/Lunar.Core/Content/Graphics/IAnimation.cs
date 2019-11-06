using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.Content.Graphics
{
    public interface IAnimation<out T> : IContentModel where T : IAnimationLayer<SpriteInfo>
    {
    }
}