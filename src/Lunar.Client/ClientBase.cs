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

using Lunar.Client.Net;
using Lunar.Client.Scenes;
using Lunar.Client.Utilities;
using Lunar.Client.Utilities.Input;
using Lunar.Client.Utilities.Services;
using Lunar.Client.World;
using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Graphics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Lunar.Client
{
    public abstract class ClientBase : Game, ISubject
    {
        protected GraphicsDeviceManager _graphics;
        protected SpriteBatch _spriteBatch;
        protected Camera _camera;

        private NetHandler _netHandler;
        private SceneManager _sceneManager;

        protected IServiceProvider Services { get; private set; }

        protected override void Initialize()
        {
            Window.Title = Settings.GameName;

            var services = new ServiceCollection();

            // MonoGame-derived primitives — only available now that GraphicsDevice exists.
            services.AddSingleton(this.GraphicsDevice);
            services.AddSingleton(this.Content);
            services.AddSingleton(this.Window);
            services.AddSingleton<Logger>();
            services.AddSingleton(new GraphicsDeviceService(this.GraphicsDevice));
            services.AddSingleton(new ContentManagerService(this.Content));
            services.AddSingleton(new Camera(new Rectangle(0, 0, Settings.ResolutionX, Settings.ResolutionY)));

            services.AddSingleton<NetHandler>();
            services.AddSingleton<SceneManager>();
            services.AddSingleton<WorldManager>();
            // GUIManager is transient: each scene owns its own.
            services.AddTransient<GUI.GUIManager>();
            services.AddSingleton<MenuScene>();
            services.AddSingleton<GameScene>();
            services.AddSingleton<LoadingScene>();

            this.ConfigureServices(services);

            Services = services.BuildServiceProvider();

            _camera = Services.GetRequiredService<Camera>();
            _netHandler = Services.GetRequiredService<NetHandler>();
            _sceneManager = Services.GetRequiredService<SceneManager>();

            EventInput.Initialize(this.Window);
            this.InitializePlatformServices(Services);
            this.InitializeScenes();

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            base.Initialize();
        }

        protected virtual void ConfigureServices(IServiceCollection services) { }

        protected virtual void InitializePlatformServices(IServiceProvider services) { }

        protected virtual void InitializeScenes()
        {
            var menuScene = Services.GetRequiredService<MenuScene>();
            var gameScene = Services.GetRequiredService<GameScene>();
            var loadingScene = Services.GetRequiredService<LoadingScene>();

            menuScene.Initalize();
            gameScene.Initalize();
            loadingScene.Initalize();

            _sceneManager.AddScene(menuScene, "menuScene");
            _sceneManager.AddScene(gameScene, "gameScene");
            _sceneManager.AddScene(loadingScene, "loadingScene");
            _sceneManager.SetActiveScene("menuScene");
        }

        protected override void LoadContent()
        {
            SpriteBatchExtensions.Initalize(this.GraphicsDevice);
            _spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            _netHandler.ProcessPacketQueue();
            _sceneManager.Update(gameTime);
            base.Update(gameTime);
            EventOccured?.Invoke(this, new SubjectEventArgs("updateFinished", new object[] { gameTime }));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, _camera.GetTransformation());
            _sceneManager.Draw(gameTime, _spriteBatch);
            this.DrawOverlay(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected virtual void DrawOverlay(SpriteBatch spriteBatch) { }

        public event EventHandler<SubjectEventArgs> EventOccured;
    }
}
