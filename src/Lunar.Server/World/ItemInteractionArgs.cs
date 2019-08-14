using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World
{
    internal class ItemInteractionArgs : ItemArgs
    {
        public IActor User { get; }

        public ItemInteractionArgs(Item item, IActor user)
            : base(item)
        {
            this.User = user;
        }
    }
}