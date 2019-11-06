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
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class PlayerFSDataManager : FSDataManager<PlayerModel>
    {
        public override PlayerModel Load(IDataManagerArguments arguments)
        {
            string filePath = this.RootPath + (arguments as PlayerDataArguments).Username + EngineConstants.ACC_FILE_EXT;

            string name = "";
            string password = "";
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
            Vector reach;

            try
            {
                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        name = binaryReader.ReadString();
                        password = binaryReader.ReadString();
                        sprite = new SpriteSheet(new SpriteInfo(binaryReader.ReadString()), binaryReader.ReadInt32(), binaryReader.ReadInt32());
                        speed = binaryReader.ReadSingle();
                        maximumHealth = binaryReader.ReadInt32();
                        health = binaryReader.ReadInt32();
                        strength = binaryReader.ReadInt32();
                        intelligence = binaryReader.ReadInt32();
                        dexterity = binaryReader.ReadInt32();
                        defense = binaryReader.ReadInt32();
                        level = binaryReader.ReadInt32();
                        position = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                        mapID = binaryReader.ReadString();
                        role = new Role(binaryReader.ReadString(), binaryReader.ReadInt32());
                        reach = new Vector(binaryReader.ReadSingle(), binaryReader.ReadSingle());
                    }
                }

                var playerDescriptor = new PlayerModel(name, password)
                {
                    SpriteSheet = sprite,
                    Speed = speed,
                    Level = level,
                    Position = position,
                    MapID = mapID,
                    Stats = new Stats()
                    {
                        Vitality = health,
                        Strength = strength,
                        Intelligence = intelligence,
                        Dexterity = dexterity,
                        Defense = defense,
                    },
                    Role = role,
                    Reach = reach
                };

                return playerDescriptor;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            PlayerModel playerDescriptor = ((PlayerModel)descriptor);

            using (var fileStream = new FileStream(this.RootPath + playerDescriptor.Name + EngineConstants.ACC_FILE_EXT, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(playerDescriptor.Name);
                    binaryWriter.Write(playerDescriptor.Password);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.Sprite.TextureName);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.FrameWidth);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.FrameHeight);
                    binaryWriter.Write(playerDescriptor.Speed);
                    binaryWriter.Write(playerDescriptor.Stats.Vitality);
                    binaryWriter.Write(playerDescriptor.Stats.Vitality);
                    binaryWriter.Write(playerDescriptor.Stats.Strength);
                    binaryWriter.Write(playerDescriptor.Stats.Intelligence);
                    binaryWriter.Write(playerDescriptor.Stats.Dexterity);
                    binaryWriter.Write(playerDescriptor.Stats.Defense);
                    binaryWriter.Write(playerDescriptor.Level);
                    binaryWriter.Write(playerDescriptor.Position.X);
                    binaryWriter.Write(playerDescriptor.Position.Y);
                    binaryWriter.Write(playerDescriptor.MapID);
                    binaryWriter.Write(playerDescriptor.Role.Name);
                    binaryWriter.Write(playerDescriptor.Role.Level);
                    binaryWriter.Write(playerDescriptor.Reach.X);
                    binaryWriter.Write(playerDescriptor.Reach.Y);
                }
            }
        }

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as PlayerDataArguments).Username + EngineConstants.ACC_FILE_EXT);
        }
    }
}