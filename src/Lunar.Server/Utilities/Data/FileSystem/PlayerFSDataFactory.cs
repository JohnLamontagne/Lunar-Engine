using System;
using System.IO;
using Lunar.Core;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Server.Utilities.Data.FileSystem
{
    public class PlayerFSDataLoader : IDataLoader<PlayerDescriptor>
    {
        public PlayerFSDataLoader()
        {
            
        }

        public PlayerDescriptor Load(IDataLoaderArguments arguments)
        {
            var playerDataLoaderArgs = arguments as PlayerDataLoaderArguments;

            string filePath = Constants.FILEPATH_ACCOUNTS + playerDataLoaderArgs.Username +
                              EngineConstants.ACC_FILE_EXT;

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
                };

                return playerDescriptor;
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Error loading player data from filesystem!", LogTypes.ERROR, ex.StackTrace);

                return null;
            }
        }
    }
}
