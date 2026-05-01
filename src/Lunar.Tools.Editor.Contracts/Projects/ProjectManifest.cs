namespace Lunar.Tools.Editor.Contracts.Projects;

public record ProjectManifest(
    string ProjectFilePath,
    string ServerDataPath,
    string ClientDataPath,
    string GameName
);
