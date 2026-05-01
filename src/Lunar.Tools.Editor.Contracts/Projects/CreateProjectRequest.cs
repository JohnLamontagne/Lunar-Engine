namespace Lunar.Tools.Editor.Contracts.Projects;

public record CreateProjectRequest(
    string ProjectFilePath,
    string ServerDataPath,
    string ClientDataPath
);
