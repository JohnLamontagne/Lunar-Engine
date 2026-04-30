using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;

namespace Lunar.Server.Scripting
{
    internal sealed class ScriptCompiler
    {
        private readonly string _scriptsRoot;
        private readonly IReadOnlyList<MetadataReference> _references;

        public ScriptCompiler(string scriptsRoot)
        {
            _scriptsRoot = scriptsRoot;
            _references = BuildReferences();
        }

        public CompilationResult Compile()
        {
            var trees = new List<SyntaxTree>();
            if (Directory.Exists(_scriptsRoot))
            {
                foreach (var path in Directory.EnumerateFiles(_scriptsRoot, "*.cs", SearchOption.AllDirectories))
                {
                    var source = File.ReadAllText(path);
                    trees.Add(CSharpSyntaxTree.ParseText(source, path: path));
                }
            }

            if (trees.Count == 0)
                return CompilationResult.Empty;

            var compilation = CSharpCompilation.Create(
                assemblyName: $"Lunar.Scripts.{Guid.NewGuid():N}",
                syntaxTrees: trees,
                references: _references,
                options: new CSharpCompilationOptions(
                    OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Debug,
                    allowUnsafe: false,
                    concurrentBuild: true));

            var ms = new MemoryStream();
            EmitResult result = compilation.Emit(ms);

            if (!result.Success)
            {
                var errors = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(FormatDiagnostic)
                    .ToList();
                return new CompilationResult(null, errors);
            }

            ms.Position = 0;
            return new CompilationResult(ms, Array.Empty<string>());
        }

        private static string FormatDiagnostic(Diagnostic d)
        {
            var span = d.Location.GetLineSpan();
            var file = string.IsNullOrEmpty(span.Path) ? "<unknown>" : Path.GetFileName(span.Path);
            return $"{file}({span.StartLinePosition.Line + 1},{span.StartLinePosition.Character + 1}): {d.Id} {d.GetMessage()}";
        }

        private static IReadOnlyList<MetadataReference> BuildReferences()
        {
            var refs = new List<MetadataReference>();
            refs.AddRange(Basic.Reference.Assemblies.Net90.References.All);

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.IsDynamic) continue;
                var loc = asm.Location;
                if (string.IsNullOrEmpty(loc)) continue;
                if (!seen.Add(loc)) continue;
                try
                {
                    refs.Add(MetadataReference.CreateFromFile(loc));
                }
                catch { /* skip refs that can't be loaded */ }
            }

            return refs;
        }
    }

    internal sealed class CompilationResult
    {
        public MemoryStream Assembly { get; }
        public IReadOnlyList<string> Errors { get; }
        public bool Success => Assembly != null;
        public bool IsEmpty => Assembly == null && Errors.Count == 0;

        public CompilationResult(MemoryStream assembly, IReadOnlyList<string> errors)
        {
            Assembly = assembly;
            Errors = errors;
        }

        public static CompilationResult Empty { get; } = new(null, Array.Empty<string>());
    }
}
