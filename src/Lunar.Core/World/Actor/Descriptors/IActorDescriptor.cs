using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.World.Actor.Descriptors
{
    public interface IActorDescriptor : IDataDescriptor
    {
        string Name { get; }

        float Speed { get; set; }

        int Level { get; set; }

        Stats Stats { get; }

        Stats StatBoosts { get; }

        Vector Position { get; set; }
    }
}
