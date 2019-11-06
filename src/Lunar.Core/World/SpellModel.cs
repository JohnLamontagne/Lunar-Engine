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
using System.IO;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;

namespace Lunar.Core.World
{
    public class SpellModel : IContentModel
    {
        private string _name;
        private SpriteInfo _displaySprite;
        private Dictionary<string, string> _scripts;
        private int _castTime;
        private int _activeTime;

        public Stats StatModifiers { get; set; }

        public Stats ReqStats { get; set; }

        public Stats StatRequirements { get; set; }

        public int HealthCost { get; set; }

        public int ManaCost { get; set; }

        public int CooldownTime { get; set; }

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public SpriteInfo DisplaySprite
        {
            get => _displaySprite;
            set => _displaySprite = value;
        }

        public Dictionary<string, string> Scripts => _scripts;

        public int CastTime
        {
            get => _castTime;
            set => _castTime = value;
        }

        public int ActiveTime
        {
            get => _activeTime;
            set => _activeTime = value;
        }

        public string CasterAnimationPath { get; set; }

        public string TargetAnimationPath { get; set; }

        public SpellModel()
        {
            _scripts = new Dictionary<string, string>();
            this.StatModifiers = new Stats();
            this.ReqStats = new Stats();
            this.StatRequirements = new Stats();
            this.CasterAnimationPath = "";
            this.TargetAnimationPath = "";
        }

        public event EventHandler<EventArgs> Changed;
    }
}