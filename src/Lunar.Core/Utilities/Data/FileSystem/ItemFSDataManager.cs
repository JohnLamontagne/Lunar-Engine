using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class ItemFSDataManager : FSDataManager<ItemModel>
    {
        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.ITEM_FILE_EXT);
        }

        public override ItemModel Load(IDataManagerArguments arguments)
        {
            var itemArguments = (arguments as ContentFileDataLoaderArguments);

            var desc = new ItemModel();

            using (var fileStream = new FileStream(this.RootPath + itemArguments.FileName + EngineConstants.ITEM_FILE_EXT, FileMode.Open))
            {
                using (var binaryReader = new BinaryReader(fileStream))
                {
                    desc.Name = binaryReader.ReadString();
                    desc.SpriteInfo = new SpriteInfo(binaryReader.ReadString());
                    desc.Stackable = binaryReader.ReadBoolean();
                    desc.ItemType = (ItemTypes)Enum.Parse(typeof(ItemTypes), binaryReader.ReadString());
                    desc.SlotType = (EquipmentSlots)Enum.Parse(typeof(EquipmentSlots), binaryReader.ReadString());
                    desc.Strength = binaryReader.ReadInt32();
                    desc.Intelligence = binaryReader.ReadInt32();
                    desc.Dexterity = binaryReader.ReadInt32();
                    desc.Defence = binaryReader.ReadInt32();
                    desc.Health = binaryReader.ReadInt32();

                    int scriptCount = binaryReader.ReadInt32();
                    for (int i = 0; i < scriptCount; i++)
                    {
                        desc.Scripts.Add(binaryReader.ReadString(), binaryReader.ReadString());
                    }
                }
            }

            return desc;
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            var itemDesc = (ItemModel)descriptor;

            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.ITEM_FILE_EXT;

            using (var fileStream = new FileStream(filePath, FileMode.OpenOrCreate))
            {
                using (var binaryWriter = new BinaryWriter(fileStream))
                {
                    binaryWriter.Write(itemDesc.Name);
                    binaryWriter.Write(itemDesc.SpriteInfo.TextureName);
                    binaryWriter.Write(itemDesc.Stackable);
                    binaryWriter.Write(itemDesc.ItemType.ToString());
                    binaryWriter.Write(itemDesc.SlotType.ToString());
                    binaryWriter.Write(itemDesc.Strength);
                    binaryWriter.Write(itemDesc.Intelligence);
                    binaryWriter.Write(itemDesc.Dexterity);
                    binaryWriter.Write(itemDesc.Defence);
                    binaryWriter.Write(itemDesc.Health);
                    binaryWriter.Write(itemDesc.Scripts.Count);
                    foreach (var script in itemDesc.Scripts)
                    {
                        binaryWriter.Write(script.Key);
                        binaryWriter.Write(script.Value);
                    }
                }
            }
        }
    }
}