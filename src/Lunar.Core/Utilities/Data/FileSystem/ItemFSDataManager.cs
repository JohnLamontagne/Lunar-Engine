using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data.Management;
using Lunar.Core.World;
using System;
using System.IO;
using System.Text.Json;

namespace Lunar.Core.Utilities.Data.FileSystem
{
    public class ItemFSDataManager : FSDataManager<ItemModel>
    {
        private record ItemDto(
            string Name,
            string SpriteName,
            bool Stackable,
            string ItemType,
            string SlotType,
            int Strength,
            int Intelligence,
            int Dexterity,
            int Defence,
            int Health,
            string BehaviorKey
        );

        private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

        public override bool Exists(IDataManagerArguments arguments)
        {
            return File.Exists(this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.ITEM_FILE_EXT);
        }

        public override ItemModel Load(IDataManagerArguments arguments)
        {
            var itemArguments = (arguments as ContentFileDataLoaderArguments);
            string json = File.ReadAllText(this.RootPath + itemArguments.FileName + EngineConstants.ITEM_FILE_EXT);
            var dto = JsonSerializer.Deserialize<ItemDto>(json, JsonOptions);

            var desc = new ItemModel();
            desc.Name = dto.Name;
            desc.SpriteInfo = new SpriteInfo(dto.SpriteName);
            desc.Stackable = dto.Stackable;
            desc.ItemType = Enum.Parse<ItemTypes>(dto.ItemType);
            desc.SlotType = Enum.Parse<EquipmentSlots>(dto.SlotType);
            desc.Strength = dto.Strength;
            desc.Intelligence = dto.Intelligence;
            desc.Dexterity = dto.Dexterity;
            desc.Defence = dto.Defence;
            desc.Health = dto.Health;
            desc.BehaviorKey = dto.BehaviorKey;

            return desc;
        }

        public override void Save(IContentModel descriptor, IDataManagerArguments arguments)
        {
            var itemDesc = (ItemModel)descriptor;
            string filePath = this.RootPath + (arguments as ContentFileDataLoaderArguments).FileName + EngineConstants.ITEM_FILE_EXT;

            var dto = new ItemDto(
                itemDesc.Name,
                itemDesc.SpriteInfo?.TextureName ?? "",
                itemDesc.Stackable,
                itemDesc.ItemType.ToString(),
                itemDesc.SlotType.ToString(),
                itemDesc.Strength,
                itemDesc.Intelligence,
                itemDesc.Dexterity,
                itemDesc.Defence,
                itemDesc.Health,
                itemDesc.BehaviorKey
            );

            File.WriteAllText(filePath, JsonSerializer.Serialize(dto, JsonOptions));
        }
    }
}
