namespace Lunar.Tools.Editor.Contracts.Documents;

/// <summary>
/// Editor-safe authoring model for an item (.item JSON file).
/// Corresponds to ItemModel but decoupled from SpriteInfo and engine runtime.
/// ItemType values: "Equipment", "Usable", "NA"
/// SlotType values: "NE", "MainArm", "SideArm", "Ring", "SecRing", "Helm", "Boots", "Chest", "Legs", "Shoulder"
/// </summary>
public record ItemEditorDocument(
    string FilePath,
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
