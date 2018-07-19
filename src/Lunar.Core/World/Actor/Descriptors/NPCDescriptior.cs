using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Actor.Descriptors
{
    public class NPCDescriptor
    {
        private string _name;
        private int _level;
        private float _speed;
        private int _maximumHealth;
        private Rect _collisionBounds;
        private Vector _frameSize;
        private int _aggresiveRange;
        private string _texturePath;
        private Vector _maxRoam;
        private int _attackRange;
        private Dictionary<string, string> _scripts;

        public string Name => _name;
        public string TexturePath => _texturePath;
        public int Level => _level;
        public float Speed => _speed;
        public int MaximumHealth => _maximumHealth;
        public Rect CollisionBounds => _collisionBounds;
        public int AggresiveRange => _aggresiveRange;
        public Vector MaxRoam => _maxRoam;
        public Vector FrameSize => _frameSize;
        public int AttackRange => _attackRange;
        public Dictionary<string, string> Scripts => _scripts;

        protected NPCDescriptor()
        {
            _scripts = new Dictionary<string, string>();
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
                    maxRoam = new Vector(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                    frameSize = new Vector(binaryReader.ReadInt32(), binaryReader.ReadInt32());
                    attackRange = binaryReader.ReadInt32();                    
                }
            }

            NPCDescriptor desc = new NPCDescriptor()
            {
                _name = name,
                _level = level,
                _speed = speed,
                _maximumHealth = maximumHealth,
                _collisionBounds = collisionBounds,
                _aggresiveRange = aggresiveRange,
                _texturePath = texturePath,
                _maxRoam = maxRoam,
                _frameSize = frameSize,
                _attackRange = attackRange
            };


            return desc;
        }
        
    }
}
