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
using System;
using Lidgren.Network;
using Lunar.Core.Content.Graphics;
using Lunar.Core.World;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using Lunar.Server.World.Actors;
using Lunar.Core.Utilities;

namespace Lunar.Server.World
{
    public class Item
    {
        public ItemDescriptor Descriptor { get; }

        public ItemBehaviorDefinition BehaviorDefinition { get; set; }

        public Item(ItemDescriptor descriptor)
        {
            if (descriptor == null)
            {
                Logger.LogEvent("Null item!", LogTypes.ERROR, new Exception("Null item"));

                Descriptor = new ItemDescriptor()
                {
                    Name = "Null",
                    SpriteInfo = new SpriteInfo("nullItem")
                };
                return;
            }

            this.InitalizeHooks();

            this.BehaviorDefinition.OnCreated?.Invoke(new GameEventArgs(this));
        }

        private void InitalizeHooks()
        {
            this.BehaviorDefinition = new ItemBehaviorDefinition();

            foreach (var pair in this.Descriptor.Scripts)
            {
                string scriptActionHook = pair.Key;
                string scriptContent = pair.Value;

                Script script = Server.ServiceLocator.Get<ScriptManager>().CreateScriptFromSource(scriptContent);

                switch (scriptActionHook)
                {
                    case "OnAcquired":
                        BehaviorDefinition.OnAcquired = new Action<GameEventArgs>((args) => 
                            {
                                script.Invoke("on_acquired", args);
                            }
                        );
                        break;

                    case "OnCreated":
                        BehaviorDefinition.OnCreated = new Action<GameEventArgs>((args) =>
                            {
                            script.Invoke("on_created", args);
                            }
                        );
                        break;

                    case "OnDropped":
                        BehaviorDefinition.OnDropped = new Action<GameEventArgs>((args) =>
                            {
                            script.Invoke("on_dropped", args);
                            }
                        );
                        break;

                    case "OnEquip":
                        BehaviorDefinition.OnEquip = new Action<GameEventArgs>((args) =>
                            {
                            script.Invoke("on_equip", args);
                            }
                        );
                        break;

                    case "OnUse":
                        BehaviorDefinition.OnUse = new Action<GameEventArgs>((args) =>
                            {
                                script.Invoke("on_use", args);
                            }
                        );
                        break;
                }
            }
        }

        public void OnUse(IActor<IActorDescriptor> user)
        {
            this.BehaviorDefinition?.OnUse?.Invoke(new GameEventArgs(this, user));
        }

        public void OnEquip(IActor<IActorDescriptor> user)
        {
            this.BehaviorDefinition?.OnEquip?.Invoke(new GameEventArgs(this, user));
        }
        
        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.Descriptor.Name);
            netBuffer.Write(this.Descriptor.SpriteInfo.TextureName);
            netBuffer.Write((int)this.Descriptor.SlotType);

            return netBuffer;
        }
    }
}
