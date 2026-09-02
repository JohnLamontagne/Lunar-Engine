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

            // The BCL comes from the net9.0 reference assemblies. Only application-local assemblies
            // (the server, Lunar.Core, LiteNetLib, ...) are added on top; adding the runtime's own
            // implementation assemblies as well gives Roslyn two candidate core libraries and it
            // then fails to resolve even System.Void (CS0518). This also keeps compilation identical
            // whether the host runs on the .NET 9 runtime or rolls forward to a newer one.
            refs.AddRange(Basic.Reference.Assemblies.Net90.References.All);

            // Application-local assemblies are referenced from disk rather than from what happens to be
            // loaded already: assembly loading is lazy, so relying on AppDomain.GetAssemblies() made the
            // set of visible types depend on what the host had touched before the first compile.
            string appRoot = AppContext.BaseDirectory;
            foreach (var dll in Directory.EnumerateFiles(appRoot, "*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    refs.Add(MetadataReference.CreateFromFile(dll));
                }
                catch
                {
                    // Native libraries and anything else that is not a managed assembly are skipped.
                }
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
