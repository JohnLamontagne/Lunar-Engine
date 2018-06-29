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
            this.InitalizeInterface();

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
            this.GuiManager.GetWidget<WidgetContainer>("mainMenuContainer").GetWidget<Textbox>("passwordLoginTextbox").Sprite = textboxPassSprite;
        }

        private void InitalizeInterface()
        {
            var font = this.ContentManager.Load<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/interfaceFont");

            var menuButtonSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/menuButton.png");
            var windowBackSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/menuWindow.png");
            var checkTrueSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/checkTrue.png");
            var checkFalseSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/checkFalse.png");

            var mainMenuContainer = new WidgetContainer(windowBackSprite);
            mainMenuContainer.Position = new Vector2((Settings.ResolutionX / 2f) - mainMenuContainer.Size.X / 2, ((Settings.ResolutionY / 2f) - mainMenuContainer.Size.Y / 2));
            mainMenuContainer.Visible = true;
            this.GuiManager.AddWidget(mainMenuContainer, "mainMenuContainer");

            var lblMenuTitle = new Label(font)
            {
                Text = "Main Menu",
                Visible = true,
                Position = new Vector2(mainMenuContainer.Position.X + 60, mainMenuContainer.Position.Y + 15)
            };
            mainMenuContainer.AddWidget(lblMenuTitle, "lblMenuTitle");

            var statusLabel = new Label(font)
            {
                Position = new Vector2(mainMenuContainer.Position.X + 40, mainMenuContainer.Position.Y + 185),
                Color = Color.Red,
                Visible= true,
                ZOrder = 1
            };
            mainMenuContainer.AddWidget(statusLabel, "lblStatus");

            var loginButtonSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/loginButton.png");
            var loginButton = new Button(loginButtonSprite, "Login", font)
            {
                Position = new Vector2(mainMenuContainer.Position.X + 30, mainMenuContainer.Position.Y + 205),
                ZOrder = 1
               
            };
            loginButton.Clicked += loginButton_ButtonClicked;
            loginButton.Visible = true;
            mainMenuContainer.AddWidget(loginButton, "btnLogin");

            var registerButton = new Button(menuButtonSprite, "Register", font)
            {
                Position = new Vector2(mainMenuContainer.Position.X + 275, mainMenuContainer.Position.Y + 60),
                Visible = true,
                ZOrder = 1,
            };
            registerButton.Clicked += registerButton_ButtonClicked;
            mainMenuContainer.AddWidget(registerButton, "btnRegister");

            var websiteButton = new Button(menuButtonSprite, "Website", font)
            {
                Position = new Vector2(mainMenuContainer.Position.X + 275, mainMenuContainer.Position.Y + 120),
                ZOrder = 1
            };
            websiteButton.Clicked += WebsiteButton_Clicked;
            websiteButton.Visible = true;
            mainMenuContainer.AddWidget(websiteButton, "btnSubmitRegistration");


            var textboxUserSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/textUser.png");
            var userLoginTextbox = new Textbox(textboxUserSprite, font, new Vector2(12, 8))
            {
                Position = new Vector2(mainMenuContainer.Position.X + 30, mainMenuContainer.Position.Y + 50),
                Visible = true,
                ZOrder=1,
            };
            userLoginTextbox.Text_Entered += UserLoginTextbox_Text_Entered;
            mainMenuContainer.AddWidget(userLoginTextbox, "userLoginTextbox");
            
            var textboxPassSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/textPass.png");
            var passwordLoginTextbox = new Textbox(textboxPassSprite, font, new Vector2(12, 8))
            {
                Position = new Vector2(mainMenuContainer.Position.X + 30, mainMenuContainer.Position.Y + 115),
                Mask = "*",
                Visible = true,
                ZOrder = 1,
            };
            passwordLoginTextbox.Text_Entered += PasswordLoginTextbox_Text_Entered;
            mainMenuContainer.AddWidget(passwordLoginTextbox, "passwordLoginTextbox");

            var muteMusicCheckbox = new Checkbox(checkTrueSprite, checkFalseSprite)
            {
                Position = new Vector2(Settings.ResolutionX - (checkTrueSprite.Width * 2), Settings.ResolutionY - (checkTrueSprite.Height * 2)),
                Visible = true, 
                ZOrder = 1,
            };
            this.GuiManager.AddWidget(muteMusicCheckbox, "muteMusicCheckbox");
            muteMusicCheckbox.Clicked += MuteMusicCheckbox_Clicked;

            var labelMuteMusic = new Label(font)
            {
                Text = "Mute Music: ",
                Position = new Vector2(muteMusicCheckbox.Position.X - 90, muteMusicCheckbox.Position.Y + (checkTrueSprite.Height / 4f)),
                Visible = true,
                ZOrder = 1
            };
            this.GuiManager.AddWidget(labelMuteMusic, "labelMuteMusic");

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
            System.Diagnostics.Process.Start("https://www.mmorpgcreation.com");
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
                !string.IsNullOrEmpty(registerMenuContainer.GetWidget<Textbox>("passwordLoginTextbox").Text))
            {
                if (!netHandler.Connected)
                {
                    netHandler.Connect();

                    var packet = new Packet(PacketType.REGISTER);
                    packet.Message.Write(registerMenuContainer.GetWidget<Textbox>("userLoginTextbox").Text);
                    packet.Message.Write(registerMenuContainer.GetWidget<Textbox>("passwordLoginTextbox").Text);
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

            if (string.IsNullOrEmpty(loginMenuContainer.GetWidget<Textbox>("passwordLoginTextbox").Text))
            {
                var textboxPassSprite = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "Interface/passInputError.png");
                loginMenuContainer.GetWidget<Textbox>("passwordLoginTextbox").Sprite = textboxPassSprite;

                failure = true;
            }

            if (!failure && !netHandler.Connected)
            {
                netHandler.Connect();


                var packet = new Packet(PacketType.LOGIN);
                packet.Message.Write(loginMenuContainer.GetWidget<Textbox>("userLoginTextbox").Text);
                packet.Message.Write(loginMenuContainer.GetWidget<Textbox>("passwordLoginTextbox").Text);
                netHandler.SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
            }
            
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch drawer)
        {
            base.Draw(gameTime, drawer);
        }
    }
}