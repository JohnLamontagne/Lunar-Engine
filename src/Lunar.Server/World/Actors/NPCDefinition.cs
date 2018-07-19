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
            this.InitalizeScripts(descriptor);
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
                    case "OnAcquired":
                        this.BehaviorDefinition.Attack = new ScriptFunction(args => script.GetFunction("Attack").Call(args));
                        break;

                    case "OnCreated":
                        this.BehaviorDefinition.OnCreated = new ScriptAction((args =>
                            {
                                script.GetFunction("OnCreated").Call(args);
                            }
                        ));
                        break;

                    case "OnDropped":
                        this.BehaviorDefinition.Attacked = new ScriptAction((args =>
                            {
                                script.GetFunction("Attacked").Call(args);
                            }
                        ));
                        break;

                    case "OnEquip":
                        this.BehaviorDefinition.OnDeath = new ScriptAction((args =>
                            {
                                script.GetFunction("OnDeath").Call(args);
                            }
                        ));
                        break;

                    case "OnUse":
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
