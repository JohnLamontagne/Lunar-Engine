using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Net;
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
            var loadingIndicatorTexture2D = this.ContentManager.Load<Texture2D>(Constants.FILEPATH_GFX + "Interface/loadingIndicator");
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
                Text = $"Loading {Constants.GAME_NAME} ...",
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
