using Lunar.Tools.Editor.Contracts.Scripts;
using Lunar.Tools.Editor.Core.Projects;

namespace Lunar.Tools.Editor.Core.Scripts;

public sealed class ScriptRepository : IScriptRepository
{
    private readonly IProjectRepository _projects;

    public ScriptRepository(IProjectRepository projects)
        => _projects = projects;

    public ScriptDocumentDto Load(string absolutePath)
    {
        string content = File.ReadAllText(absolutePath);
        string relativePath = MakeRelative(absolutePath);
        return new ScriptDocumentDto(absolutePath, relativePath, content);
    }

    public void Save(string absolutePath, string content)
        => File.WriteAllText(absolutePath, content);

    private string MakeRelative(string absolutePath)
    {
        var project = _projects.CurrentProject;
        if (project is null)
            return Path.GetFileName(absolutePath);

        string scriptsRoot = Path.Combine(project.ServerDataPath, "World", "Scripts") + Path.DirectorySeparatorChar;
        if (absolutePath.StartsWith(scriptsRoot, StringComparison.OrdinalIgnoreCase))
            return absolutePath[scriptsRoot.Length..];

        return Path.GetFileName(absolutePath);
    }
}
