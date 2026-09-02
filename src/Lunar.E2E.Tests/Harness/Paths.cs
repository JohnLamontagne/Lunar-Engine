using System;
using System.IO;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// Locates build outputs and artifact folders. Overridable through environment variables so the
    /// same tests run from an IDE, the command line, or a container with prebuilt binaries.
    /// </summary>
    public static class Paths
    {
        /// <summary>Directory containing Lunar.Server.dll (LUNAR_SERVER_BIN).</summary>
        public static string ServerBin => FromEnvOrConvention("LUNAR_SERVER_BIN", "Lunar.Server");

        /// <summary>Directory containing Lunar.Client.Desktop.dll (LUNAR_CLIENT_BIN).</summary>
        public static string ClientBin => FromEnvOrConvention("LUNAR_CLIENT_BIN", "Lunar.Client.Desktop");

        /// <summary>Where screenshots and logs are written (LUNAR_E2E_ARTIFACTS).</summary>
        public static string Artifacts
        {
            get
            {
                var fromEnv = Environment.GetEnvironmentVariable("LUNAR_E2E_ARTIFACTS");
                var dir = string.IsNullOrWhiteSpace(fromEnv)
                    ? Path.Combine(SourceRoot, "..", "artifacts", "e2e")
                    : fromEnv;
                Directory.CreateDirectory(dir);
                return Path.GetFullPath(dir);
            }
        }

        /// <summary>The src/ directory, found by walking up from the test assembly.</summary>
        public static string SourceRoot
        {
            get
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                while (dir != null && !File.Exists(Path.Combine(dir.FullName, "Lunar Engine.sln")))
                    dir = dir.Parent;
                if (dir == null)
                    throw new DirectoryNotFoundException("Could not find the src directory containing 'Lunar Engine.sln' above " + AppContext.BaseDirectory);
                return dir.FullName;
            }
        }

        private static string FromEnvOrConvention(string variable, string project)
        {
            var fromEnv = Environment.GetEnvironmentVariable(variable);
            if (!string.IsNullOrWhiteSpace(fromEnv))
                return fromEnv;

            // Mirror this test assembly's own configuration and framework folder names,
            // e.g. src/Lunar.Server/bin/Debug/net9.0.
            var testBin = new DirectoryInfo(AppContext.BaseDirectory);
            string framework = testBin.Name;
            string configuration = testBin.Parent?.Name ?? "Debug";
            return Path.Combine(SourceRoot, project, "bin", configuration, framework);
        }
    }
}
