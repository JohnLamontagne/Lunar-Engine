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

using Lunar.Client.GUI;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Client.Utilities.Services;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Client.Scenes
{
    internal class LoadingScene : Scene
    {
        private bool _finishedLoading;
        private double _minEndTime;

        private readonly SceneManager _sceneManager;

        public LoadingScene(ContentManagerService contentManagerService, GameWindow gameWindow, NetHandler netHandler, LightManagerService lightManager, GUIManager guiManager, SceneManager sceneManager) :
            base(contentManagerService, gameWindow, netHandler, lightManager, guiManager)
        {
            _sceneManager = sceneManager;
            this.InitalizeInterface();
        }

        public override void Update(GameTime gameTime)
        {
            if (_minEndTime <= 0)
            {
                _minEndTime = gameTime.TotalGameTime.TotalMilliseconds + Constants.MIN_LOAD_TIME;
            }

            if (_finishedLoading && gameTime.TotalGameTime.TotalMilliseconds > _minEndTime)
            {
                _sceneManager.SetActiveScene("gameScene");
                var mapLoaded = new Packet();
                this.NetHandler.SendPacket(PacketType.MAP_LOADED, mapLoaded, DeliveryMethod.ReliableOrdered);
            }

            base.Update(gameTime);
        }

        protected override void OnEnter()
        {
            _finishedLoading = false;
            _minEndTime = 0;

            base.OnEnter();
        }

        private void InitalizeInterface()
        {
            var loadingIndicatorTexture2D = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/loadingIndicator.png");
            var loadingFont = this.ContentManager.LoadAsset<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/loadingFont");

            var picLoadingIndicator = new AnimatedPicture(loadingIndicatorTexture2D, 25,
                new Vector2(loadingIndicatorTexture2D.Width, loadingIndicatorTexture2D.Height))
            {
                Position = new Vector2(Settings.ResolutionX / 2f, (Settings.ResolutionY / 2f) - 100),
                Visible = true,
                FrameRotation = 1f,
                ZOrder = 1
            };
            this.GuiManager.AddWidget(picLoadingIndicator, "picLoadingIndicator");

            var lblLoading = new Label(loadingFont)
            {
                Text = $"Loading {Settings.GameName} ...",
                Visible = true,
                ZOrder = 1
            };
            lblLoading.Position = new Vector2(Settings.ResolutionX / 2f - (loadingFont.MeasureString(lblLoading.Text).X / 2f), picLoadingIndicator.Position.Y + 200 + (loadingFont.MeasureString(lblLoading.Text).Y));
            this.GuiManager.AddWidget(lblLoading, "lblLoading");
        }

        public void OnFinishedLoading()
        {
            _finishedLoading = true;
        }
    }
}