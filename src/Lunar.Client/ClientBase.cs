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
using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Graphics;
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

        protected override void Initialize()
        {
            Window.Title = Settings.GameName;

            Engine.Services.Register(new GraphicsDeviceService(this.GraphicsDevice));
            Engine.Services.Register(new ContentManagerService(this.Content));
            Engine.Services.Register(new NetHandler());
            Engine.Services.Register(new SceneManager());

            _camera = new Camera(new Rectangle(0, 0, Settings.ResolutionX, Settings.ResolutionY));

            EventInput.Initialize(this.Window);
            this.InitializePlatformServices();
            this.InitializeScenes();

            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            base.Initialize();
        }

        protected virtual void InitializePlatformServices() { }

        protected virtual void InitializeScenes()
        {
            var menuScene = new MenuScene(this.Content, this.Window);
            var gameScene = new GameScene(this.Content, this.Window, _camera);
            var loadingScene = new LoadingScene(this.Content, this.Window);

            menuScene.Initalize();
            gameScene.Initalize();
            loadingScene.Initalize();

            Engine.Services.Get<SceneManager>().AddScene(menuScene, "menuScene");
            Engine.Services.Get<SceneManager>().AddScene(gameScene, "gameScene");
            Engine.Services.Get<SceneManager>().AddScene(loadingScene, "loadingScene");
            Engine.Services.Get<SceneManager>().SetActiveScene("menuScene");
        }

        protected override void LoadContent()
        {
            SpriteBatchExtensions.Initalize(this.GraphicsDevice);
            _spriteBatch = new SpriteBatch(this.GraphicsDevice);
        }

        protected override void Update(GameTime gameTime)
        {
            Engine.Services.Get<NetHandler>().ProcessPacketQueue();
            Engine.Services.Get<SceneManager>().Update(gameTime);
            base.Update(gameTime);
            EventOccured?.Invoke(this, new SubjectEventArgs("updateFinished", new object[] { gameTime }));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            _spriteBatch.Begin(SpriteSortMode.FrontToBack, null, null, null, null, null, _camera.GetTransformation());
            Engine.Services.Get<SceneManager>().Draw(gameTime, _spriteBatch);
            this.DrawOverlay(_spriteBatch);
            _spriteBatch.End();
            base.Draw(gameTime);
        }

        protected virtual void DrawOverlay(SpriteBatch spriteBatch) { }

        public event EventHandler<SubjectEventArgs> EventOccured;
    }
}
