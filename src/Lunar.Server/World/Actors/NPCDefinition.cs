using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;

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

            Script script = new Script(Constants.FILEPATH_SCRIPTS + "aggressive_npc.lua");

            this.InitalizeDefaultBehavior();

            //this.InitalizeScripts(descriptor);
        }

        private void InitalizeDefaultBehavior()
        {
            _behaviorDefinition = new ActorBehaviorDefinition();

            Script script = new Script(Constants.FILEPATH_SCRIPTS + "aggressive_npc.lua");

            this.BehaviorDefinition.Attack = new ScriptFunction(args => script.GetFunction("Attack").Call(args));
            this.BehaviorDefinition.OnCreated = new ScriptAction((args => script.GetFunction("OnCreated").Call(args)));
            this.BehaviorDefinition.Attacked = new ScriptAction((args =>
                    {
                        script.GetFunction("Attacked").Call(args);
                    }
                ));
            this.BehaviorDefinition.OnDeath = new ScriptAction((args =>
                    {
                        script.GetFunction("OnDeath").Call(args);
                    }
                ));

            this.BehaviorDefinition.Update = new ScriptAction((args =>
                    {
                        script.GetFunction("Update").Call(args);
                    }
                ));
        }

        private void InitalizeScripts(NPCDescriptor descriptor)
        {
            _behaviorDefinition = new ActorBehaviorDefinition();
            foreach (var scriptDef in descriptor.Scripts)
            {
                string scriptActionHook = scriptDef.Key;
                string scriptContent = scriptDef.Value;

                Script script = new Script(scriptContent, false);

                switch (scriptActionHook)
                {
                    case "OnAttack":
                        this.BehaviorDefinition.Attack = new ScriptFunction(args => script.GetFunction("Attack").Call(args));
                        break;

                    case "OnCreated":
                        this.BehaviorDefinition.OnCreated = new ScriptAction((args =>
                            {
                                script.GetFunction("OnCreated").Call(args);
                            }
                        ));
                        break;

                    case "OnAttacked":
                        this.BehaviorDefinition.Attacked = new ScriptAction((args =>
                            {
                                script.GetFunction("Attacked").Call(args);
                            }
                        ));
                        break;

                    case "OnDeath":
                        this.BehaviorDefinition.OnDeath = new ScriptAction((args =>
                            {
                                script.GetFunction("OnDeath").Call(args);
                            }
                        ));
                        break;

                    case "OnUpdate":
                        this.BehaviorDefinition.Update = new ScriptAction((args =>
                            {
                                script.GetFunction("Update").Call(args);
                            }
                        ));
                        break;
                }
            }

        }
    }
}
