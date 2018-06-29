/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Lunar.Server.Content.Graphics;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using System.IO;
using Lunar.Core.Utilities.Data;

namespace Lunar.Server.World.Actors
{
    public class PlayerDescriptor
    {
        private string _name;
        private string _password;
        private Sprite _sprite;
        private float _speed;
        private int _level;
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

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
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

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        public int Strength { get => _strength;
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

        public ActorBehaviorDefinition BehaviorDefinition => _behaviorDefinition;

        public PlayerDescriptor(string username, string password)
        {
            _name = username;
            _password = password;
            _mapID = Constants.STARTER_MAP;
        }

        public static PlayerDescriptor Create(string name, string password)
        {
            var script = new Script(Constants.FILEPATH_SCRIPTS + "player.lua");
            var behaviorDefinition = (ActorBehaviorDefinition)script["BehaviorDefinition"];

            var descriptor = new PlayerDescriptor(name, password)
            {
                Name= name,
                Password = password,
                Sprite = new Sprite("chara1.png"),
                Health = 100,
                MaximumHealth = 100,
                Speed = .1f,
                Strength = 10,
                Intelligence = 10,
                Dexterity = 10,
                Defense = 10,
                _behaviorDefinition = behaviorDefinition
            };

            return descriptor;
        }

        public static PlayerDescriptor Load(string name)
        {
            var password = "";
            Sprite sprite;
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

            using (var fileStream = new FileStream(Constants.FILEPATH_ACCOUNTS + name + ".acc", FileMode.Open))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    password = binaryReader.ReadString();
                    sprite = new Sprite(binaryReader.ReadString());
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
                }
            }

            var script = new Script(Constants.FILEPATH_SCRIPTS + "player.lua");
            var behaviorDefinition = (ActorBehaviorDefinition)script["BehaviorDefinition"];

            var playerDescriptor = new PlayerDescriptor(name, password)
            {
                Sprite = sprite,
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
                _behaviorDefinition = behaviorDefinition
            };

            return playerDescriptor;
        }

        public void Save()
        {
            using (var fileStream = new FileStream(Constants.FILEPATH_ACCOUNTS + _name + ".acc", FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(_password);
                    binaryWriter.Write(_sprite.TextureName);
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
                }
            }
        }
    }
}