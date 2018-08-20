using System.Collections.Generic;
using System.IO;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Actor.Descriptors
{
    public class NPCDescriptor
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

        public int Strength { get; set; }

        public int Intelligence { get; set; }

        public int Defence { get; set; }

        public int Dexterity { get; set; }

        public int Health { get; set; }

        public float Speed { get; set; }

        public int MaximumHealth { get; set; }

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
                    binaryWriter.Write(this.MaximumHealth);
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

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    name = binaryReader.ReadString();
                    level = binaryReader.ReadInt32();
                    speed = binaryReader.ReadSingle();
                    maximumHealth = binaryReader.ReadInt32();
                    collisionBounds = new Rect(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                    aggresiveRange = binaryReader.ReadInt32();
                    texturePath = binaryReader.ReadString();
                    maxRoam = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    frameSize = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    attackRange = binaryReader.ReadInt32();                    
                }
            }

            NPCDescriptor desc = new NPCDescriptor()
            {
                Name = name,
                Level = level,
                Speed = speed,
                MaximumHealth = maximumHealth,
                CollisionBounds = collisionBounds,
                AggresiveRange = aggresiveRange,
                TexturePath = texturePath,
                MaxRoam = maxRoam,
                FrameSize = frameSize,
                AttackRange = attackRange
            };


            return desc;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
