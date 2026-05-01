namespace Lunar.Tools.Editor.Contracts.Content;

/// <summary>
/// A node in the project content browser tree.
/// NodeType values: "folder", "map", "item", "npc", "spell", "anim", "dialogue", "script"
/// </summary>
public record ContentTreeNodeDto(
    string Name,
    string Path,
    string NodeType,
    IReadOnlyList<ContentTreeNodeDto> Children
);
