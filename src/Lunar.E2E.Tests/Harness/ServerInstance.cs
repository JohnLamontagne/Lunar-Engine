using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// A real Lunar.Server process on a free UDP port with its own copy of "Server Data" in a
    /// temporary directory, so tests never touch the checked-in content or each other.
    /// </summary>
    public sealed class ServerInstance : IDisposable
    {
        private readonly ManagedProcess _process;
        private readonly string _dataRoot;

        public int Port { get; }
        public string Output => _process.Output;
        public string DataRoot => _dataRoot;

        private ServerInstance(ManagedProcess process, int port, string dataRoot)
        {
            _process = process;
            Port = port;
            _dataRoot = dataRoot;
        }

        public static async Task<ServerInstance> StartAsync(string artifactDir)
        {
            string bin = Paths.ServerBin;
            string dll = Path.Combine(bin, "Lunar.Server.dll");
            if (!File.Exists(dll))
                throw new FileNotFoundException("Server build output not found. Build Lunar.Server first.", dll);

            string dataRoot = Path.Combine(Path.GetTempPath(), "lunar-e2e", Guid.NewGuid().ToString("N"));
            CopyDirectory(Path.Combine(bin, "Server Data"), Path.Combine(dataRoot, "Server Data"));

            int port = Ports.FreeUdpPort();
            var env = new Dictionary<string, string>
            {
                ["LUNAR_SERVER_PORT"] = port.ToString(),
                ["LUNAR_DATA_ROOT"] = dataRoot,
                ["DOTNET_ROLL_FORWARD"] = Environment.GetEnvironmentVariable("DOTNET_ROLL_FORWARD") ?? "Major"
            };

            var process = new ManagedProcess("server", "dotnet", new[] { dll }, bin, env, Path.Combine(artifactDir, "server.log"));
            var instance = new ServerInstance(process, port, dataRoot);

            try
            {
                await process.WaitForLineAsync(l => l.Contains("Server ready on port"), TimeSpan.FromSeconds(60));
            }
            catch
            {
                instance.Dispose();
                throw;
            }
            return instance;
        }

        private static void CopyDirectory(string source, string destination)
        {
            Directory.CreateDirectory(destination);
            foreach (var file in Directory.GetFiles(source))
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
            foreach (var dir in Directory.GetDirectories(source))
                CopyDirectory(dir, Path.Combine(destination, Path.GetFileName(dir)));
        }

        /// <summary>Number of output lines containing <paramref name="needle"/> so far.</summary>
        public int CountOutputLines(string needle)
        {
            int count = 0;
            foreach (var line in Output.Split('\n'))
                if (line.Contains(needle, StringComparison.Ordinal))
                    count++;
            return count;
        }

        /// <summary>Waits until at least <paramref name="minimum"/> output lines contain <paramref name="needle"/>.</summary>
        public async Task WaitForOutputCountAsync(string needle, int minimum, TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (CountOutputLines(needle) < minimum)
            {
                if (DateTime.UtcNow > deadline)
                    throw new TimeoutException($"Server output did not contain '{needle}' {minimum} time(s) within {timeout}.\n--- server output ---\n{Output}");
                await Task.Delay(100);
            }
        }

        /// <summary>Asks the server to shut down the way a container runtime would, and returns its exit code.</summary>
        public Task<int> StopGracefullyAsync(TimeSpan timeout) => _process.StopGracefullyAsync(timeout);

        public void Dispose()
        {
            _process.Dispose();
            try { Directory.Delete(_dataRoot, true); } catch { /* best effort */ }
        }
    }
}
