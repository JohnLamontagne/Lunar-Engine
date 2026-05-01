using System.Text.Json;
using Lunar.Tools.Editor.Contracts.Documents;

namespace Lunar.Tools.Editor.Core.Items;

public sealed class ItemRepository : IItemRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private record ItemFileDto(
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

    public ItemEditorDocument Load(string absolutePath)
    {
        string json = File.ReadAllText(absolutePath);
        var dto = JsonSerializer.Deserialize<ItemFileDto>(json, JsonOptions)
            ?? throw new InvalidDataException($"Failed to parse item file: {absolutePath}");

        return new ItemEditorDocument(
            FilePath: absolutePath,
            Name: dto.Name,
            SpriteName: dto.SpriteName,
            Stackable: dto.Stackable,
            ItemType: dto.ItemType,
            SlotType: dto.SlotType,
            Strength: dto.Strength,
            Intelligence: dto.Intelligence,
            Dexterity: dto.Dexterity,
            Defence: dto.Defence,
            Health: dto.Health,
            BehaviorKey: dto.BehaviorKey
        );
    }

    public ItemEditorDocument Create(CreateItemRequest request)
    {
        string filePath = Path.Combine(request.DirPath, request.Name + ".item");
        var doc = new ItemEditorDocument(
            FilePath: filePath,
            Name: request.Name,
            SpriteName: "",
            Stackable: false,
            ItemType: "NA",
            SlotType: "NE",
            Strength: 0, Intelligence: 0, Dexterity: 0, Defence: 0, Health: 0,
            BehaviorKey: ""
        );
        Save(doc);
        return doc;
    }

    public void Save(ItemEditorDocument document)
    {
        var dto = new ItemFileDto(
            document.Name, document.SpriteName, document.Stackable,
            document.ItemType, document.SlotType,
            document.Strength, document.Intelligence, document.Dexterity,
            document.Defence, document.Health, document.BehaviorKey
        );
        File.WriteAllText(document.FilePath, JsonSerializer.Serialize(dto, JsonOptions));
    }

    public void Delete(string absolutePath) => File.Delete(absolutePath);
}
