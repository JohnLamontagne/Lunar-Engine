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
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Core.Net;

namespace Lunar.Client.Scenes
{
    class LoadingScene : Scene
    {
        private bool _finishedLoading;
        private double _minEndTime;

        public LoadingScene(ContentManager contentManager, GameWindow gameWindow) : 
            base(contentManager, gameWindow)
        {
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
                Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("gameScene");
                var mapLoaded = new Packet(PacketType.MAP_LOADED);
                Client.ServiceLocator.GetService<NetHandler>().SendMessage(mapLoaded.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
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
            var loadingFont = this.ContentManager.Load<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/loadingFont");

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
