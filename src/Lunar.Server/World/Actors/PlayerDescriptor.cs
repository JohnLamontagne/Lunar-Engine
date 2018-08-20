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
using Lunar.Server.Content.Graphics;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using System.IO;
using Lunar.Core.Utilities.Data;
using Lunar.Server.Utilities;

namespace Lunar.Server.World.Actors
{
    public class PlayerDescriptor
    {
        private string _name;
        private string _password;
        private SpriteSheet _spriteSheet;
        private float _speed;
        private int _level;
        private int _experience;
        private int _health;
        private int _vitality;
        private int _maximumHealth;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defense;
        private Vector _position;
        private string _mapID;
        private ActorBehaviorDefinition _behaviorDefinition;
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

        public int Health
        {
            get => _health;
            set => _health = value;
        }

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

        public int Defense
        {
            get => _defense;
            set => _defense = value;
        }

        public int MaximumHealth
        {
            get => _maximumHealth;
            set => _maximumHealth = value;
        }

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

        public ActorBehaviorDefinition BehaviorDefinition => _behaviorDefinition;

        public PlayerDescriptor(string username, string password)
        {
            _name = username;
            _password = password;
            _mapID = Settings.StartingMap;
        }

        public static PlayerDescriptor Create(string name, string password)
        {
            var script = new Script(Constants.FILEPATH_SCRIPTS + "player.lua");
            var behaviorDefinition = (ActorBehaviorDefinition)script["BehaviorDefinition"];

            var descriptor = new PlayerDescriptor(name, password)
            {
                Name = name,
                Password = password,
                SpriteSheet = new SpriteSheet(new Sprite("chara1.png"), 3, 4, 52, 72),
                Health = 100,
                MaximumHealth = 100,
                Level = 1,
                Experience = 0,
                Speed = .1f,
                Strength = 10,
                Intelligence = 10,
                Dexterity = 10,
                Defense = 10,
                _behaviorDefinition = behaviorDefinition,
                _mapID = Settings.StartingMap,
                _role = Settings.DefaultRole
            };

            return descriptor;
        }

        public static PlayerDescriptor Load(string name)
        {
            var password = "";
            SpriteSheet sprite;
            float speed;
            int level;
            int health;
            int maximumHealth;
            int strength;
            int intelligence;
            int dexterity;
            int defense;
            Vector position;
            string mapID;
            Role role;
            try
            {

                using (var fileStream = new FileStream(Constants.FILEPATH_ACCOUNTS + name + ".acc", FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        password = binaryReader.ReadString();
                        sprite = new SpriteSheet(new Sprite(binaryReader.ReadString()), binaryReader.ReadInt32(),
                            binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                        speed = binaryReader.ReadSingle();
                        maximumHealth = binaryReader.ReadInt32();
                        health = binaryReader.ReadInt32();
                        level = binaryReader.ReadInt32();
                        strength = binaryReader.ReadInt32();
                        intelligence = binaryReader.ReadInt32();
                        dexterity = binaryReader.ReadInt32();
                        defense = binaryReader.ReadInt32();
                        position = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        mapID = binaryReader.ReadString();
                        role = Settings.Roles?[binaryReader.ReadString()] ?? Settings.DefaultRole;
                    }
                }

                var script = new Script(Constants.FILEPATH_SCRIPTS + "player.lua");
                var behaviorDefinition = (ActorBehaviorDefinition)script["BehaviorDefinition"];

                var playerDescriptor = new PlayerDescriptor(name, password)
                {
                    SpriteSheet = sprite,
                    Speed = speed,
                    Level = level,
                    Health = health,
                    MaximumHealth = maximumHealth,
                    Strength = strength,
                    Intelligence = intelligence,
                    Dexterity = dexterity,
                    Defense = defense,
                    Position = position,
                    MapID = mapID,
                    _behaviorDefinition = behaviorDefinition,
                    Role = role
                };

                return playerDescriptor;
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Failed to register player: {ex.Message}", LogTypes.ERROR, Environment.StackTrace);
                return null;
            }
        }

        public void Save()
        {
            using (var fileStream = new FileStream(Constants.FILEPATH_ACCOUNTS + _name + ".acc", FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(_password);
                    binaryWriter.Write(_spriteSheet.Sprite.TextureName);
                    binaryWriter.Write(_spriteSheet.HorizontalFrames);
                    binaryWriter.Write(_spriteSheet.VerticalFrames);
                    binaryWriter.Write(_spriteSheet.FrameWidth);
                    binaryWriter.Write(_spriteSheet.FrameHeight);
                    binaryWriter.Write(_speed);
                    binaryWriter.Write(_maximumHealth);
                    binaryWriter.Write(_health);
                    binaryWriter.Write(_level);
                    binaryWriter.Write(_strength);
                    binaryWriter.Write(_intelligence);
                    binaryWriter.Write(_dexterity);
                    binaryWriter.Write(_defense);
                    binaryWriter.Write(_position.X);
                    binaryWriter.Write(_position.Y);
                    binaryWriter.Write(_mapID);
                    binaryWriter.Write(_role.Name);
                }
            }
        }
    }
}