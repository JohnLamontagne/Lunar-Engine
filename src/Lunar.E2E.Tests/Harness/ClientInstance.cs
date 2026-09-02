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

        // ------------------------------------------------------------------ Observation

        public async Task<List<UiNode>> UiAsync()
        {
            var json = await _http.GetStringAsync("ui");
            return JsonSerializer.Deserialize<List<UiNode>>(json, Json);
        }

        /// <summary>Finds a widget by name anywhere in the active scene's tree, or null.</summary>
        public async Task<UiNode> FindAsync(string name)
        {
            static UiNode Search(IEnumerable<UiNode> nodes, string n)
            {
                foreach (var node in nodes)
                {
                    if (node.Name == n) return node;
                    if (node.Children != null)
                    {
                        var hit = Search(node.Children, n);
                        if (hit != null) return hit;
                    }
                }
                return null;
            }
            return Search(await UiAsync(), name);
        }

        public async Task<List<EntityState>> EntitiesAsync()
        {
            var json = await _http.GetStringAsync("entities");
            return JsonSerializer.Deserialize<List<EntityState>>(json, Json);
        }

        public async Task<List<string>> ChatAsync()
        {
            var json = await _http.GetStringAsync("chat");
            return JsonSerializer.Deserialize<List<string>>(json, Json);
        }

        // ------------------------------------------------------------------ Interaction (player paths)

        /// <summary>Clicks the centre of the named widget with a real (virtual) mouse click.</summary>
        public async Task ClickAsync(string widgetName, string button = "Left")
        {
            var response = await _http.PostAsJsonAsync("ui/click", new { name = widgetName, button });
            await EnsureUiActionAsync(response, $"click '{widgetName}'");
        }

        /// <summary>Clicks the named textbox to focus it, then types <paramref name="text"/> as key characters.</summary>
        public async Task TypeAsync(string widgetName, string text, bool enter = false)
        {
            var response = await _http.PostAsJsonAsync("ui/type", new { name = widgetName, text, enter });
            await EnsureUiActionAsync(response, $"type into '{widgetName}'");
        }

        /// <summary>Taps a key for one frame. Key names follow Microsoft.Xna.Framework.Input.Keys.</summary>
        public Task KeyTapAsync(string key, int frames = 1) => PostAsync("input/key", new { key, action = "tap", frames });

        public Task KeyDownAsync(string key) => PostAsync("input/key", new { key, action = "down" });

        public Task KeyUpAsync(string key) => PostAsync("input/key", new { key, action = "up" });

        /// <summary>Holds a key for the given duration; returns after it has been released.</summary>
        public Task KeyHoldAsync(string key, int durationMs) => PostAsync("input/key", new { key, action = "hold", durationMs });

        public Task MouseMoveAsync(int x, int y) => PostAsync("input/mouse", new { action = "move", x, y });

        public Task MouseClickAsync(int x, int y, string button = "Left") => PostAsync("input/mouse", new { action = "click", x, y, button });

        /// <summary>Delivers characters to whatever widget currently has focus.</summary>
        public Task TextAsync(string text) => PostAsync("input/text", new { text });

        public Task ResetInputAsync() => PostAsync("input/reset", new { });

        /// <summary>Runs a developer-console command; returns its success flag and output.</summary>
        public async Task<(bool Success, string Output)> CommandAsync(string text)
        {
            var response = await _http.PostAsJsonAsync("command", new { text });
            response.EnsureSuccessStatusCode();
            var doc = JsonSerializer.Deserialize<CommandResult>(await response.Content.ReadAsStringAsync(), Json);
            return (doc.Success, doc.Output);
        }

        // ------------------------------------------------------------------ Flows built from the primitives

        /// <summary>Fills in the menu's credential boxes and clicks Login, exactly as a player would.</summary>
        public async Task LoginAsync(string username, string password)
        {
            await TypeAsync("userLoginTextbox", username);
            await TypeAsync("userPasswordTextbox", password);
            await ClickAsync("btnLogin");
        }

        /// <summary>Fills in the menu's credential boxes and clicks Register.</summary>
        public async Task RegisterAsync(string username, string password)
        {
            await TypeAsync("userLoginTextbox", username);
            await TypeAsync("userPasswordTextbox", password);
            await ClickAsync("btnRegister");
        }

        /// <summary>Types a chat line into the in-world chat box and sends it with Enter.</summary>
        public Task SayAsync(string message) => TypeAsync("messageEntry", message, enter: true);

        private async Task PostAsync(string path, object body)
        {
            var response = await _http.PostAsJsonAsync(path, body);
            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"{path} failed ({(int)response.StatusCode}): {await response.Content.ReadAsStringAsync()}");
        }

        private static async Task EnsureUiActionAsync(HttpResponseMessage response, string what)
        {
            var body = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) return;
            throw new InvalidOperationException($"Could not {what}: {body}");
        }

        /// <summary>Polls <paramref name="probe"/> until it returns non-null (or true), or times out.</summary>
        public async Task<T> WaitForAsync<T>(Func<Task<T>> probe, TimeSpan timeout, string description)
        {
            var deadline = DateTime.UtcNow + timeout;
            while (DateTime.UtcNow < deadline)
            {
                var value = await probe();
                if (value is bool ok ? ok : value != null)
                    return value;
                await Task.Delay(200);
            }
            throw new TimeoutException($"Client did not reach '{description}' within {timeout}.\n--- client output ---\n{_process.Output}");
        }

        /// <summary>Polls the widget tree until the named widget satisfies <paramref name="predicate"/>.</summary>
        public Task<UiNode> WaitForUiAsync(string widgetName, Func<UiNode, bool> predicate, TimeSpan timeout, string description)
            => WaitForAsync(async () =>
            {
                var node = await FindAsync(widgetName);
                return node != null && predicate(node) ? node : null;
            }, timeout, description);

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
            public long Frame { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public EntityState Player { get; set; }
        }

        public sealed class EntityState
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public int Health { get; set; }
            public int MaximumHealth { get; set; }
            public bool IsLocal { get; set; }
        }

        public sealed class UiNode
        {
            public string Name { get; set; }
            public string Type { get; set; }
            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public bool Visible { get; set; }
            public bool Active { get; set; }
            public string Text { get; set; }
            public List<UiNode> Children { get; set; }
        }

        private sealed class CommandResult { public bool Success { get; set; } public string Output { get; set; } }
    }
}
