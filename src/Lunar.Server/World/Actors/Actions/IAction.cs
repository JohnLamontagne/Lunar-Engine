
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Server.World.Actors.Actions
{
    public interface IAction<T> where T : IActor
    {
        void Execute(T actor);
    }
}
