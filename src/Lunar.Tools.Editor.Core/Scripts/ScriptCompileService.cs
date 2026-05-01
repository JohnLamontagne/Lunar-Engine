using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Lunar.Tools.Editor.Contracts.Validation;
using Lunar.Tools.Editor.Core.Projects;

namespace Lunar.Tools.Editor.Core.Scripts;

public sealed class ScriptCompileService
{
    private readonly IProjectRepository _projects;

    public ScriptCompileService(IProjectRepository projects)
        => _projects = projects;

    public IReadOnlyList<ValidationIssueDto> Compile()
    {
        var project = _projects.CurrentProject
            ?? throw new InvalidOperationException("No project is currently open.");

        string scriptsRoot = Path.Combine(project.ServerDataPath, "World", "Scripts");

        var trees = new List<SyntaxTree>();
        if (Directory.Exists(scriptsRoot))
        {
            foreach (var path in Directory.EnumerateFiles(scriptsRoot, "*.cs", SearchOption.AllDirectories))
            {
                string source = File.ReadAllText(path);
                trees.Add(CSharpSyntaxTree.ParseText(source, path: path));
            }
        }

        if (trees.Count == 0)
            return Array.Empty<ValidationIssueDto>();

        var references = BuildReferences();
        var compilation = CSharpCompilation.Create(
            assemblyName: $"Lunar.Scripts.{Guid.NewGuid():N}",
            syntaxTrees: trees,
            references: references,
            options: new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary,
                optimizationLevel: OptimizationLevel.Debug,
                allowUnsafe: false,
                concurrentBuild: true));

        using var ms = new MemoryStream();
        EmitResult result = compilation.Emit(ms);

        return result.Diagnostics
            .Where(d => d.Severity >= DiagnosticSeverity.Warning)
            .Select(ToDto)
            .ToList();
    }

    private static ValidationIssueDto ToDto(Diagnostic d)
    {
        var span = d.Location.GetLineSpan();
        return new ValidationIssueDto(
            FileName: string.IsNullOrEmpty(span.Path) ? "<unknown>" : Path.GetFileName(span.Path),
            FilePath: string.IsNullOrEmpty(span.Path) ? null : span.Path,
            Line: span.StartLinePosition.Line + 1,
            Column: span.StartLinePosition.Character + 1,
            DiagnosticId: d.Id,
            Message: d.GetMessage(),
            Severity: d.Severity switch
            {
                DiagnosticSeverity.Error   => "error",
                DiagnosticSeverity.Warning => "warning",
                _                          => "info"
            }
        );
    }

    private static IReadOnlyList<MetadataReference> BuildReferences()
    {
        var refs = new List<MetadataReference>();
        refs.AddRange(Basic.Reference.Assemblies.Net90.References.All);

        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (asm.IsDynamic) continue;
            string loc = asm.Location;
            if (string.IsNullOrEmpty(loc) || !seen.Add(loc)) continue;
            try { refs.Add(MetadataReference.CreateFromFile(loc)); }
            catch { /* skip unloadable assemblies */ }
        }

        return refs;
    }
}
