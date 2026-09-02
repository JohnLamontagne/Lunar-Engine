/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lunar.Client.GUI;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Net;
using Lunar.Client.Scenes;
using Lunar.Client.Utilities.Input;
using Lunar.Client.World;
using Lunar.Client.World.Actors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.Automation
{
    /// <summary>
    /// Test-automation endpoint hosted inside the running client. Enabled only when the
    /// <c>LUNAR_AUTOMATION_PORT</c> environment variable is set; never active in a normal launch.
    /// Listens on loopback only.
    ///
    /// The design rule: automation drives the client the way a player does. Input goes through
    /// <see cref="Input"/> so it takes the same path as hardware; UI actions locate real widgets and
    /// click or type into them; nothing here calls gameplay methods directly. Observation endpoints
    /// expose what is on screen (widget tree, entities, chat, frames) so tests can assert on it.
    ///
    /// <list type="bullet">
    /// <item><c>GET /health</c>, <c>GET /state</c>, <c>GET /screenshot</c></item>
    /// <item><c>GET /ui</c>: widget tree of the active scene. <c>POST /ui/click {name}</c>, <c>POST /ui/type {name,text,enter}</c></item>
    /// <item><c>POST /input/key {key, action: down|up|tap|hold, frames, durationMs}</c></item>
    /// <item><c>POST /input/mouse {action: move|down|up|click, x, y, button}</c>, <c>POST /input/text {text}</c>, <c>POST /input/reset</c></item>
    /// <item><c>GET /entities</c>, <c>GET /chat</c></item>
    /// <item><c>POST /command {text}</c>: developer console command (when the platform provides one)</item>
    /// <item><c>POST /quit</c></item>
    /// </list>
    /// </summary>
    public sealed class AutomationServer : IDisposable
    {
        public const string PortEnvironmentVariable = "LUNAR_AUTOMATION_PORT";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };

        private readonly Game _game;
        private readonly IServiceProvider _services;
        private readonly HttpListener _listener = new HttpListener();
        private readonly ConcurrentQueue<Action> _gameThreadActions = new ConcurrentQueue<Action>();
        private readonly CancellationTokenSource _shutdown = new CancellationTokenSource();

        private TaskCompletionSource<byte[]> _pendingCapture;
        private long _framesRendered;
        private volatile bool _loopRunning;

        public int Port { get; }

        /// <summary>
        /// Optional developer-console bridge supplied by the platform layer: input text in, (success, output) out.
        /// </summary>
        public Func<string, (bool Success, string Output)> CommandHandler { get; set; }

        public AutomationServer(Game game, IServiceProvider services, int port)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            Port = port;
        }

        public static int? PortFromEnvironment()
        {
            var raw = Environment.GetEnvironmentVariable(PortEnvironmentVariable);
            return int.TryParse(raw, out var port) && port > 0 && port < 65536 ? port : (int?)null;
        }

        public void Start()
        {
            _listener.Prefixes.Add($"http://127.0.0.1:{Port}/");
            _listener.Start();
            Task.Run(this.AcceptLoop);
            Console.WriteLine($"[Automation] listening on http://127.0.0.1:{Port}/");
        }

        /// <summary>Called by the game once per Update on the game thread, after input has been sampled.</summary>
        public void OnUpdate()
        {
            _loopRunning = true;

            while (_gameThreadActions.TryDequeue(out var action))
            {
                try { action(); }
                catch (Exception ex) { Console.WriteLine($"[Automation] action failed: {ex}"); }
            }
        }

        /// <summary>Called by the game at the end of Draw, before Present, on the game thread.</summary>
        public void OnFrameRendered(GraphicsDevice device)
        {
            Interlocked.Increment(ref _framesRendered);

            var capture = Interlocked.Exchange(ref _pendingCapture, null);
            if (capture == null)
                return;

            try { capture.TrySetResult(CaptureBackBufferPng(device)); }
            catch (Exception ex) { capture.TrySetException(ex); }
        }

        private static byte[] CaptureBackBufferPng(GraphicsDevice device)
        {
            int width = device.PresentationParameters.BackBufferWidth;
            int height = device.PresentationParameters.BackBufferHeight;

            var pixels = new Color[width * height];
            device.GetBackBufferData(pixels);

            using (var texture = new Texture2D(device, width, height))
            using (var stream = new MemoryStream())
            {
                texture.SetData(pixels);
                texture.SaveAsPng(stream, width, height);
                return stream.ToArray();
            }
        }

        // ------------------------------------------------------------------ HTTP plumbing

        private async Task AcceptLoop()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                HttpListenerContext context;
                try { context = await _listener.GetContextAsync().ConfigureAwait(false); }
                catch (Exception) when (_shutdown.IsCancellationRequested) { return; }
                catch (Exception ex) { Console.WriteLine($"[Automation] accept failed: {ex.Message}"); continue; }

                _ = Task.Run(() => this.Handle(context));
            }
        }

        private async Task Handle(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                string path = request.Url?.AbsolutePath?.TrimEnd('/') ?? string.Empty;
                bool isPost = request.HttpMethod == "POST";

                switch (path)
                {
                    case "/health":
                        await WriteText(response, _loopRunning ? 200 : 503, _loopRunning ? "ok" : "starting");
                        break;

                    case "/state":
                        await WriteJson(response, 200, await this.RunOnGameThread(this.BuildState));
                        break;

                    case "/screenshot":
                        var png = await this.RequestCapture();
                        await WriteBytes(response, 200, "image/png", png);
                        break;

                    case "/ui":
                        await WriteJson(response, 200, await this.RunOnGameThread(this.BuildUiTree));
                        break;

                    case "/ui/click":
                    {
                        if (!isPost) { await WriteText(response, 405, "POST required"); break; }
                        var req = await ReadJson<UiActionRequest>(request);
                        var result = await this.RunOnGameThread(() => this.ClickWidget(req.Name, ParseButton(req.Button)));
                        await WriteJson(response, result.Found ? 200 : 404, result);
                        break;
                    }

                    case "/ui/type":
                    {
                        if (!isPost) { await WriteText(response, 405, "POST required"); break; }
                        var req = await ReadJson<UiActionRequest>(request);
                        var result = await this.RunOnGameThread(() => this.ClickWidget(req.Name, MouseButtons.Left));
                        if (!result.Found) { await WriteJson(response, 404, result); break; }

                        // Let the click land (one frame) so the textbox is active before characters arrive.
                        await this.WaitFrames(2);
                        Input.Virtual.Text(req.Text ?? string.Empty);
                        if (req.Enter)
                        {
                            // Textboxes ignore special keys for a short cooldown after activation, so that the
                            // keystroke which focused them cannot also submit. Wait it out like a person would.
                            await Task.Delay(300);
                            await this.WaitFrames(1);
                            Input.Virtual.Tap(Keys.Enter);
                        }
                        await this.WaitFrames(2);
                        await WriteJson(response, 200, result);
                        break;
                    }

                    case "/input/key":
                    {
                        if (!isPost) { await WriteText(response, 405, "POST required"); break; }
                        var req = await ReadJson<KeyRequest>(request);
                        if (!Enum.TryParse<Keys>(req.Key ?? string.Empty, true, out var key))
                        {
                            await WriteText(response, 400, $"Unknown key '{req.Key}'. Use Microsoft.Xna.Framework.Input.Keys names.");
                            break;
                        }
                        switch ((req.Action ?? "tap").ToLowerInvariant())
                        {
                            case "down": Input.Virtual.KeyDown(key); break;
                            case "up": Input.Virtual.KeyUp(key); break;
                            case "tap": Input.Virtual.Tap(key, req.Frames > 0 ? req.Frames : 1); break;
                            case "hold":
                                Input.Virtual.KeyDown(key);
                                await Task.Delay(Math.Max(0, req.DurationMs));
                                Input.Virtual.KeyUp(key);
                                break;
                            default:
                                await WriteText(response, 400, "action must be down, up, tap or hold");
                                return;
                        }
                        await this.WaitFrames(2);
                        await WriteJson(response, 200, new { ok = true, key = key.ToString(), action = req.Action });
                        break;
                    }

                    case "/input/mouse":
                    {
                        if (!isPost) { await WriteText(response, 405, "POST required"); break; }
                        var req = await ReadJson<MouseRequest>(request);
                        var button = ParseButton(req.Button);
                        switch ((req.Action ?? "move").ToLowerInvariant())
                        {
                            case "move": Input.Virtual.MouseMove(req.X, req.Y); break;
                            case "down": Input.Virtual.MouseMove(req.X, req.Y); Input.Virtual.MouseButton(button, true); break;
                            case "up": Input.Virtual.MouseButton(button, false); break;
                            case "click": Input.Virtual.Click(req.X, req.Y, button); break;
                            default:
                                await WriteText(response, 400, "action must be move, down, up or click");
                                return;
                        }
                        await this.WaitFrames(2);
                        await WriteJson(response, 200, new { ok = true });
                        break;
                    }

                    case "/input/text":
                    {
                        if (!isPost) { await WriteText(response, 405, "POST required"); break; }
                        var req = await ReadJson<TextRequest>(request);
                        Input.Virtual.Text(req.Text ?? string.Empty);
                        await this.WaitFrames(2);
                        await WriteJson(response, 200, new { ok = true, length = (req.Text ?? string.Empty).Length });
                        break;
                    }

                    case "/input/reset":
                        Input.Virtual.Reset();
                        await this.WaitFrames(1);
                        await WriteJson(response, 200, new { ok = true });
                        break;

                    case "/entities":
                        await WriteJson(response, 200, await this.RunOnGameThread(this.BuildEntities));
                        break;

                    case "/chat":
                        await WriteJson(response, 200, await this.RunOnGameThread(this.BuildChat));
                        break;

                    case "/command":
                    {
                        if (!isPost) { await WriteText(response, 405, "POST required"); break; }
                        var req = await ReadJson<TextRequest>(request);
                        if (CommandHandler == null) { await WriteText(response, 501, "No command handler on this platform."); break; }
                        var (success, output) = await this.RunOnGameThread(() => CommandHandler(req.Text ?? string.Empty));
                        await WriteJson(response, 200, new { success, output });
                        break;
                    }

                    case "/quit":
                        _gameThreadActions.Enqueue(() => _game.Exit());
                        await WriteText(response, 200, "quitting");
                        break;

                    default:
                        await WriteText(response, 404, "unknown endpoint");
                        break;
                }
            }
            catch (Exception ex)
            {
                try { await WriteText(response, 500, ex.ToString()); } catch { /* client went away */ }
            }
            finally
            {
                try { response.Close(); } catch { /* ignore */ }
            }
        }

        // ------------------------------------------------------------------ Game-thread helpers

        private Task<byte[]> RequestCapture()
        {
            var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            var existing = Interlocked.CompareExchange(ref _pendingCapture, tcs, null);
            return WithTimeout((existing ?? tcs).Task, TimeSpan.FromSeconds(10), "screenshot");
        }

        private Task<T> RunOnGameThread<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
            _gameThreadActions.Enqueue(() =>
            {
                try { tcs.TrySetResult(func()); }
                catch (Exception ex) { tcs.TrySetException(ex); }
            });
            return WithTimeout(tcs.Task, TimeSpan.FromSeconds(10), "game thread action");
        }

        /// <summary>Resolves after the game loop has processed at least <paramref name="frames"/> more updates.</summary>
        private async Task WaitFrames(int frames)
        {
            long target = Input.Frame + frames;
            var deadline = DateTime.UtcNow + TimeSpan.FromSeconds(10);
            while (Input.Frame < target)
            {
                if (DateTime.UtcNow > deadline)
                    throw new TimeoutException("Game loop stopped advancing frames.");
                await Task.Delay(5);
            }
        }

        private static async Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeout, string what)
        {
            var completed = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);
            if (completed != task)
                throw new TimeoutException($"Timed out waiting for {what}; is the game loop running?");
            return await task.ConfigureAwait(false);
        }

        private static MouseButtons ParseButton(string button)
        {
            return Enum.TryParse<MouseButtons>(button ?? "Left", true, out var b) ? b : MouseButtons.Left;
        }

        // ------------------------------------------------------------------ Observation

        private ClientState BuildState()
        {
            var sceneManager = _services.GetRequiredService<SceneManager>();
            var netHandler = _services.GetRequiredService<NetHandler>();
            var worldManager = _services.GetRequiredService<WorldManager>();
            var menu = _services.GetRequiredService<MenuScene>();
            var player = worldManager.Player;

            return new ClientState
            {
                Scene = sceneManager.ActiveSceneName,
                Connected = netHandler.Connected,
                StatusText = menu.StatusText,
                FramesRendered = Interlocked.Read(ref _framesRendered),
                Frame = Input.Frame,
                Width = _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = _game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                Mouse = new PointState { X = Input.Mouse.X, Y = Input.Mouse.Y },
                Player = player == null ? null : new EntityState
                {
                    Id = player.UniqueID,
                    Name = player.Name,
                    Type = "Player",
                    X = player.Position.X,
                    Y = player.Position.Y,
                    Health = player.Health,
                    MaximumHealth = player.MaximumHealth,
                    IsLocal = true
                }
            };
        }

        private GUIManager ActiveGui()
        {
            var scene = _services.GetRequiredService<SceneManager>().ActiveScreen;
            return scene?.Gui;
        }

        private List<UiNode> BuildUiTree()
        {
            var gui = ActiveGui();
            return gui == null ? new List<UiNode>() : BuildNodes(gui);
        }

        private static List<UiNode> BuildNodes(WidgetCollection collection)
        {
            var nodes = new List<UiNode>();
            foreach (var entry in collection.GetWidgetEntries().OrderBy(e => e.Value.ZOrder))
            {
                var w = entry.Value;
                var bounds = w.Bounds;
                var node = new UiNode
                {
                    Name = entry.Key,
                    Type = w.GetType().Name,
                    X = bounds.X, Y = bounds.Y, Width = bounds.Width, Height = bounds.Height,
                    Visible = w.Visible,
                    Active = w.Active,
                    Text = TextOf(w),
                };
                if (w is WidgetCollection children)
                    node.Children = BuildNodes(children);
                nodes.Add(node);
            }
            return nodes;
        }

        private static string TextOf(IWidget widget)
        {
            switch (widget)
            {
                case Label l: return l.Text;
                case Textbox t: return t.Text;
                case Button b: return b.Text;
                default: return null;
            }
        }

        /// <summary>Depth-first search by widget name across nested containers.</summary>
        private static IWidget FindWidget(WidgetCollection collection, string name, out bool visibleChain)
        {
            visibleChain = true;
            foreach (var entry in collection.GetWidgetEntries())
            {
                if (entry.Key == name)
                {
                    visibleChain = entry.Value.Visible;
                    return entry.Value;
                }
                if (entry.Value is WidgetCollection children)
                {
                    var found = FindWidget(children, name, out var childVisible);
                    if (found != null)
                    {
                        visibleChain = childVisible && entry.Value.Visible;
                        return found;
                    }
                }
            }
            return null;
        }

        private UiActionResult ClickWidget(string name, MouseButtons button)
        {
            var gui = ActiveGui();
            var widget = gui == null ? null : FindWidget(gui, name, out var visible);
            if (widget == null)
                return new UiActionResult { Found = false, Name = name, Error = "No widget with that name in the active scene." };

            var b = widget.Bounds;
            if (b.Width <= 0 || b.Height <= 0)
                return new UiActionResult { Found = true, Name = name, Error = "Widget has no clickable area." };
            if (!widget.Visible)
                return new UiActionResult { Found = true, Name = name, Error = "Widget is not visible." };

            int x = b.X + b.Width / 2;
            int y = b.Y + b.Height / 2;
            Input.Virtual.Click(x, y, button);
            return new UiActionResult { Found = true, Name = name, X = x, Y = y };
        }

        private List<EntityState> BuildEntities()
        {
            var worldManager = _services.GetRequiredService<WorldManager>();
            var local = worldManager.Player;
            var list = new List<EntityState>();
            if (worldManager.Map == null)
                return list;

            foreach (var entity in worldManager.Map.GetEntities())
            {
                switch (entity)
                {
                    case Player p:
                        list.Add(new EntityState { Id = p.UniqueID, Name = p.Name, Type = "Player", X = p.Position.X, Y = p.Position.Y, Health = p.Health, MaximumHealth = p.MaximumHealth, IsLocal = ReferenceEquals(p, local) });
                        break;
                    case NPC n:
                        list.Add(new EntityState { Id = n.UniqueID, Name = n.Name, Type = "NPC", X = n.Position.X, Y = n.Position.Y, Health = n.Health, MaximumHealth = n.MaximumHealth });
                        break;
                    default:
                        list.Add(new EntityState { Type = entity.GetType().Name, X = entity.Position.X, Y = entity.Position.Y });
                        break;
                }
            }
            return list;
        }

        private List<string> BuildChat()
        {
            var gui = ActiveGui();
            var chat = gui == null ? null : FindWidget(gui, "chatbox", out _) as Chatbox;
            if (chat == null)
                return new List<string>();

            // Entries are labels stacked bottom-up; return them oldest first (top to bottom on screen).
            return chat.GetWidgets<Label>().OrderBy(l => l.Position.Y).Select(l => l.Text).ToList();
        }

        // ------------------------------------------------------------------ JSON I/O

        private static async Task<T> ReadJson<T>(HttpListenerRequest request) where T : new()
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
            {
                var body = await reader.ReadToEndAsync();
                if (string.IsNullOrWhiteSpace(body)) return new T();
                return JsonSerializer.Deserialize<T>(body, JsonOptions) ?? new T();
            }
        }

        private static Task WriteJson(HttpListenerResponse response, int status, object value)
            => WriteBytes(response, status, "application/json", JsonSerializer.SerializeToUtf8Bytes(value, value.GetType(), JsonOptions));

        private static Task WriteText(HttpListenerResponse response, int status, string text)
            => WriteBytes(response, status, "text/plain; charset=utf-8", Encoding.UTF8.GetBytes(text));

        private static async Task WriteBytes(HttpListenerResponse response, int status, string contentType, byte[] bytes)
        {
            response.StatusCode = status;
            response.ContentType = contentType;
            response.ContentLength64 = bytes.Length;
            await response.OutputStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public void Dispose()
        {
            _shutdown.Cancel();
            try { _listener.Stop(); _listener.Close(); } catch { /* ignore */ }
        }

        // ------------------------------------------------------------------ Wire types

        private sealed class UiActionRequest { public string Name { get; set; } public string Text { get; set; } public bool Enter { get; set; } public string Button { get; set; } }
        private sealed class KeyRequest { public string Key { get; set; } public string Action { get; set; } public int Frames { get; set; } public int DurationMs { get; set; } }
        private sealed class MouseRequest { public string Action { get; set; } public int X { get; set; } public int Y { get; set; } public string Button { get; set; } }
        private sealed class TextRequest { public string Text { get; set; } }

        public sealed class UiActionResult { public bool Found { get; set; } public string Name { get; set; } public int X { get; set; } public int Y { get; set; } public string Error { get; set; } }

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

        public sealed class PointState { public int X { get; set; } public int Y { get; set; } }

        public sealed class ClientState
        {
            public string Scene { get; set; }
            public bool Connected { get; set; }
            public string StatusText { get; set; }
            public long FramesRendered { get; set; }
            public long Frame { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }
            public PointState Mouse { get; set; }
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
    }
}
