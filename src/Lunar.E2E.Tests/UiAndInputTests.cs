using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Lunar.E2E.Tests.Harness;
using Xunit;

namespace Lunar.E2E.Tests
{
    /// <summary>
    /// Exercises the generic interaction surface: the widget tree, virtual keyboard and mouse, chat.
    /// Every action here goes through the same input path a player's hardware uses.
    /// </summary>
    [Trait("Category", "E2E")]
    public class UiAndInputTests : IClassFixture<E2EFixture>
    {
        private static readonly TimeSpan Wait = TimeSpan.FromSeconds(30);
        private readonly E2EFixture _fx;

        public UiAndInputTests(E2EFixture fx) => _fx = fx;

        private static string NewUser() => "e2e_" + Guid.NewGuid().ToString("N").Substring(0, 8);

        private async Task<ClientInstance> EnterWorldAsync(string dir, string name, string user)
        {
            var client = await _fx.StartClientAsync(dir, name);
            await client.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");
            await client.RegisterAsync(user, "pw");
            await client.WaitForStateAsync(s => s.Scene == "gameScene" && s.Player?.Name == user, Wait, "in world");
            return client;
        }

        [Fact]
        public async Task Menu_widget_tree_exposes_the_login_controls_with_real_bounds()
        {
            var dir = _fx.ArtifactDir(nameof(Menu_widget_tree_exposes_the_login_controls_with_real_bounds));
            using var client = await _fx.StartClientAsync(dir);
            await client.WaitForStateAsync(s => s.Scene == "menuScene" && s.FramesRendered > 10, Wait, "menu ready");

            var tree = await client.UiAsync();
            Assert.NotEmpty(tree);

            // The status label exists but is empty until something happens, so it has no area yet.
            var status = await client.FindAsync("lblStatus");
            Assert.True(status != null, "Widget 'lblStatus' not found in the menu tree.");
            Assert.Equal("Label", status.Type);

            foreach (var name in new[] { "userLoginTextbox", "userPasswordTextbox", "btnLogin", "btnRegister" })
            {
                var node = await client.FindAsync(name);
                Assert.True(node != null, $"Widget '{name}' not found in the menu tree.");
                Assert.True(node.Visible, $"Widget '{name}' should be visible.");
                Assert.True(node.Width > 0 && node.Height > 0, $"Widget '{name}' has empty bounds.");
                Assert.True(node.X >= 0 && node.Y >= 0 && node.X + node.Width <= client.Width && node.Y + node.Height <= client.Height,
                    $"Widget '{name}' bounds {node.X},{node.Y} {node.Width}x{node.Height} fall outside the {client.Width}x{client.Height} frame.");
            }

            // Typing through the real focus path lands in the textbox.
            await client.TypeAsync("userLoginTextbox", "typed_by_test");
            var box = await client.WaitForUiAsync("userLoginTextbox", n => n.Text == "typed_by_test", Wait, "typed text visible in textbox");
            Assert.Equal("typed_by_test", box.Text);
        }

        [Fact]
        public async Task Holding_a_movement_key_moves_the_player_and_the_server_agrees()
        {
            var dir = _fx.ArtifactDir(nameof(Holding_a_movement_key_moves_the_player_and_the_server_agrees));
            string user = NewUser();
            using var client = await EnterWorldAsync(dir, "client", user);

            var before = await client.GetStateAsync();
            Assert.Equal(320f, before.Player.X);

            await client.KeyHoldAsync("D", 600);

            var moving = await client.WaitForStateAsync(s => s.Player != null && s.Player.X > before.Player.X, Wait, "player moved right");
            Assert.Equal(before.Player.Y, moving.Player.Y);

            // After release the client snaps to the server's authoritative stop position; it must still be
            // to the right of where we started, and stable.
            await Task.Delay(750);
            var settled1 = await client.GetStateAsync();
            await Task.Delay(250);
            var settled2 = await client.GetStateAsync();
            Assert.True(settled1.Player.X > before.Player.X, $"Expected X > {before.Player.X} after moving, got {settled1.Player.X}.");
            Assert.Equal(settled1.Player.X, settled2.Player.X);

            using var frame = await client.ScreenshotAsync(Path.Combine(dir, "after-move.png"));
            Assert.True(frame.LitFraction() > 0.05);
        }

        [Fact]
        public async Task A_chat_message_typed_by_one_player_is_seen_by_another()
        {
            var dir = _fx.ArtifactDir(nameof(A_chat_message_typed_by_one_player_is_seen_by_another));
            string alice = NewUser(), bob = NewUser();

            using var a = await EnterWorldAsync(dir, "alice", alice);
            using var b = await EnterWorldAsync(dir, "bob", bob);

            // Both clients see both players in the world.
            await a.WaitForAsync(async () => (await a.EntitiesAsync()).Any(e => e.Name == bob), Wait, "alice sees bob");
            await b.WaitForAsync(async () => (await b.EntitiesAsync()).Any(e => e.Name == alice), Wait, "bob sees alice");

            string message = "hello from " + alice;
            await a.SayAsync(message);

            var bobChat = await b.WaitForAsync(async () =>
            {
                var chat = await b.ChatAsync();
                return chat.Any(line => line.Contains(message)) ? chat : null;
            }, Wait, "bob's chat shows alice's message");
            Assert.Contains(bobChat, line => line.Contains(message));

            var aliceChat = await a.ChatAsync();
            Assert.Contains(aliceChat, line => line.Contains(message));

            using var frame = await b.ScreenshotAsync(Path.Combine(dir, "bob-sees-chat.png"));
            Assert.True(frame.LitFraction() > 0.05);
        }
    }
}
