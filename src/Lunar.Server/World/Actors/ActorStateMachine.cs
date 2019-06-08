using Lunar.Core.Utilities;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;
using System;

namespace Lunar.Server.World.Actors
{
    public class ActorStateMachine<T> where T : IActor<IActorDescriptor>
    {
        private bool _started;

        public T Actor { get; }

        public IActorState<T> CurrentState { get; private set; }

        public ActorStateMachine(T actor)
        {
            this.Actor = actor;
            _started = false;
        }

        public void Start(IActorState<T> state)
        {
            // The state machine should only be started once. This forces script developers to transition to new states through the return-based scene flow paradigm.
            if (_started)
            {
                Logger.LogEvent("Error: State Machine already started for Actor " + this.Actor.Descriptor.Name + " with behavior definition " + this.Actor.Behavior?.GetType().Name, LogTypes.ERROR, Environment.StackTrace);
                return;
            }

            this.CurrentState = state;
            _started = true;
        }

        public void Update(GameTime gameTime)
        {
            try
            {
                var nextState = this.CurrentState?.Update(gameTime, this.Actor);

                if (this.CurrentState != nextState)
                {
                    this.CurrentState.OnExit(this.Actor);
                    this.CurrentState = nextState;
                    this.CurrentState.OnEnter(this.Actor);
                }
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Error: " + ex.Message, LogTypes.ERROR, ex.StackTrace);
            }
        }
    }
}
