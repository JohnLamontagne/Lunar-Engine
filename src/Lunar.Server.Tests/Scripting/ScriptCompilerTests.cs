using System;
using System.IO;
using System.Linq;
using Lunar.Server.Scripting;
using Xunit;

namespace Lunar.Server.Tests.Scripting
{
    /// <summary>
    /// The Roslyn script host must compile gameplay scripts against the server's own types using the
    /// .NET 9 reference assemblies, regardless of which runtime the host process is on.
    /// </summary>
    public class ScriptCompilerTests : IDisposable
    {
        private readonly string _root = Path.Combine(Path.GetTempPath(), "lunar-script-tests", Guid.NewGuid().ToString("N"));

        public ScriptCompilerTests() => Directory.CreateDirectory(_root);

        [Fact]
        public void Empty_directory_yields_empty_result()
        {
            var result = new ScriptCompiler(_root).Compile();
            Assert.True(result.IsEmpty);
            Assert.False(result.Success);
        }

        [Fact]
        public void Compiles_a_script_that_uses_bcl_and_server_types()
        {
            File.WriteAllText(Path.Combine(_root, "Hello.cs"), @"
using System;
using System.Collections.Generic;
using Lunar.Server.Scripting.Api;

[CommandScript]
public sealed class HelloCommands : CommandScript
{
    public override void Register(ICommandRegistrar registrar)
    {
        var greetings = new List<string> { ""hi"" };
        Console.WriteLine(string.Join("","", greetings));
    }
}
");
            var result = new ScriptCompiler(_root).Compile();

            Assert.True(result.Success, "Compile failed:\n" + string.Join("\n", result.Errors));
            Assert.NotNull(result.Assembly);
            Assert.True(result.Assembly.Length > 0);
        }

        [Fact]
        public void Reports_errors_with_file_and_position()
        {
            File.WriteAllText(Path.Combine(_root, "Broken.cs"), "public class Broken { int x = \"not an int\"; }");

            var result = new ScriptCompiler(_root).Compile();

            Assert.False(result.Success);
            var error = Assert.Single(result.Errors);
            Assert.StartsWith("Broken.cs(1,", error);
            Assert.Contains("CS0029", error);
        }

        [Fact]
        public void Shipped_scripts_compile()
        {
            // The scripts under Server Data are copied to the server's output directory, which is
            // where this test's reference to Lunar.Server places them as well.
            string shipped = Path.Combine(AppContext.BaseDirectory, "Server Data", "World", "Scripts");
            Assert.True(Directory.Exists(shipped), $"Expected shipped scripts at {shipped}.");
            Assert.NotEmpty(Directory.EnumerateFiles(shipped, "*.cs", SearchOption.AllDirectories));

            var result = new ScriptCompiler(shipped).Compile();

            Assert.True(result.Success, "Shipped scripts failed to compile:\n" + string.Join("\n", result.Errors));
        }

        public void Dispose()
        {
            try { Directory.Delete(_root, true); } catch { /* best effort */ }
        }
    }
}
