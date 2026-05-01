using Lunar.Tools.Editor.Contracts.Content;
using Lunar.Tools.Editor.Core.Projects;

namespace Lunar.Tools.Editor.Core.Content;

public sealed class ContentIndexer : IContentIndexer
{
    private readonly IProjectRepository _projects;

    private static readonly IReadOnlyDictionary<string, string> ExtToNodeType =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [".map"]  = "map",
            [".item"] = "item",
            [".npc"]  = "npc",
            [".spell"]= "spell",
            [".anim"] = "anim",
            [".dxml"] = "dialogue",
            [".cs"]   = "script",
        };

    public ContentIndexer(IProjectRepository projects)
        => _projects = projects;

    public ContentTreeNodeDto BuildTree()
    {
        var project = _projects.CurrentProject
            ?? throw new InvalidOperationException("No project is currently open.");

        string worldRoot = Path.Combine(project.ServerDataPath, "World");
        return BuildNode(worldRoot, "World");
    }

    private static ContentTreeNodeDto BuildNode(string dirPath, string name)
    {
        if (!Directory.Exists(dirPath))
            return new ContentTreeNodeDto(name, dirPath, "folder", Array.Empty<ContentTreeNodeDto>());

        var children = new List<ContentTreeNodeDto>();

        foreach (var subDir in Directory.EnumerateDirectories(dirPath).OrderBy(d => d))
            children.Add(BuildNode(subDir, Path.GetFileName(subDir)));

        foreach (var file in Directory.EnumerateFiles(dirPath).OrderBy(f => f))
        {
            string ext = Path.GetExtension(file);
            if (!ExtToNodeType.TryGetValue(ext, out string? nodeType))
                continue;

            children.Add(new ContentTreeNodeDto(
                Name: Path.GetFileNameWithoutExtension(file),
                Path: file,
                NodeType: nodeType,
                Children: Array.Empty<ContentTreeNodeDto>()
            ));
        }

        return new ContentTreeNodeDto(name, dirPath, "folder", children);
    }
}
