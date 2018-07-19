using Lunar.Core.World;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;

namespace Lunar.Server.World
{
    public class ItemDefinition
    {
        private ItemBehaviorDefinition _behaviorDefinition;

        public ItemBehaviorDefinition BehaviorDefinition => _behaviorDefinition;

        public ItemDescriptor Descriptor { get; }

        public ItemDefinition(ItemDescriptor descriptor)
        {
            Descriptor = descriptor;

            this.InitalizeHooks();
        }

        private void InitalizeHooks()
        {
            foreach (var pair in this.Descriptor.Scripts)
            {
                string scriptActionHook = pair.Key;
                string scriptContent = pair.Value;

                Script script = new Script(scriptContent, false);

                switch (scriptActionHook)
                {
                    case "OnAcquired":
                        BehaviorDefinition.OnAcquired = new ScriptAction((args =>
                            {
                                script.GetFunction("OnAcquired").Call(args);
                            }
                        ));
                        break;

                    case "OnCreated":
                        BehaviorDefinition.OnCreated = new ScriptAction((args =>
                            {
                                script.GetFunction("OnCreated").Call(args);
                            }
                        ));
                        break;

                    case "OnDropped":
                        BehaviorDefinition.OnDropped = new ScriptAction((args =>
                            {
                                script.GetFunction("OnDropped").Call(args);
                            }
                        ));
                        break;

                    case "OnEquip":
                        BehaviorDefinition.OnEquip = new ScriptAction((args =>
                            {
                                script.GetFunction("OnEquip").Call(args);
                            }
                        ));
                        break;

                    case "OnUse":
                        BehaviorDefinition.OnUse = new ScriptAction((args =>
                            {
                                script.GetFunction("OnUse").Call(args);
                            }
                        ));
                        break;
                }
            }
        }
    }
}
