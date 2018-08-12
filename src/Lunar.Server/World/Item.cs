/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

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
using Lunar.Core.World;
using Lunar.Server.Content.Graphics;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World
{
    public class Item
    {
        private string _name;
        private Sprite _sprite;
        private bool _stackable;
        private EquipmentSlots _slotType;
        private ItemTypes _itemType;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defence;
        private int _health;
        private ItemBehaviorDefinition _behaviorDefinition;

        public string Name { get { return _name; } set { _name = value; } }

        public Sprite Sprite { get { return _sprite; } set { _sprite = value; } }

        public bool Stackable {  get { return _stackable; } set { _stackable = value; } }

        public ItemTypes ItemType { get { return _itemType; } set { _itemType = value; } }
        
        public EquipmentSlots SlotType { get { return _slotType; } set { _slotType = value; } }

        public int Strength { get { return _strength; } set { _strength = value; } }

        public int Intelligence { get { return _intelligence; } set { _intelligence = value; } }

        public int Dexterity { get { return _dexterity; } set { _dexterity = value; } }

        public int Defence { get { return _defence; } set { _defence = value; } }

        public int Health { get { return _health; } set { _health = value; } }

        public ItemBehaviorDefinition BehaviorDefinition { get { return _behaviorDefinition; } set { _behaviorDefinition = value; } }

        public Item(ItemDefinition itemDef)
        {
            if (itemDef == null)
            {
                Logger.LogEvent("Null item definition!", LogTypes.ERROR, Environment.StackTrace);

                this.Name = "Null";
                this.Sprite = new Sprite("nullItem");
                return;
            }

            this.Name = itemDef.Descriptor.Name;
            this.Sprite = new Sprite(itemDef.Descriptor.TexturePath);
            this.Stackable = itemDef.Descriptor.Stackable;
            this.ItemType = itemDef.Descriptor.ItemType;
            this.SlotType = itemDef.Descriptor.SlotType;
            this.Strength = itemDef.Descriptor.Strength;
            this.Intelligence = itemDef.Descriptor.Intelligence;
            this.Dexterity = itemDef.Descriptor.Dexterity;
            this.Defence = itemDef.Descriptor.Defence;
            this.Health = itemDef.Descriptor.Health;
            this.BehaviorDefinition = itemDef.BehaviorDefinition;

            itemDef.Descriptor.DefinitionChanged += ItemDescriptor_DefinitionChanged;

            this.BehaviorDefinition.OnCreated?.Invoke(new ScriptActionArgs(this));
        }

        private void ItemDescriptor_DefinitionChanged(object sender, System.EventArgs e)
        {
            this.BehaviorDefinition = ((ItemDefinition)sender).BehaviorDefinition;
        }

        public void OnUse(IActor user)
        {
            this.BehaviorDefinition?.OnUse?.Invoke(new ScriptActionArgs(this, user));
        }

        public void OnEquip(IActor user)
        {
            this.BehaviorDefinition?.OnEquip?.Invoke(new ScriptActionArgs(this, user));
        }
        
        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.Name);
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write((int)this.SlotType);

            return netBuffer;
        }
    }
}
