using System;
using Lunar.Core.Utilities;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;

namespace Lunar.Server.Scripting.Api
{
    public abstract class ActorBehavior
    {
        public virtual void OnCreated(IActor actor) { }

        public virtual void OnDeath(IActor actor) { }

        public virtual void Update(IActor actor, GameTime gameTime) { }

        public virtual int Attack(IActor attacker, IActor target) => 0;

        public virtual void Attacked(IActor attacked, IActor attacker, int damageDelt) { }

        public event EventHandler<SubjectEventArgs> EventOccured;

        protected void RaiseEvent(string name, params object[] args)
            => EventOccured?.Invoke(this, new SubjectEventArgs(name, args));
    }
}
