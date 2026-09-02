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
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Lunar.Client.Net;
using Lunar.Client.Scenes;
using Lunar.Client.World;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Client.Automation
{
    /// <summary>
    /// Test-automation endpoint hosted inside the running client. Enabled only when the
    /// <c>LUNAR_AUTOMATION_PORT</c> environment variable is set; never active in a normal launch.
    /// Listens on loopback only and exposes:
    /// <list type="bullet">
    /// <item><c>GET /health</c>: 200 once the game loop is running.</item>
    /// <item><c>GET /state</c>: JSON snapshot of scene, connection and player state.</item>
    /// <item><c>GET /screenshot</c>: PNG of the next rendered back buffer.</item>
    /// <item><c>POST /login</c>, <c>POST /register</c>: JSON body <c>{"username","password"}</c>.</item>
    /// <item><c>POST /quit</c>: exits the client.</item>
    /// </list>
    /// All game-state access is marshalled onto the game thread; the HTTP threads only wait.
    /// </summary>
    public sealed class AutomationServer : IDisposable
    {
        public const string PortEnvironmentVariable = "LUNAR_AUTOMATION_PORT";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
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

        public AutomationServer(Game game, IServiceProvider services, int port)
        {
            _game = game ?? throw new ArgumentNullException(nameof(game));
            _services = services ?? throw new ArgumentNullException(nameof(services));
            Port = port;
        }

        /// <summary>
        /// Reads <see cref="PortEnvironmentVariable"/>; returns null when automation is not requested.
        /// </summary>
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

        /// <summary>Called by the game once per Update on the game thread.</summary>
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

            try
            {
                capture.TrySetResult(CaptureBackBufferPng(device));
            }
            catch (Exception ex)
            {
                capture.TrySetException(ex);
            }
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

        private async Task AcceptLoop()
        {
            while (!_shutdown.IsCancellationRequested)
            {
                HttpListenerContext context;
                try
                {
                    context = await _listener.GetContextAsync().ConfigureAwait(false);
                }
                catch (Exception) when (_shutdown.IsCancellationRequested)
                {
                    return;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[Automation] accept failed: {ex.Message}");
                    continue;
                }

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

                switch (path)
                {
                    case "/health":
                        await WriteText(response, _loopRunning ? 200 : 503, _loopRunning ? "ok" : "starting");
                        break;

                    case "/state":
                        var state = await this.RunOnGameThread(this.BuildState);
                        await WriteJson(response, 200, state);
                        break;

                    case "/screenshot":
                        var png = await this.RequestCapture();
                        response.StatusCode = 200;
                        response.ContentType = "image/png";
                        response.ContentLength64 = png.Length;
                        await response.OutputStream.WriteAsync(png, 0, png.Length);
                        break;

                    case "/login":
                    case "/register":
                        if (request.HttpMethod != "POST")
                        {
                            await WriteText(response, 405, "POST required");
                            break;
                        }
                        var credentials = await ReadJson<Credentials>(request);
                        bool register = path == "/register";
                        await this.RunOnGameThread(() =>
                        {
                            var menu = _services.GetRequiredService<MenuScene>();
                            menu.Authenticate(credentials.Username, credentials.Password, register);
                            return true;
                        });
                        await WriteJson(response, 200, new { accepted = true, action = path.Trim('/') });
                        break;

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

        private Task<byte[]> RequestCapture()
        {
            var tcs = new TaskCompletionSource<byte[]>(TaskCreationOptions.RunContinuationsAsynchronously);
            var existing = Interlocked.CompareExchange(ref _pendingCapture, tcs, null);
            var awaited = existing ?? tcs;
            return WithTimeout(awaited.Task, TimeSpan.FromSeconds(10), "screenshot");
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

        private static async Task<T> WithTimeout<T>(Task<T> task, TimeSpan timeout, string what)
        {
            var completed = await Task.WhenAny(task, Task.Delay(timeout)).ConfigureAwait(false);
            if (completed != task)
                throw new TimeoutException($"Timed out waiting for {what}; is the game loop running?");
            return await task.ConfigureAwait(false);
        }

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
                Width = _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                Height = _game.GraphicsDevice.PresentationParameters.BackBufferHeight,
                Player = player == null ? null : new PlayerState
                {
                    Name = player.Name,
                    X = player.Position.X,
                    Y = player.Position.Y,
                    Health = player.Health,
                    MaximumHealth = player.MaximumHealth
                }
            };
        }

        private static async Task<T> ReadJson<T>(HttpListenerRequest request)
        {
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding ?? Encoding.UTF8))
            {
                var body = await reader.ReadToEndAsync();
                return JsonSerializer.Deserialize<T>(body, JsonOptions)
                       ?? throw new InvalidDataException("Empty JSON body.");
            }
        }

        private static Task WriteJson(HttpListenerResponse response, int status, object value)
        {
            return WriteBytes(response, status, "application/json", JsonSerializer.SerializeToUtf8Bytes(value, value.GetType(), JsonOptions));
        }

        private static Task WriteText(HttpListenerResponse response, int status, string text)
        {
            return WriteBytes(response, status, "text/plain; charset=utf-8", Encoding.UTF8.GetBytes(text));
        }

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

        private sealed class Credentials
        {
            public string Username { get; set; }
            public string Password { get; set; }
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
