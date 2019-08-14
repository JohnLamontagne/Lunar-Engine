using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Actors
{
    public interface IActorState<T> where T :IActor
    {
        void OnEnter(T actor);

        void OnExit(T actor);

        IActorState<T> Update(GameTime gameTime, T actor);
    }
}
