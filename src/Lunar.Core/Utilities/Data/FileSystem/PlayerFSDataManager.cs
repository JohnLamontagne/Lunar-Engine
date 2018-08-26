using System;
using System.IO;
using Lidgren.Network;
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.FileSystem;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Server.Utilities.Data.FileSystem
{
    public class PlayerFSDataLoader : IDataManager<PlayerDescriptor>
    {
        public PlayerFSDataLoader()
        {
            
        }

        public PlayerDescriptor Load(IDataManagerArguments arguments)
        {
            var playerDataLoaderArgs = arguments as PlayerDataLoaderArguments;

            string filePath = EngineConstants.FILEPATH_ACCOUNTS + playerDataLoaderArgs.Username + EngineConstants.ACC_FILE_EXT;

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
            try
            {

                using (var fileStream = new FileStream(filePath, FileMode.Open))
                {
                    using (var binaryReader = new BinaryReader(fileStream))
                    {
                        name = binaryReader.ReadString();
                        password = binaryReader.ReadString();
                        sprite = new SpriteSheet(new SpriteInfo(binaryReader.ReadString()), binaryReader.ReadInt32(),
                            binaryReader.ReadInt32(), binaryReader.ReadInt32(), binaryReader.ReadInt32());
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
                    }
                }

                var playerDescriptor = new PlayerDescriptor(name, password)
                {
                    SpriteSheet = sprite,
                    Speed = speed,
                    Level = level,
                    Position = position,
                    MapID = mapID,
                    Stats = new Stats()
                    {
                        Health = health,
                        MaximumHealth = maximumHealth,
                        Strength = strength,
                        Intelligence = intelligence,
                        Dexterity = dexterity,
                        Defense = defense,
                    },
                    Role = role
                };

                return playerDescriptor;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public void Save(IDataDescriptor descriptor)
        {
            PlayerDescriptor playerDescriptor = ((PlayerDescriptor) descriptor);

            string filePath = EngineConstants.FILEPATH_DATA + playerDescriptor.Name + EngineConstants.ACC_FILE_EXT;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(playerDescriptor.Name);
                    binaryWriter.Write(playerDescriptor.Password);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.Sprite.TextureName);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.HorizontalFrames);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.VerticalFrames);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.FrameWidth);
                    binaryWriter.Write(playerDescriptor.SpriteSheet.FrameHeight);
                    binaryWriter.Write(playerDescriptor.Speed);
                    binaryWriter.Write(playerDescriptor.Stats.MaximumHealth);
                    binaryWriter.Write(playerDescriptor.Stats.Health);
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
                }
            }
        }

        public bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(EngineConstants.FILEPATH_DATA + (arguments as PlayerDataLoaderArguments)?.Username + EngineConstants.ACC_FILE_EXT);
        }
    }
}
