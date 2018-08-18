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
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Core.Net;
using GameTime = Microsoft.Xna.Framework.GameTime;
using Label = Lunar.Client.GUI.Widgets.Label;

namespace Lunar.Client.Scenes
{
    internal class MenuScene : Scene
    {
        private GameWindow _gameWindow;

        public MenuScene(ContentManager contentManager, GameWindow gameWindow)
            : base(contentManager, gameWindow)
        {
            _gameWindow = gameWindow;

            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.REGISTER_SUCCESS, this.Handle_AuthenticationSuccess);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.LOGIN_SUCCESS, this.Handle_AuthenticationSuccess);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.LOGIN_FAIL, this.Handle_AuthenticationFailure);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.REGISTRATION_FAIL, this.Handle_AuthenticationFailure);
        }

        protected override void OnEnter()
        {
            this.GuiManager.LoadFromFile(Constants.FILEPATH_DATA + "Interface/menu/menu_interface.xml", this.ContentManager);
            this.HookInterfaceEvents();

            base.OnEnter();
        }

        protected override void OnExit()
        {
            MediaPlayer.Stop();
            MediaPlayer.IsRepeating = false;

            this.GuiManager.ClearWidgets();

            base.OnExit();
        }


        private void Handle_AuthenticationSuccess(PacketReceivedEventArgs args)
        {
            Client.ServiceLocator.GetService<SceneManager>().GetScene<GameScene>("gameScene").InitalizeInterface();
            Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("loadingScene");
        }

        private void Handle_AuthenticationFailure(PacketReceivedEventArgs args)
        {
            var failMessage = args.Message.ReadString();

            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Label>("lblStatus").Text =
                failMessage;
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Label>("lblStatus")
                .Visible = true;

            var textboxUserSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/userInputError.png");
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Textbox>("userLoginTextbox").Sprite = textboxUserSprite;


            var textboxPassSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/passInputError.png");
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Textbox>("userPasswordTextbox").Sprite = textboxPassSprite;
        }

        private void HookInterfaceEvents()
        {
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Textbox>("userLoginTextbox").Text_Entered += UserLoginTextbox_Text_Entered;
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Textbox>("userPasswordTextbox").Text_Entered += PasswordLoginTextbox_Text_Entered;

            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Button>("btnLogin").Clicked += loginButton_ButtonClicked;
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Button>("btnRegister").Clicked += registerButton_ButtonClicked;
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Button>("btnWebsite").Clicked += WebsiteButton_Clicked;

        }

        private void PasswordLoginTextbox_Text_Entered(object sender, EventArgs e)
        {
            var textboxUserSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/passInputSuccess.png");
            ((Textbox)sender).Sprite = textboxUserSprite;
        }

        private void UserLoginTextbox_Text_Entered(object sender, EventArgs e)
        {
            var textboxUserSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/userInputSuccess.png");
            ((Textbox) sender).Sprite = textboxUserSprite;
        }

        private void WebsiteButton_Clicked(object sender, WidgetClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(Settings.Website);
        }

        private void MuteMusicCheckbox_Clicked(object sender, WidgetClickedEventArgs e)
        {
            MediaPlayer.IsMuted = ((Checkbox)sender).Value;
        }

        private void registerButton_ButtonClicked(object sender, EventArgs e)
        {
            NetHandler netHandler = Client.ServiceLocator.GetService<NetHandler>();

            var registerMenuContainer = this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer");

            if (!string.IsNullOrEmpty(registerMenuContainer.GetWidget<Textbox>("userLoginTextbox").Text) &&
                !string.IsNullOrEmpty(registerMenuContainer.GetWidget<Textbox>("userPasswordTextbox").Text))
            {
                if (!netHandler.Connected)
                {
                    netHandler.Connect();

                    var packet = new Packet(PacketType.REGISTER);
                    packet.Message.Write(registerMenuContainer.GetWidget<Textbox>("userLoginTextbox").Text);
                    packet.Message.Write(registerMenuContainer.GetWidget<Textbox>("userPasswordTextbox").Text);
                    netHandler.SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
                }

            }
        }

        private void loginButton_ButtonClicked(object sender, EventArgs e)
        {
            NetHandler netHandler = Client.ServiceLocator.GetService<NetHandler>();

            var loginMenuContainer = this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer");

            bool failure = false;

            if (string.IsNullOrEmpty(loginMenuContainer.GetWidget<Textbox>("userLoginTextbox").Text))
            {
                var textboxUserSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/userInputError.png");
                loginMenuContainer.GetWidget<Textbox>("userLoginTextbox").Sprite = textboxUserSprite;

                failure = true;
            }

            if (string.IsNullOrEmpty(loginMenuContainer.GetWidget<Textbox>("userPasswordTextbox").Text))
            {
                var textboxPassSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/passInputError.png");
                loginMenuContainer.GetWidget<Textbox>("userPasswordTextbox").Sprite = textboxPassSprite;

                failure = true;
            }

            if (!failure && !netHandler.Connected)
            {
                netHandler.Connect();


                var packet = new Packet(PacketType.LOGIN);
                packet.Message.Write(loginMenuContainer.GetWidget<Textbox>("userLoginTextbox").Text);
                packet.Message.Write(loginMenuContainer.GetWidget<Textbox>("userPasswordTextbox").Text);
                netHandler.SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
            }
            
        }

        public override void Draw(GameTime gameTime, SpriteBatch drawer)
        {
            base.Draw(gameTime, drawer);
        }
    }
}