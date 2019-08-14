using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class NPCFSDataManager : FSDataManager<NPCDescriptor>
    {
        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.NPC_FILE_EXT);
        }

        public override NPCDescriptor Load(IDataManagerArguments arguments)
        {

            try
            {
                var npcArguments = (arguments as ContentFileDataLoaderArguments);

                string name = "";
                int level = 0;
                float speed = 0f;
                Rect collisionBounds = new Rect();
                int aggresiveRange = 0;
                string texturePath = "";
                Vector maxRoam = new Vector();
                Vector frameSize = new Vector();
                Vector reach = new Vector();
                Stats stats;
                List<string> scripts = new List<string>();
                long uniqueID = 0;
                string dialogue = "";
                string dialogueBranch = "";

                using (var fileStream = new FileStream(this.RootPath + npcArguments.FileName + EngineConstants.NPC_FILE_EXT, FileMode.OpenOrCreate))
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
                            CurrentHealth = binaryReader.ReadInt32(),
                            Intelligence = binaryReader.ReadInt32(),
                            Health = binaryReader.ReadInt32()
                        };

                        collisionBounds = new Rect(binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                        aggresiveRange = binaryReader.ReadInt32();
                        texturePath = binaryReader.ReadString();
                        maxRoam = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        frameSize = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        reach = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());

                        int scriptCount = binaryReader.ReadInt32();
                        for (int i = 0; i < scriptCount; i++)
                        {
                            scripts.Add(binaryReader.ReadString());
                        }

                        dialogue = binaryReader.ReadString();
                        dialogueBranch = binaryReader.ReadString();
                    }
                }

                var desc = NPCDescriptor.Create(npcArguments.FileName);
                desc.Name = name;
                desc.Level = level;
                desc.Speed = speed;
                desc.Stats = stats;
                desc.CollisionBounds = collisionBounds;
                desc.AggresiveRange = aggresiveRange;
                desc.TexturePath = texturePath;
                desc.MaxRoam = maxRoam;
                desc.FrameSize = frameSize;
                desc.Reach = reach;
                desc.Dialogue = dialogue;
                desc.DialogueBranch = dialogueBranch;
                desc.Scripts.AddRange(scripts);

                return desc;
            }
            catch (IOException exception)
            {
                Engine.Services.Get<Logger>().LogEvent("Unable to load NPC. " + exception.Message, LogTypes.ERROR, exception);
                return null;
            }
        }

        public override void Save(IContentDescriptor descriptor, IDataManagerArguments arguments)
        {
            var npcDesc = (NPCDescriptor)descriptor;

            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.NPC_FILE_EXT;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(npcDesc.Name);
                    binaryWriter.Write(npcDesc.Level);
                    binaryWriter.Write(npcDesc.Speed);
                    binaryWriter.Write(npcDesc.Stats.Strength);
                    binaryWriter.Write(npcDesc.Stats.Defense);
                    binaryWriter.Write(npcDesc.Stats.Dexterity);
                    binaryWriter.Write(npcDesc.Stats.CurrentHealth);
                    binaryWriter.Write(npcDesc.Stats.Intelligence);
                    binaryWriter.Write(npcDesc.Stats.Health);
                    binaryWriter.Write(npcDesc.CollisionBounds.X);
                    binaryWriter.Write(npcDesc.CollisionBounds.Y);
                    binaryWriter.Write(npcDesc.CollisionBounds.Width);
                    binaryWriter.Write(npcDesc.CollisionBounds.Height);
                    binaryWriter.Write(npcDesc.AggresiveRange);
                    binaryWriter.Write(npcDesc.TexturePath);
                    binaryWriter.Write(npcDesc.MaxRoam.X);
                    binaryWriter.Write(npcDesc.MaxRoam.Y);
                    binaryWriter.Write(npcDesc.FrameSize.X);
                    binaryWriter.Write(npcDesc.FrameSize.Y);
                    binaryWriter.Write(npcDesc.Reach.X);
                    binaryWriter.Write(npcDesc.Reach.Y);

                    binaryWriter.Write(npcDesc.Scripts.Count);
                    foreach (var script in npcDesc.Scripts)
                        binaryWriter.Write(script);

                    binaryWriter.Write(npcDesc.Dialogue);
                    binaryWriter.Write(npcDesc.DialogueBranch);
                }
            }
        }
    }
}
