using Microsoft.Xna.Framework;

namespace Lunar.Client
{
    public interface IGameComponentContainer
    {
        void AddGameComponent(IGameComponent gameComponent);

        void RemoveGameComponent(IGameComponent gameComponent);

        IGameComponent GetGameComponent(string name);
    }
}