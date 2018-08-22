using System.Collections.Generic;
using System.IO;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Actor.Descriptors
{
    public class NPCDescriptor : IActorDescriptor
    {
        private int _level;
        private float _speed;
        private int _maximumHealth;
        private Rect _collisionBounds;
        private Vector _frameSize;
        private bool _aggressive;
        private int _aggresiveRange;
        private Vector _maxRoam;
        private int _attackRange;
        private Dictionary<string, string> _scripts;

        public string Name { get; set; }

        public string TexturePath { get; set; }
    
        public int Level { get; set; }

        public Vector Position { get; set; }

        public Stats Stats { get; private set; }

        public Stats StatBoosts { get; private set; }

        public float Speed { get; set; }

        public Rect CollisionBounds { get; set; }

        public bool Aggressive { get; set; }

        public int AggresiveRange { get; set; }

        public Vector MaxRoam { get; set; }

        public Vector FrameSize { get; set; }
    
        public int AttackRange { get; set; }

        public Dictionary<string, string> Scripts => _scripts;

        protected NPCDescriptor()
        {
            _scripts = new Dictionary<string, string>();
        }

        public void Save(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(this.Name);
                    binaryWriter.Write(this.Level);
                    binaryWriter.Write(this.Speed);
                    binaryWriter.Write(this.Stats.Strength);
                    binaryWriter.Write(this.Stats.Defense);
                    binaryWriter.Write(this.Stats.Dexterity);
                    binaryWriter.Write(this.Stats.Health);
                    binaryWriter.Write(this.Stats.Intelligence);
                    binaryWriter.Write(this.Stats.MaximumHealth);
                    binaryWriter.Write(this.CollisionBounds.Left);
                    binaryWriter.Write(this.CollisionBounds.Top);
                    binaryWriter.Write(this.CollisionBounds.Width);
                    binaryWriter.Write(this.CollisionBounds.Height);
                    binaryWriter.Write(this.AggresiveRange);
                    binaryWriter.Write(this.TexturePath);
                    binaryWriter.Write(this.MaxRoam.X);
                    binaryWriter.Write(this.MaxRoam.Y);
                    binaryWriter.Write(this.FrameSize.X);
                    binaryWriter.Write(this.FrameSize.Y);
                    binaryWriter.Write(this.AttackRange);
                }
            }
        }

        public static NPCDescriptor Create()
        {
            return new NPCDescriptor()
            {
                Name = "",
                AggresiveRange = 0,
                AttackRange = 0,
                Aggressive = false,
                TexturePath = ""
            };
        }

        public static NPCDescriptor Load(string filePath)
        {
            string name = "";
            int level = 0;
            float speed = 0f;
            int maximumHealth = 0;
            Rect collisionBounds = new Rect();
            int aggresiveRange = 0;
            string texturePath = "";
            Vector maxRoam = new Vector();
            Vector frameSize = new Vector();
            int attackRange = 0;
            Actor.Stats stats;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    name = binaryReader.ReadString();
                    level = binaryReader.ReadInt32();
                    speed = binaryReader.ReadSingle();

                    stats = new Stats()
                    {
                        Strength = binaryReader.ReadInt32(),
                        Defense = binaryReader.ReadInt32(),
                        Dexterity = binaryReader.ReadInt32(),
                        Health = binaryReader.ReadInt32(),
                        Intelligence = binaryReader.ReadInt32(),
                        MaximumHealth = binaryReader.ReadInt32()
                    };

                    collisionBounds = new Rect(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                    aggresiveRange = binaryReader.ReadInt32();
                    texturePath = binaryReader.ReadString();
                    maxRoam = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    frameSize = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    attackRange = binaryReader.ReadInt32();                    
                }
            }

            NPCDescriptor desc = new NPCDescriptor
            {
                Name = name,
                Level = level,
                Speed = speed,
                Stats = stats,
                CollisionBounds = collisionBounds,
                AggresiveRange = aggresiveRange,
                TexturePath = texturePath,
                MaxRoam = maxRoam,
                FrameSize = frameSize,
                AttackRange = attackRange,
                StatBoosts = new Stats()
            };


            return desc;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
