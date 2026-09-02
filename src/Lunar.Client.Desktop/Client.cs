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
#define DEV_MODE

using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Client.Utilities.Services;
using Lunar.Core;
using Lunar.Graphics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using System;

namespace Lunar.Client
{
    public class Client : ClientBase
    {
        private Texture2D _cursorSprite;
        private Vector2 _cursorPos;
        private KeyboardState _previousKeyboardState;
        private ConsoleRedirector _consoleRedirector;
        private DeveloperConsoleComponent _consoleComponent;
        private CommandInterpreter _commandInterpreter;

        public static bool ShuttingDown { get; set; }

        public Client()
        {
#if DEV_MODE
            string rootPath = Engine.FindDevRootPath("Client Data");
#else
            string rootPath = AppDomain.CurrentDomain.BaseDirectory;
#endif
            Engine.Initialize(rootPath);
            Settings.Initalize();

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Settings.ResolutionY,
                PreferredBackBufferWidth = Settings.ResolutionX
            };
            _graphics.PreparingDeviceSettings += OnPreparingDeviceSettings;

            Content.RootDirectory = "Client Data";
        }

        private void OnPreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<LightManagerService>(_ => new LightManagerService(new PenumbraComponent(this)));
        }

        protected override void InitializePlatformServices(IServiceProvider services)
        {
            var lightManager = services.GetRequiredService<LightManagerService>();
            lightManager.Component.Initialize();

            var interpreter = new CommandInterpreter(services.GetRequiredService<NetHandler>());
            _commandInterpreter = interpreter;
            _consoleComponent = new DeveloperConsoleComponent(this, interpreter);
            _consoleComponent.FontColor = Color.Wheat;
            this.Components.Add(_consoleComponent);

            _consoleRedirector = new ConsoleRedirector(_consoleComponent);
            Console.SetOut(_consoleRedirector);
        }

        protected override void ConfigureAutomation(Lunar.Client.Automation.AutomationServer automation)
        {
            automation.CommandHandler = input =>
            {
                var lines = new System.Text.StringBuilder();
                bool ok = _commandInterpreter.Execute(input, line => lines.AppendLine(line));
                return (ok, lines.ToString());
            };
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _cursorSprite = this.Content.LoadTexture2D(Constants.FILEPATH_GFX + "cursor.png");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Lunar.Client.Utilities.Input.Input.Keyboard.IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState currentKeyboardState = Lunar.Client.Utilities.Input.Input.Keyboard;
            if (_previousKeyboardState.IsKeyUp(Keys.OemTilde) && currentKeyboardState.IsKeyDown(Keys.OemTilde))
                _consoleComponent.ToggleOpenClose();
            _previousKeyboardState = currentKeyboardState;

            Services.GetRequiredService<LightManagerService>().Component.Transform = _camera.GetTransformation();
            _cursorPos = new Vector2(Lunar.Client.Utilities.Input.Input.Mouse.X, Lunar.Client.Utilities.Input.Input.Mouse.Y);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            Services.GetRequiredService<LightManagerService>().Component.BeginDraw();
            base.Draw(gameTime);
        }

        protected override void DrawOverlay(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_cursorSprite, _cursorPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);
        }
    }
}
