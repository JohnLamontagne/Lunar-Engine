using Lunar.Server.World.Actors;

namespace Lunar.Server.Scripting.Api
{
    public abstract class NpcBehavior : ActorBehavior
    {
        public virtual void OnSpawn(NPC npc) { }
    }
}
