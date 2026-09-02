using System;
using System.IO;
using System.Threading.Tasks;
using Lunar.E2E.Tests.Harness;
using Xunit;

namespace Lunar.E2E.Tests
{
    /// <summary>
    /// Boot, register and login flows exercised against a real server with a real rendering client.
    /// Registration on this server logs the new account straight into the world.
    /// </summary>
    [Trait("Category", "E2E")]
    public class MenuAndLoginTests : IClassFixture<E2EFixture>
    {
        private static readonly TimeSpan Wait = TimeSpan.FromSeconds(30);
        private readonly E2EFixture _fx;

        public MenuAndLoginTests(E2EFixture fx) => _fx = fx;

        private static string NewUser() => "e2e_" + Guid.NewGuid().ToString("N").Substring(0, 8);

        [Fact]
        public async Task Client_boots_into_the_menu_and_renders_it()
        {
            var dir = _fx.ArtifactDir(nameof(Client_boots_into_the_menu_and_renders_it));
            using var client = await _fx.StartClientAsync(dir);

            var state = await client.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu scene rendering");
            Assert.Equal(client.Width, state.Width);
            Assert.Equal(client.Height, state.Height);

            using var frame = await client.ScreenshotAsync(Path.Combine(dir, "menu.png"));
            Assert.Equal(client.Width, frame.Width);
            Assert.Equal(client.Height, frame.Height);

            // The menu has a background and widgets: the frame must not be blank and must contain
            // more than a handful of colours.
            Assert.True(frame.LitFraction() > 0.05, $"Frame is almost entirely black (lit fraction {frame.LitFraction():F3}).");
            Assert.True(frame.DistinctColorBuckets() > 8, $"Frame has too few colours ({frame.DistinctColorBuckets()}) to be the menu.");

            // Pixel-level regression against the committed golden; loose tolerance for font rendering.
            Golden.AssertMatches("menu", frame, maxMeanDifference: 6.0, artifactDir: dir);
        }

        [Fact]
        public async Task Registering_enters_the_world_on_the_spawn_tile()
        {
            var dir = _fx.ArtifactDir(nameof(Registering_enters_the_world_on_the_spawn_tile));
            using var client = await _fx.StartClientAsync(dir);
            string user = NewUser();

            await client.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");
            await client.RegisterAsync(user, "correct horse battery staple");

            var state = await client.WaitForStateAsync(
                s => s.Scene == "gameScene" && s.Connected && s.Player != null && s.Player.Name == user,
                Wait, "game scene with our player");

            // The default map's player spawn is tile (10, 7) => pixels (320, 224).
            Assert.Equal(320f, state.Player.X);
            Assert.Equal(224f, state.Player.Y);
            Assert.Equal(state.Player.MaximumHealth, state.Player.Health);

            // Let the world draw a few frames before capturing.
            await client.WaitForStateAsync(s => s.FramesRendered > state.FramesRendered + 30, Wait, "world frames");
            using var frame = await client.ScreenshotAsync(Path.Combine(dir, "in-world.png"));
            Assert.True(frame.LitFraction() > 0.05, $"World frame is almost entirely black (lit fraction {frame.LitFraction():F3}).");
            Golden.AssertMatches("world-spawn", frame, maxMeanDifference: 6.0, artifactDir: dir);

            // Server-side confirmation: the account file exists in this run's private data root.
            var accounts = Path.Combine(_fx.Server.DataRoot, "Server Data", "World", "Accounts");
            Assert.True(Directory.Exists(accounts) && Directory.GetFiles(accounts, user + ".*").Length == 1,
                $"Expected one account file for {user} under {accounts}.\n--- server output ---\n{_fx.Server.Output}");
        }

        [Fact]
        public async Task Logging_in_with_an_existing_account_from_a_new_client_enters_the_world()
        {
            var dir = _fx.ArtifactDir(nameof(Logging_in_with_an_existing_account_from_a_new_client_enters_the_world));
            string user = NewUser();
            const string password = "hunter22";

            // First client: create the account (which logs it in), then quit so the server saves it.
            int disconnectsBefore = _fx.Server.CountOutputLines("lost (");
            using (var first = await _fx.StartClientAsync(dir, "client-register"))
            {
                await first.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");
                await first.RegisterAsync(user, password);
                await first.WaitForStateAsync(s => s.Scene == "gameScene" && s.Player?.Name == user, Wait, "registered and in world");
            }

            // A clean client exit must release the account on the server straight away.
            await _fx.Server.WaitForOutputCountAsync("lost (", disconnectsBefore + 1, TimeSpan.FromSeconds(10));

            // Second client: plain login with the same credentials.
            using var second = await _fx.StartClientAsync(dir, "client-login");
            await second.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");
            await second.LoginAsync(user, password);

            var state = await second.WaitForStateAsync(
                s => s.Scene == "gameScene" && s.Connected && s.Player?.Name == user,
                Wait, "logged in and in world");

            using var frame = await second.ScreenshotAsync(Path.Combine(dir, "logged-in.png"));
            Assert.True(frame.LitFraction() > 0.05);
            Assert.Equal(user, state.Player.Name);
        }

        [Fact]
        public async Task Logging_in_with_a_wrong_password_stays_on_the_menu_with_an_error()
        {
            var dir = _fx.ArtifactDir(nameof(Logging_in_with_a_wrong_password_stays_on_the_menu_with_an_error));
            using var client = await _fx.StartClientAsync(dir);

            await client.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");
            await client.LoginAsync(NewUser(), "nope");

            var state = await client.WaitForStateAsync(
                s => !string.IsNullOrEmpty(s.StatusText), Wait, "failure status text");

            Assert.Equal("menuScene", state.Scene);
            Assert.Contains("does not exist", state.StatusText, StringComparison.OrdinalIgnoreCase);
            Assert.True(state.Connected, "A failed login should leave the connection open so the player can retry.");
            using var frame = await client.ScreenshotAsync(Path.Combine(dir, "login-failed.png"));
            Assert.True(frame.LitFraction() > 0.05);
        }
    }
}
