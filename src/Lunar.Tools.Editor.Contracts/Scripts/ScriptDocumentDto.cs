namespace Lunar.Tools.Editor.Contracts.Scripts;

public record ScriptDocumentDto(
    string FilePath,
    string RelativePath,
    string Content
);
