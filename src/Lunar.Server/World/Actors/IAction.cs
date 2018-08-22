using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Server.World.Actors
{
    public interface IAction<T> where T : IActor<IActorDescriptor>
    {
        void Execute(T actor);
    }
}
