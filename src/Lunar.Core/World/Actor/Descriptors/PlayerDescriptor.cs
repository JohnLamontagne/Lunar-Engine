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
using System.IO;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Actor.Descriptors
{
    public class PlayerDescriptor : IActorDescriptor
    {
        private string _name;
        private string _password;
        private SpriteSheet _spriteSheet;
        private float _speed;
        private int _level;
        private int _experience;
        private Vector _position;
        private string _mapID;
        private Role _role;

        public string Name
        {
            get => _name;
            set => _name = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public SpriteSheet SpriteSheet
        {
            get => _spriteSheet;
            set => _spriteSheet = value;
        }

        public float Speed
        {
            get => _speed;
            set => _speed = value;
        }

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        public int Experience
        {
            get => _experience;
            set => _experience = value;
        }

        public Stats Stats { get; set; }

        public Stats StatBoosts { get; private set; }

        public Vector Position
        {
            get => _position;
            set => _position = value;
        }

        public string MapID
        {
            get => _mapID;
            set => _mapID = value;
        }

        public Role Role
        {
            get => _role;
            set => _role = value;
        }

        public Rect CollisionBounds { get; set; }

        public PlayerDescriptor(string username, string password)
        {
            _name = username;
            _password = password;

            this.StatBoosts = new Stats();
        }

        public static PlayerDescriptor Create(string name, string password)
        {
            var descriptor = new PlayerDescriptor(name, password)
            {
                Name = name,
                Password = password,
                SpriteSheet = new SpriteSheet(new SpriteInfo("soldier.png"), 64, 64),
                Level = 1,
                Experience = 0,
                Speed = .1f,
                Stats = new Stats()
                {
                    Health = 100,
                    MaximumHealth = 100,
                    Strength = 10,
                    Intelligence = 10,
                    Dexterity = 10,
                    Defense = 10,
                },
                StatBoosts = new Stats(),
                Role = Role.Default
            };

            return descriptor;
        }

        public event EventHandler ExperienceChanged;
    }
}