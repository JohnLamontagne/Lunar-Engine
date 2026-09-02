using System;
using System.IO;
using System.Threading.Tasks;
using Lunar.E2E.Tests.Harness;
using Xunit;

namespace Lunar.E2E.Tests
{
    /// <summary>
    /// The server must stop cleanly on SIGTERM (what Docker and the test harness send) and persist
    /// connected players on the way out. Uses its own server because it shuts it down.
    /// </summary>
    [Trait("Category", "E2E")]
    public class ServerLifecycleTests
    {
        private static readonly TimeSpan Wait = TimeSpan.FromSeconds(30);

        [Fact]
        public async Task Sigterm_stops_the_server_cleanly_and_saves_connected_players()
        {
            var artifactDir = Path.Combine(Paths.Artifacts, DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + "-lifecycle");
            Directory.CreateDirectory(artifactDir);

            using var server = await ServerInstance.StartAsync(artifactDir);
            string user = "e2e_" + Guid.NewGuid().ToString("N").Substring(0, 8);

            using (var client = await ClientInstance.StartAsync(artifactDir, "127.0.0.1", server.Port))
            {
                await client.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");
                await client.RegisterAsync(user, "pw");
                await client.WaitForStateAsync(s => s.Scene == "gameScene" && s.Player?.Name == user, Wait, "in world");

                int exitCode = await server.StopGracefullyAsync(Wait);
                Assert.True(exitCode == 0, $"Server exit code was {exitCode}.\n--- server output ---\n{server.Output}");
            }

            Assert.Contains("Shutdown requested (SIGTERM)", server.Output);
            Assert.Contains("Server stopped.", server.Output);
            Assert.DoesNotContain("Unhandled exception", server.Output);

            var accountFile = Path.Combine(server.DataRoot, "Server Data", "World", "Accounts", user + ".player");
            Assert.True(File.Exists(accountFile), $"Expected saved account at {accountFile}.\n--- server output ---\n{server.Output}");
        }
    }
}
