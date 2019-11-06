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
using System.Collections.Generic;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.World
{
    public class ItemModel : IContentModel
    {
        private string _name;
        private SpriteInfo _spriteInfo;
        private bool _stackable;
        private EquipmentSlots _slotType;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defence;
        private int _health;
        private ItemTypes _itemType;
        private Dictionary<string, string> _scripts;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public SpriteInfo SpriteInfo
        {
            get => _spriteInfo;
            set => _spriteInfo = value;
        }

        public bool Stackable
        {
            get => _stackable;
            set => _stackable = value;
        }

        public ItemTypes ItemType
        {
            get => _itemType;
            set => _itemType = value;
        }

        public EquipmentSlots SlotType
        {
            get => _slotType;
            set => _slotType = value;
        }

        public Dictionary<string, string> Scripts => _scripts;

        public int Strength
        {
            get => _strength;
            set => _strength = value;
        }

        public int Intelligence
        {
            get => _intelligence;
            set => _intelligence = value;
        }

        public int Dexterity
        {
            get => _dexterity;
            set => _dexterity = value;
        }

        public int Defence
        {
            get => _defence;
            set => _defence = value;
        }

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public ItemModel()
        {
            _scripts = new Dictionary<string, string>();
        }

        public static ItemModel Create()
        {
            var desc = new ItemModel()
            {
                _name = "Blank",
                _spriteInfo = new SpriteInfo(""),
                _stackable = false,
                _itemType = ItemTypes.NA,
                _slotType = EquipmentSlots.NE,
                _strength = 0,
                _intelligence = 0,
                _dexterity = 0,
                _defence = 0,
                _health = 0,
            };

            return desc;
        }

        public event EventHandler<EventArgs> DefinitionChanged;
    }
}