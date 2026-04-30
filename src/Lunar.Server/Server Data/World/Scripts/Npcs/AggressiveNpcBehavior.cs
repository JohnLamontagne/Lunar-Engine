using Lunar.Core;
using Lunar.Core.Utilities.Data;
using Lunar.Server.Scripting.Api;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;
using static Lunar.Core.EngineConstants;

[NpcBehavior("aggressive")]
public class AggressiveNpcBehavior : NpcBehavior
{
    public override void OnCreated(IActor actor)
    {
        var npc = (NPC)actor;
        npc.GameTimers.Register("randomWalkTmr", new GameTimer(500));
        npc.StateMachine.Start(new IdleState());
    }

    public override int Attack(IActor attacker, IActor target)
    {
        const int damage = 10;
        target.OnAttacked(attacker, damage);
        return damage;
    }

    // ---- States ----

    private class IdleState : IActorState<NPC>
    {
        public void OnEnter(NPC npc) { }
        public void OnExit(NPC npc) { }

        public IActorState<NPC> Update(GameTime gameTime, NPC npc)
        {
            var target = npc.FindPlayerTarget();
            if (target != null)
            {
                npc.Target = target;
                return new CombatState();
            }
            if (npc.GameTimers.Get("randomWalkTmr").Finished)
                return new WanderState();
            return this;
        }
    }

    private class CombatState : IActorState<NPC>
    {
        public void OnEnter(NPC npc)
        {
            npc.GameTimers.Register("attackTmr", new GameTimer(1000));
        }

        public void OnExit(NPC npc)
        {
            npc.GameTimers.Remove("attackTmr");
        }

        public IActorState<NPC> Update(GameTime gameTime, NPC npc)
        {
            if (npc.Target == null || !npc.Target.Alive || !npc.Target.Attackable)
                return new IdleState();

            if (npc.GameTimers.Get("attackTmr").Finished)
            {
                if (npc.WithinAttackingRangeOf(npc.Target))
                {
                    npc.Behavior?.Attack(npc, npc.Target);
                    npc.GameTimers.Get("attackTmr").Reset();
                }
                else
                {
                    npc.GoTo(npc.Target);
                    return new MovingState(this);
                }
            }
            return this;
        }
    }

    private class MovingState : IActorState<NPC>
    {
        private readonly IActorState<NPC> _returnState;
        public MovingState(IActorState<NPC> returnState) => _returnState = returnState;

        public void OnEnter(NPC npc) { }
        public void OnExit(NPC npc) { }

        public IActorState<NPC> Update(GameTime gameTime, NPC npc)
        {
            if (!npc.Moving)
                return _returnState;

            var target = npc.FindPlayerTarget();
            if (target != null)
            {
                npc.Target = target;
                return new CombatState();
            }
            return this;
        }
    }

    private class WanderState : IActorState<NPC>
    {
        private static readonly System.Random _rng = new System.Random();

        public void OnEnter(NPC npc)
        {
            npc.GameTimers.Get("randomWalkTmr").Reset();
        }

        public void OnExit(NPC npc) { }

        public IActorState<NPC> Update(GameTime gameTime, NPC npc)
        {
            if (!npc.GameTimers.Get("randomWalkTmr").Finished)
                return this;

            int dir = _rng.NextDouble() < 0.5 ? -1 : 1;
            float rx = (float)(_rng.NextDouble() * npc.MaxRoam.X * TILE_SIZE) * dir;
            float ry = (float)(_rng.NextDouble() * npc.MaxRoam.Y * TILE_SIZE) * dir;
            npc.GoTo(new Vector(npc.Position.X + rx, npc.Position.Y + ry));

            return new MovingState(new IdleState());
        }
    }
}
