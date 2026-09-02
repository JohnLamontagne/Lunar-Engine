using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// One server per test class; each test starts the clients it needs against it. Every test gets
    /// an artifact folder named after it under a per-run timestamped root.
    /// </summary>
    public sealed class E2EFixture : IAsyncLifetime
    {
        public ServerInstance Server { get; private set; }
        public string ArtifactRoot { get; } = Path.Combine(Paths.Artifacts, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss"));

        public async Task InitializeAsync()
        {
            Directory.CreateDirectory(ArtifactRoot);
            Server = await ServerInstance.StartAsync(ArtifactRoot);
        }

        public string ArtifactDir(string testName)
        {
            var dir = Path.Combine(ArtifactRoot, testName);
            Directory.CreateDirectory(dir);
            return dir;
        }

        /// <summary>Starts a fresh client connected to this fixture's server. Dispose it when the test ends.</summary>
        public Task<ClientInstance> StartClientAsync(string artifactDir, string name = "client")
            => ClientInstance.StartAsync(artifactDir, "127.0.0.1", Server.Port, name: name);

        public Task DisposeAsync()
        {
            Server?.Dispose();
            return Task.CompletedTask;
        }
    }
}
