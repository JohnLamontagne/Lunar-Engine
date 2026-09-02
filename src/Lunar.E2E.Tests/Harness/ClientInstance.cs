using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// A real Lunar.Client.Desktop process rendering on a virtual display, driven through its
    /// automation endpoint (enabled by LUNAR_AUTOMATION_PORT).
    /// </summary>
    public sealed class ClientInstance : IDisposable
    {
        private static readonly JsonSerializerOptions Json = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly XvfbDisplay _display;
        private readonly ManagedProcess _process;
        private readonly HttpClient _http;

        public int Width { get; }
        public int Height { get; }
        public string Output => _process.Output;

        private ClientInstance(XvfbDisplay display, ManagedProcess process, HttpClient http, int width, int height)
        {
            _display = display;
            _process = process;
            _http = http;
            Width = width;
            Height = height;
        }

        public static async Task<ClientInstance> StartAsync(string artifactDir, string serverHost, int serverPort, int width = 1280, int height = 720, string name = "client")
        {
            string bin = Paths.ClientBin;
            string dll = Path.Combine(bin, "Lunar.Client.Desktop.dll");
            if (!File.Exists(dll))
                throw new FileNotFoundException("Client build output not found. Build Lunar.Client.Desktop first.", dll);

            var display = new XvfbDisplay(width, height, Path.Combine(artifactDir, name + ".xvfb.log"));
            int automationPort = Ports.FreeTcpPort();

            var env = new Dictionary<string, string>
            {
                ["LUNAR_AUTOMATION_PORT"] = automationPort.ToString(),
                ["LUNAR_SERVER_HOST"] = serverHost,
                ["LUNAR_SERVER_PORT"] = serverPort.ToString(),
                ["LUNAR_RESOLUTION"] = $"{width}x{height}",
                ["DOTNET_ROLL_FORWARD"] = Environment.GetEnvironmentVariable("DOTNET_ROLL_FORWARD") ?? "Major"
            };
            display.ApplyTo(env);

            ManagedProcess process = null;
            try
            {
                process = new ManagedProcess(name, "dotnet", new[] { dll }, bin, env, Path.Combine(artifactDir, name + ".log"));
                var http = new HttpClient { BaseAddress = new Uri($"http://127.0.0.1:{automationPort}/"), Timeout = TimeSpan.FromSeconds(30) };
                var instance = new ClientInstance(display, process, http, width, height);
                await instance.WaitForHealthyAsync(TimeSpan.FromSeconds(90));
                return instance;
            }
            catch
            {
                process?.Dispose();
                display.Dispose();
                throw;
            }
        }

        private async Task WaitForHealthyAsync(TimeSpan timeout)
        {
            var deadline = DateTime.UtcNow + timeout;
            Exception last = null;
            while (DateTime.UtcNow < deadline)
            {
                if (_process.HasExited)
                    throw new InvalidOperationException($"Client exited with code {_process.ExitCode} during startup.\n--- client output ---\n{_process.Output}");
                try
                {
                    var response = await _http.GetAsync("health");
                    if (response.IsSuccessStatusCode)
                        return;
                }
                catch (Exception ex) { last = ex; }
                await Task.Delay(250);
            }
            throw new TimeoutException($"Client automation endpoint did not become healthy within {timeout}. Last error: {last?.Message}\n--- client output ---\n{_process.Output}");
        }

        public async Task<ClientState> GetStateAsync()
        {
            var json = await _http.GetStringAsync("state");
            return JsonSerializer.Deserialize<ClientState>(json, Json);
        }

        public async Task<Frame> ScreenshotAsync(string savePath = null)
        {
            var bytes = await _http.GetByteArrayAsync("screenshot");
            if (savePath != null)
                await File.WriteAllBytesAsync(savePath, bytes);
            return Frame.FromPng(bytes);
        }

        public Task LoginAsync(string username, string password) => PostAsync("login", username, password);

        public Task RegisterAsync(string username, string password) => PostAsync("register", username, password);

        private async Task PostAsync(string path, string username, string password)
        {
            var response = await _http.PostAsJsonAsync(path, new { username, password });
            response.EnsureSuccessStatusCode();
        }

        /// <summary>Polls state until <paramref name="predicate"/> holds or the timeout elapses.</summary>
        public async Task<ClientState> WaitForStateAsync(Func<ClientState, bool> predicate, TimeSpan timeout, string description)
        {
            var deadline = DateTime.UtcNow + timeout;
            ClientState last = null;
            while (DateTime.UtcNow < deadline)
            {
                last = await GetStateAsync();
                if (predicate(last))
                    return last;
                await Task.Delay(200);
            }
            throw new TimeoutException($"Client did not reach '{description}' within {timeout}. Last state: {JsonSerializer.Serialize(last)}\n--- client output ---\n{_process.Output}");
        }

        public void Dispose()
        {
            // Ask the client to exit through its normal path (which disconnects from the server) and
            // give it a moment to do so; only then fall back to killing the process tree.
            try { _http.PostAsync("quit", null).Wait(TimeSpan.FromSeconds(3)); } catch { /* may already be gone */ }
            _process.WaitForExit(TimeSpan.FromSeconds(10));
            _http.Dispose();
            _process.Dispose();
            _display.Dispose();
        }

        public sealed class ClientState
        {
            public string Scene { get; set; }
            public bool Connected { get; set; }
            public string StatusText { get; set; }
            public long FramesRendered { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public PlayerState Player { get; set; }
        }

        public sealed class PlayerState
        {
            public string Name { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public int Health { get; set; }
            public int MaximumHealth { get; set; }
        }
    }
}
