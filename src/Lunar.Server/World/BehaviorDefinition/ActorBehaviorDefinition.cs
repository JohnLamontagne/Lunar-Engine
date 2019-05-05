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

namespace Lunar.Server.World.BehaviorDefinition
{
    public class ActorBehaviorDefinition
    {
        /// <summary>
        /// Invoked upon actor death
        /// </summary>
        public Action<GameEventArgs> OnDeath { get; set; }

        /// <summary>
        ///  Attacks the specified actor, returning the amount of damage delt.
        /// </summary>
        public Func<GameEventArgs, int> Attack { get; set; }

        /// <summary>
        /// Invoked upon being attacked by the specified actor
        /// </summary>
        public Action<GameEventArgs> Attacked { get; set; }

        /// <summary>
        /// Invoked every actor Update() pass
        /// </summary>
        public Action<GameEventArgs> Update { get; set; }

        /// <summary>
        /// Invoked when the actor is created
        /// </summary>
        public Action<GameEventArgs> OnCreated { get; set; }

        public virtual event EventHandler<SubjectEventArgs> EventOccured;
    }
}