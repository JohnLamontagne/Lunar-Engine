using System.Collections.Generic;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Actors.Components
{
    public class ActionProcessor<T> where T : IActor<IActorDescriptor>
    {
        private T Actor { get; }

        private Queue<IAction<T>> _actionQueue { get; }

        private long _nextActionTime;

        public ActionProcessor(T actor)
        {
            this.Actor = actor;

            _actionQueue = new Queue<IAction<T>>(Constants.MAX_QUEUED_ACTIONS);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalElapsedTime <= _nextActionTime)
            {
                var action = _actionQueue.Dequeue();

                action.Execute(this.Actor);

                _nextActionTime = gameTime.TotalElapsedTime + (1000 / Constants.ACTIONS_PER_SECOND);
            }
        }

        public void Process(IAction<T> action)
        {
            if (_actionQueue.Count <= Constants.MAX_QUEUED_ACTIONS)
            {
                _actionQueue.Enqueue(action);
            }
        }
    }
}
