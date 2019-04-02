/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
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
