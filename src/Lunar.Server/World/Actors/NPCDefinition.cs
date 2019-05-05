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

namespace Lunar.Server.World.Actors
{
    public class NPCDefinition
    {
        private ActorBehaviorDefinition _behaviorDefinition;

        public ActorBehaviorDefinition BehaviorDefinition => _behaviorDefinition;
        public NPCDescriptor Descriptor { get; }

        public NPCDefinition(NPCDescriptor descriptor)
        {
            this.Descriptor = descriptor;

            Script script = Server.ServiceLocator.Get<ScriptManager>().CreateScript(Constants.FILEPATH_SCRIPTS + "aggressive_npc.py"); 

            this.InitalizeDefaultBehavior();

            //this.InitalizeScripts(descriptor);
        }

        private void InitalizeDefaultBehavior()
        {
            _behaviorDefinition = new ActorBehaviorDefinition();

            Script script = Server.ServiceLocator.Get<ScriptManager>().CreateScript(Constants.FILEPATH_SCRIPTS + "aggressive_npc.py");

            this.BehaviorDefinition.Attack = new Func<GameEventArgs, int>((args)=> 
            {
                return script.Invoke<int>("attack", args);
            });

            this.BehaviorDefinition.Attacked = new Action<GameEventArgs>((args) =>
            {
                script.Invoke("attack", args);
            });

            this.BehaviorDefinition.OnCreated = new Action<GameEventArgs>((args) =>
            {
                script.Invoke("on_created", args);
            });

            
            this.BehaviorDefinition.OnDeath = new Action<GameEventArgs>((args) =>
            {
                script.Invoke("on_death", args);
            });

            this.BehaviorDefinition.OnDeath = new Action<GameEventArgs>((args) =>
            {
                script.Invoke("update", args);
            });
        }
    }
}
