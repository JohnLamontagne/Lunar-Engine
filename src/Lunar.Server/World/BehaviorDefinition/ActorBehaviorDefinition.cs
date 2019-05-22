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
using Lunar.Server.Utilities.Scripting;
using System;
using Lunar.Core.Utilities;
using Lunar.Server.World.Actors;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.BehaviorDefinition
{
    public abstract class ActorBehaviorDefinition
    {

        /// <summary>
        /// Invoked upon actor death
        /// </summary>
        public abstract void OnDeath(IActor<IActorDescriptor> actor);

        /// <summary>
        ///  Attacks the specified actor, returning the amount of damage delt.
        /// </summary>
        public abstract int Attack(IActor<IActorDescriptor> attacker, IActor<IActorDescriptor> target);

        /// <summary>
        /// Invoked upon being attacked by the specified actor
        /// </summary>
        public abstract void Attacked(IActor<IActorDescriptor> attacked, IActor<IActorDescriptor> attacker, int damageDelt);

        /// <summary>
        /// Invoked every actor Update() pass
        /// </summary>
        public abstract void Update(IActor<IActorDescriptor> actor, GameTime gameTime);

        /// <summary>
        /// Invoked when the actor is created
        /// </summary>
        public abstract void OnCreated(IActor<IActorDescriptor> actor);

        public virtual event EventHandler<SubjectEventArgs> EventOccured;
    }
}