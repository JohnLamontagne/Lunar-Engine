using System.Xml.Linq;
using Lunar.Tools.Editor.Contracts.Projects;

namespace Lunar.Tools.Editor.Core.Projects;

public sealed class ProjectRepository : IProjectRepository
{
    private ProjectManifest? _current;

    public ProjectManifest? CurrentProject => _current;

    public ProjectManifest Open(OpenProjectRequest request)
    {
        string path = request.ProjectFilePath.Trim();

        if (Directory.Exists(path))
            throw new ArgumentException($"'{path}' is a directory. Provide the full path to a .lproj file.");

        if (!File.Exists(path))
            throw new FileNotFoundException($"Project file not found: {path}");

        var doc = XDocument.Load(path);
        var general = doc.Elements("Config").Elements("General");

        string serverDataPath = general.Elements("Server_Data_Path").FirstOrDefault()?.Value
            ?? throw new InvalidDataException("Missing Server_Data_Path in project file.");
        string clientDataPath = general.Elements("Client_Data_Path").FirstOrDefault()?.Value
            ?? throw new InvalidDataException("Missing Client_Data_Path in project file.");

        string gameName = Path.GetFileNameWithoutExtension(path);
        _current = new ProjectManifest(path, serverDataPath, clientDataPath, gameName);
        return _current;
    }

    public ProjectManifest Create(CreateProjectRequest request)
    {
        string projectFilePath = request.ProjectFilePath.Trim();
        string serverDataPath = request.ServerDataPath.Trim().Replace('\\', '/');
        string clientDataPath = request.ClientDataPath.Trim().Replace('\\', '/');

        if (string.IsNullOrEmpty(Path.GetFileName(projectFilePath)))
            throw new ArgumentException("Project file path must include a filename (e.g. MyGame.lproj).");

        if (Directory.Exists(projectFilePath))
            throw new ArgumentException($"'{projectFilePath}' is an existing directory. Provide a full file path including the filename.");

        // Ensure the .lproj file's parent directory exists before saving.
        string? projectDir = Path.GetDirectoryName(projectFilePath);
        if (!string.IsNullOrEmpty(projectDir))
            Directory.CreateDirectory(projectDir);

        CreateDirectoryLayout(serverDataPath);
        Directory.CreateDirectory(clientDataPath);

        var xml = new XElement("Config",
            new XElement("General",
                new XElement("Server_Data_Path", serverDataPath),
                new XElement("Client_Data_Path", clientDataPath)
            )
        );
        xml.Save(projectFilePath);

        string gameName = Path.GetFileNameWithoutExtension(projectFilePath);
        _current = new ProjectManifest(projectFilePath, serverDataPath, clientDataPath, gameName);
        return _current;
    }

    private static void CreateDirectoryLayout(string serverRoot)
    {
        var subdirs = new[]
        {
            "World/Maps",
            "World/Items",
            "World/Npcs",
            "World/Spells",
            "World/Animations",
            "World/Dialogues",
            "World/Scripts/Commands",
            "World/Scripts/Npcs",
            "World/Scripts/Players",
            "internal",
        };

        foreach (var sub in subdirs)
            Directory.CreateDirectory(Path.Combine(serverRoot, sub));
    }
}
