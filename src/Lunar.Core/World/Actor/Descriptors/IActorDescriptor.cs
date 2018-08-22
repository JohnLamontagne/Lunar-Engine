using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Actor.Descriptors
{
    public interface IActorDescriptor
    {
        string Name { get; }

        float Speed { get; set; }

        int Level { get; set; }

        Stats Stats { get; }

        Stats StatBoosts { get; }

        Vector Position { get; set; }
    }
}
