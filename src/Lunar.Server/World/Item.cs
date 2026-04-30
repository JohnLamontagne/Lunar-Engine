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
using Lunar.Core.Content.Graphics;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;
using Lunar.Server.Scripting;
using Lunar.Server.Scripting.Api;
using Lunar.Server.World.Actors;
using Lunar.Core.Utilities;
using Lunar.Core;

namespace Lunar.Server.World
{
    public class Item
    {
        public ItemModel Descriptor { get; }

        public ItemBehavior Behavior { get; }

        public Item(ItemModel descriptor, ScriptHost scriptHost, Logger logger)
        {
            if (descriptor == null)
            {
                logger.LogEvent("Null item!", LogTypes.ERROR, new Exception("Null item"));
                Descriptor = new ItemModel()
                {
                    Name = "Null",
                    SpriteInfo = new SpriteInfo("nullItem")
                };
                return;
            }

            Descriptor = descriptor;

            scriptHost.TryCreateItemBehavior(descriptor.BehaviorKey, out var behavior);
            Behavior = behavior;

            Behavior?.OnCreated(this);
        }

        public void OnUse(IActor user)
        {
            Behavior?.OnUse(this, user);
        }

        public void OnEquip(IActor user)
        {
            Behavior?.OnEquip(this, user);
        }

        public Packet PackData()
        {
            var packet = new Packet();

            packet.Write(this.Descriptor.Name);
            packet.Write(this.Descriptor.SpriteInfo.TextureName);
            packet.Write((int)this.Descriptor.SlotType);

            return packet;
        }
    }
}