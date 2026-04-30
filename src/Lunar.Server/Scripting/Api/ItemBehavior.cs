using Lunar.Server.World;
using Lunar.Server.World.Actors;

namespace Lunar.Server.Scripting.Api
{
    public abstract class ItemBehavior
    {
        public virtual void OnCreated(Item item) { }

        public virtual void OnAcquired(Item item, IActor user) { }

        public virtual void OnDropped(Item item, IActor user) { }

        public virtual void OnEquip(Item item, IActor user) { }

        public virtual void OnUse(Item item, IActor user) { }
    }
}
