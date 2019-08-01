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

using Lunar.Core;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using System;
using System.Collections.Generic;

namespace Lunar.Server.World.Actors
{
    /// <summary>
    /// Defines world a unique world NPC based off of underlying descriptor
    /// </summary>
    public class NPCDefinition : NPCDescriptor
    {
        public ActorBehaviorDefinition BehaviorDefinition { get; set; }

        private List<Script> _scripts;

        public NPCDefinition(NPCDescriptor descriptor)
        {
            _scripts = new List<Script>();

            this.Name = descriptor.Name;
            this.Level = descriptor.Level;
            this.MaxRoam = descriptor.MaxRoam;
            this.Position = descriptor.Position;
            this.FrameSize = descriptor.FrameSize;

            this.AggresiveRange = descriptor.AggresiveRange;
            this.Reach = descriptor.Reach;
            this.Speed = descriptor.Speed;
            this.TexturePath = descriptor.TexturePath;
            this.Stats.Defense = descriptor.Stats.Defense;
            this.Stats.Dexterity = descriptor.Stats.Dexterity;
            this.Stats.Health = descriptor.Stats.Health;
            this.Stats.MaximumHealth = descriptor.Stats.MaximumHealth;
            this.Stats.Strength = descriptor.Stats.Strength;
            this.Stats.Intelligence = descriptor.Stats.Intelligence;
            this.CollisionBounds = descriptor.CollisionBounds;
            this.Dialogue = descriptor.Dialogue;
            this.DialogueBranch = descriptor.DialogueBranch;

            this.InitalizeScripts(descriptor.Scripts);
            this.InitalizeDefaultBehavior();
        }

        private void InitalizeScripts(IEnumerable<string> scriptPaths)
        {
            foreach (var scriptPath in scriptPaths)
            {
                Script script = Engine.Services.Get<ScriptManager>().CreateScript(Constants.FILEPATH_DATA + scriptPath);
                ActorBehaviorDefinition behaviorDefinition = script?.GetVariable<ActorBehaviorDefinition>("BehaviorDefinition");

                if (behaviorDefinition != null)
                {
                    this.BehaviorDefinition = behaviorDefinition;
                }
            }
        }

        private void InitalizeDefaultBehavior()
        {
        }
    }
}