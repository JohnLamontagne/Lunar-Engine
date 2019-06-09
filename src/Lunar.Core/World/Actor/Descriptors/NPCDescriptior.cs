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
using System.Collections.Generic;
using System.IO;
using Lunar.Core.Utilities;
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
        private List<string> _scripts;

        public string UniqueID { get; private set; }

        public string Name { get; set; }

        public string TexturePath { get; set; }
    
        public int Level { get; set; }

        public Vector Position { get; set; }

        public Stats Stats { get; private set; }

        public Stats StatBoosts { get; private set; }

        public float Speed { get; set; }

        public Rect CollisionBounds { get; set; }

        public int AggresiveRange { get; set; }

        public Vector MaxRoam { get; set; }

        public Vector FrameSize { get; set; }
    
        public int AttackRange { get; set; }

        public List<string> Scripts => _scripts;

        public Dictionary<string, object> CustomVariables { get; }

        protected NPCDescriptor()
        {
            _scripts = new List<string>();
            this.CustomVariables = new Dictionary<string, object>();

            this.Stats = new Stats();
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

                    binaryWriter.Write(this.Scripts.Count);
                    foreach (var script in this.Scripts)
                        binaryWriter.Write(script);

                    binaryWriter.Write(this.UniqueID);
                }
            }
        }

        public static NPCDescriptor Create(string uniqueID)
        {
            return new NPCDescriptor()
            {
                Name = "",
                AggresiveRange = 0,
                AttackRange = 0,
                TexturePath = "",
                UniqueID = uniqueID
            };
        }

        public static NPCDescriptor Load(string filePath)
        {
            try
            {
                string name = "";
                int level = 0;
                float speed = 0f;
                Rect collisionBounds = new Rect();
                int aggresiveRange = 0;
                string texturePath = "";
                Vector maxRoam = new Vector();
                Vector frameSize = new Vector();
                int attackRange = 0;
                Actor.Stats stats;
                List<string> scripts = new List<string>();
                string uniqueID = "";

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


                        int scriptCount = binaryReader.ReadInt32();
                        for (int i = 0; i < scriptCount; i++)
                        {
                            scripts.Add(binaryReader.ReadString());
                        }

                        uniqueID = binaryReader.ReadString();
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
                    StatBoosts = new Stats(),
                    UniqueID = uniqueID
                };
                desc.Scripts.AddRange(scripts);

                return desc;
            }
            catch (IOException exception)
            {
                Logger.LogEvent("Unable to load NPC. " + exception.Message, LogTypes.ERROR, exception);
                return null;
            }
            
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}
