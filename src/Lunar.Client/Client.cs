/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using QuakeConsole;
using Lunar.Client.Net;
using Lunar.Client.Scenes;
using Lunar.Client.Utilities;
using Lunar.Client.Utilities.Input;
using Lunar.Client.Utilities.Services;
using Lunar.Core.Utilities;
using Lunar.Graphics;

namespace Lunar.Client
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Client : Game, ISubject
    {
        private static ServiceLocator _serviceLocator;

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _cursorSprite;
        private Vector2 _cursorPos;
        private Camera _camera;
        private KeyboardState _previousKeyboardState;

        private ConsoleRedirector _consoleRedirector;
        private ConsoleComponent _consoleComponent;

        public static ServiceLocator ServiceLocator { get { return _serviceLocator = _serviceLocator ?? new ServiceLocator(); } }

        public static bool ShuttingDown { get; set; }

        public Client()
        {
            Settings.Initalize();

            _graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferHeight = Settings.ResolutionY,
                PreferredBackBufferWidth = Settings.ResolutionX
            };

            _graphics.PreparingDeviceSettings += _graphics_PreparingDeviceSettings;

            Content.RootDirectory = "Content";

        }

        private void _graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.RenderTargetUsage = RenderTargetUsage.PreserveContents;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Client.ServiceLocator.RegisterService(new GraphicsDeviceService(this.GraphicsDevice));
            Client.ServiceLocator.RegisterService(new ContentManagerService(this.Content));
            Client.ServiceLocator.RegisterService(new LightManagerService(new PenumbraComponent(this)));
            Client.ServiceLocator.RegisterService(new NetHandler());
            Client.ServiceLocator.RegisterService(new SceneManager());

            
            Client.ServiceLocator.GetService<LightManagerService>().Component.Initialize();

            _camera = new Camera(new Rectangle(0, 0, Settings.ResolutionX, Settings.ResolutionY));

            EventInput.Initialize(this.Window);
            this.InitalizeScenes();

            _consoleComponent = new ConsoleComponent(this);
            this.Components.Add(_consoleComponent);
            this.InitalizeCommands();

            _consoleRedirector = new ConsoleRedirector(_consoleComponent);
            Console.SetOut(_consoleRedirector);


            Window.Title = Settings.GameName;

            base.Initialize();
        }

        private void InitalizeCommands()
        {
            var interpreter = new CommandInterpreter();

            _consoleComponent.Interpreter = interpreter;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Initialize the SpriteBatchExtensions
            SpriteBatchExtensions.Initalize(this.GraphicsDevice);

            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(this.GraphicsDevice);


            _cursorSprite = this.Content.LoadTexture2D(Constants.FILEPATH_GFX + "cursor.png");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState currentKeyboardState = Keyboard.GetState();

            if (_previousKeyboardState.IsKeyUp(Keys.OemTilde) && currentKeyboardState.IsKeyDown(Keys.OemTilde))
                _consoleComponent.ToggleOpenClose();

            _previousKeyboardState = currentKeyboardState;

            Client.ServiceLocator.GetService<LightManagerService>().Component.Transform = _camera.GetTransformation();

            Client.ServiceLocator.GetService<NetHandler>().Update();

            _cursorPos = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);

            Client.ServiceLocator.GetService<SceneManager>().Update(gameTime);

            base.Update(gameTime);

            this.EventOccured?.Invoke(this, new SubjectEventArgs("updateFinished", new object[] { gameTime }));
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Client.ServiceLocator.GetService<LightManagerService>().Component.BeginDraw();

            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, SamplerState.PointClamp, null, null, null, _camera.GetTransformation());

            Client.ServiceLocator.GetService<SceneManager>().Draw(gameTime, _spriteBatch);

            // The cursor should always be the foremost visible
            _spriteBatch.Draw(_cursorSprite, _cursorPos, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1);

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void InitalizeScenes()
        {

            Client.ServiceLocator.GetService<SceneManager>().AddScene(new MenuScene(this.Content, this.Window), "menuScene");
            Client.ServiceLocator.GetService<SceneManager>().AddScene(new GameScene(this.Content, this.Window, _camera), "gameScene");
            Client.ServiceLocator.GetService<SceneManager>().AddScene(new LoadingScene(this.Content, this.Window), "loadingScene");
            Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("menuScene");
        }

        public event EventHandler<SubjectEventArgs> EventOccured;
    }
}