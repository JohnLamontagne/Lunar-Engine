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

        public Stats Stats { get;  set; }

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

        public PlayerDescriptor(string username, string password)
        {
            _name = username;
            _password = password;
        }

        public static PlayerDescriptor Create(string name, string password)
        {
            var descriptor = new PlayerDescriptor(name, password)
            {
                Name = name,
                Password = password,
                SpriteSheet = new SpriteSheet(new SpriteInfo("chara1.png"), 3, 4, 52, 72),
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


        public void Save(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
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
                    binaryWriter.Write(this.Stats.MaximumHealth);
                    binaryWriter.Write(this.Stats.Health);
                    binaryWriter.Write(this.Stats.Strength);
                    binaryWriter.Write(this.Stats.Intelligence);
                    binaryWriter.Write(this.Stats.Dexterity);
                    binaryWriter.Write(this.Stats.Defense);
                    binaryWriter.Write(_level);
                    binaryWriter.Write(_position.X);
                    binaryWriter.Write(_position.Y);
                    binaryWriter.Write(_mapID);
                    binaryWriter.Write(_role.Name);
                }
            }
        }


        public event EventHandler StatChanged;
        public event EventHandler ExperienceChanged;
    }
}
