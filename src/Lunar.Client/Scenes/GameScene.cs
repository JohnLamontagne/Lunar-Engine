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
using Microsoft.Xna.Framework.Input;
using Lunar.Client.GUI.Widgets;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Client.World;
using Lunar.Client.World.Actors;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;

namespace Lunar.Client.Scenes
{
    public class GameScene : Scene
    {
        private WorldManager _worldManager;
        private Camera _camera;
        private MouseState _oldMouseState;
        private IActor _target;
        private string _dialogueUniqueID;

        private bool _loadingScreen;

        public GameScene(ContentManager contentManager, GameWindow gameWindow, Camera camera)
            : base(contentManager, gameWindow)
        {
            _camera = camera;
            _worldManager = new WorldManager(contentManager, _camera);

            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.PLAYER_MSG, this.Handle_PlayerMessage);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.INVENTORY_UPDATE, this.Handle_InventoryUpdate);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.EQUIPMENT_UPDATE, this.Handle_EquipmentUpdate);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.TARGET_ACQ, this.Handle_TargetAcquired);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.QUIT_GAME, this.Handle_QuitGame);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.DIALOGUE, this.Handle_Dialogue);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.DIALOGUE_END, this.Handle_DialogueEnd);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.LOADING_SCREEN, this.Handle_LoadingScreen);

            Client.ServiceLocator.GetService<NetHandler>().Disconnected += Handle_Disconnected;

            _worldManager.EventOccured += _worldManager_EventOccured;

            Client.ServiceLocator.RegisterService(_worldManager);
        }

        private void Handle_LoadingScreen(PacketReceivedEventArgs obj)
        {
            _loadingScreen = true;
            Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("loadingScene");
        }


        private void Handle_DialogueEnd(PacketReceivedEventArgs obj)
        {
            _dialogueUniqueID = "";
            this.GuiManager.GetWidget<WidgetContainer>("dialogueWindow").Visible = false;
        }

        private void Handle_Dialogue(PacketReceivedEventArgs args)
        {
            _dialogueUniqueID = args.Message.ReadString();

            var dialogueWindow = this.GuiManager.GetWidget<WidgetContainer>("dialogueWindow");

            dialogueWindow.Visible = true;
            dialogueWindow.ClearWidgets();

            var font = this.ContentManager.Load<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/dialogueFont");

            var dialogueTextLabel = new Label(font);
            dialogueTextLabel.Position = new Vector2(dialogueWindow.Position.X + 20, dialogueWindow.Position.Y + 30);
            dialogueTextLabel.Text = '"' + args.Message.ReadString() + '"';
            dialogueTextLabel.WrapText(dialogueWindow.Size.X - 20);
            dialogueTextLabel.Visible = true;

            dialogueWindow.AddWidget(dialogueTextLabel, "dialogueText");

            int responseCount = args.Message.ReadInt32();
            var responseLabels = new Label[responseCount];
            for (int i = 0; i < responseCount; i++)
            {
                var responseLabel = new Label(font)
                {
                    Text = args.Message.ReadString(),
                    Visible = true
                };
                responseLabel.Clicked += ResponseLabel_Clicked;
                responseLabel.Mouse_Hover += ResponseLabel_Mouse_Hover;
                responseLabel.Mouse_Left += ResponseLabel_Mouse_Left;
                responseLabels[i] = responseLabel;
            }

            responseLabels[0].Position = new Vector2(dialogueWindow.Position.X + 20, dialogueWindow.Position.Y + dialogueWindow.Size.Y - 30);
            dialogueWindow.AddWidget(responseLabels[0], responseLabels[0].Text);

            if (responseCount > 1)
            {
                for (int i = 1; i < responseCount; i++)
                {
                    var prevWidth = font.MeasureString(responseLabels[i - 1].Text).X;
                    responseLabels[i].Position = new Vector2(responseLabels[i - 1].Position.X + prevWidth + Constants.DIALOGUE_SEP_X, dialogueWindow.Position.Y + dialogueWindow.Size.Y - 30);
                    dialogueWindow.AddWidget(responseLabels[i], responseLabels[i].Text);
                }
            }
        }

        private void ResponseLabel_Mouse_Left(object sender, EventArgs e)
        {
            // Make the text regular
            ((Label)sender).Font = this.ContentManager.Load<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/dialogueFont");
        }

        private void ResponseLabel_Mouse_Hover(object sender, EventArgs e)
        {
            // Make the text bold
            ((Label)sender).Font = this.ContentManager.Load<SpriteFont>(Constants.FILEPATH_GFX + "Fonts/dialogueFont_B");
        }

        private void ResponseLabel_Clicked(object sender, WidgetClickedEventArgs e)
        {
            var packet = new Packet(PacketType.DIALOGUE_RESP);
            packet.Message.Write(_dialogueUniqueID);
            packet.Message.Write(((Label)sender).Text);
            Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
        }

        private void Handle_TargetAcquired(PacketReceivedEventArgs args)
        {
            var enemyPortraitContainer = this.GuiManager.GetWidget<WidgetContainer>("targetPortraitContainer");

            var uniqueID = args.Message.ReadInt64();

            _target = _worldManager.Map.GetEntity(uniqueID);

            enemyPortraitContainer.GetWidget<Picture>("portraitGraphic").Visible = _target is NPC;
            

            enemyPortraitContainer.GetWidget<StatusBar>("targetHealthBar").Value = ((float)_target.Health / (float)_target.MaximumHealth) * 100f;
            enemyPortraitContainer.GetWidget<StatusBar>("targetHealthBar").Text = $"{_target.Health} / {_target.MaximumHealth}";

            enemyPortraitContainer.Visible = true;
        }

        private void Handle_QuitGame(PacketReceivedEventArgs args)
        {
            this.GuiManager.GetWidget<Chatbox>("chatbox").Clear();

            // Unload the world.
            _worldManager.Unload();

            Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("menuScene");
        }

        private void Handle_Disconnected(object sender, EventArgs e)
        {
            if (!this.Active)
                return;

            this.GuiManager.GetWidget<Chatbox>("chatbox").Clear();

            // Unload the world.
            _worldManager.Unload();

            Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("menuScene");
        }

        private void _worldManager_EventOccured(object sender, Core.Utilities.SubjectEventArgs e)
        {
            if (e.EventName == "playerUpdated")
            {
                Player player = (Player)e.Args[0];

                if ((Player)e.Args[0] == ((WorldManager)sender).Player)
                {
                    this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Value = (player.Health / player.MaximumHealth) * 100;
                    this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Text = $"HP {player.Health}/{player.MaximumHealth}";
                    this.GuiManager.GetWidget<StatusBar>("healthStatusBar").TextOffset =
                        new Vector2(this.GuiManager.GetWidget<StatusBar>("healthStatusBar").FillSprite.Width - this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Font.MeasureString(this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Text).X,
                            this.GuiManager.GetWidget<StatusBar>("healthStatusBar").FillSprite.Height / 2f);

                    this.GuiManager.GetWidget<StatusBar>("experienceBar").Value = (player.Experience / player.NextLevelExperience) * 100;

                    this.GuiManager.GetWidget<Label>("lblExperience").Text = $"{player.Experience}/{player.NextLevelExperience}";

                    var characterWindow = this.GuiManager.GetWidget<WidgetContainer>("characterWindow");

                    characterWindow.GetWidget<Label>("charWindowNameLabel").Text = player.Name;
                    characterWindow.GetWidget<Label>("charHealthLabel").Text = "Health: " + player.Health + "/" + player.MaximumHealth;
                    characterWindow.GetWidget<Label>("charStrengthLabel").Text = "Strength: " + player.Strength;
                    characterWindow.GetWidget<Label>("charIntLabel").Text = "Intelligence: " + player.Intelligence;
                    characterWindow.GetWidget<Label>("charDexLabel").Text = "Dexterity: " + player.Dexterity;
                    characterWindow.GetWidget<Label>("charDefLabel").Text = "Defence: " + player.Strength;

                    characterWindow.GetWidget<Label>("charLevelLabel").Text = "Level: " + player.Level;

                    if (!characterWindow.WidgetExists("characterPicture"))
                    {
                        AnimatedPicture characterPicture = new AnimatedPicture(player.SpriteSheet.Sprite.Texture, (int)((72 / player.Speed) / (player.SpriteSheet.Sprite.Texture.Width / 52f)), 
                            new Vector2(player.SpriteSheet.FrameSize.X, player.SpriteSheet.FrameSize.Y))
                        {
                            Position = new Vector2(characterWindow.Position.X + 210, characterWindow.Position.Y + 150),
                            Visible = true
                        };
                        characterWindow.AddWidget(characterPicture, "characterPicture");
                    }
                    else
                    {
                        characterWindow.GetWidget<AnimatedPicture>("characterPicture").Sprite = player.SpriteSheet.Sprite.Texture;
                        characterWindow.GetWidget<AnimatedPicture>("characterPicture").FrameTime =
                            (int) ((72 / player.Speed) / (player.SpriteSheet.Sprite.Texture.Width / 52f));
                    }
                }
            }
            else if (e.EventName == "loadingMap")
            {
                _loadingScreen = true;
                Client.ServiceLocator.GetService<SceneManager>().SetActiveScene("loadingScene");
            }
            else if (e.EventName == "finishedLoadingMap")
            {
                Client.ServiceLocator.GetService<SceneManager>().GetScene<LoadingScene>("loadingScene").OnFinishedLoading();
            }
        }

        private void Handle_EquipmentUpdate(PacketReceivedEventArgs args)
        {
            var equipmentContainer = this.GuiManager.GetWidget<WidgetContainer>("characterWindow").GetWidget<WidgetContainer>("equipmentContainer");

            equipmentContainer.ClearWidgets();
            
            for (int i = 0; i < Enum.GetNames(typeof(EquipmentSlots)).Length; i++)
            {
                bool hasItem = args.Message.ReadBoolean();

                if (!hasItem)
                    continue;

 
                var itemName = args.Message.ReadString();
                var textureName = args.Message.ReadString();
                EquipmentSlots slotType = (EquipmentSlots)args.Message.ReadInt32();

                Texture2D texture2D = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "/Items/" + textureName);

                var equipSlot = new Picture(texture2D);

                Vector2 position = Vector2.Zero;

                switch (slotType)
                {
                    case EquipmentSlots.Ring:
                        position = new Vector2(equipmentContainer.Position.X + 150, equipmentContainer.Position.Y + 310);
                        break;
                }

                equipSlot.Position = position;
                equipSlot.Visible = true;
                equipSlot.Tag = i.ToString();

                equipSlot.Clicked += EquipSlot_Clicked;

                equipmentContainer.AddWidget(equipSlot, $"equipSlot{i}");
            }
        }

       

        private void Handle_InventoryUpdate(PacketReceivedEventArgs args)
        {
            var inventoryWidget = this.GuiManager.GetWidget<WidgetContainer>("inventoryWidget");

            inventoryWidget.ClearWidgets();

            for (int i = 0; i < Constants.MAX_INVENTORY; i++)
            {
                bool slotOccupied = args.Message.ReadBoolean();

                if (slotOccupied)
                {
                    var itemName = args.Message.ReadString();
                    var textureName = args.Message.ReadString();
                    var slotType = args.Message.ReadInt32();
                    var amount = args.Message.ReadInt32();

                    Texture2D texture2D = this.ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "/Items/" + textureName);

                    var invSlot = new Picture(texture2D);
                    invSlot.Position = new Vector2(((i % 5) * Constants.INV_SLOT_OFFSET) + Constants.INVENTORY_OFFSET_X, ((i / 5) * Constants.INV_SLOT_OFFSET) + 
                        Constants.INVENTORY_OFFSET_Y) + inventoryWidget.Position;

                    invSlot.Visible = true;

                    invSlot.Clicked += InvSlot_Clicked;
                   

                    inventoryWidget.AddWidget(invSlot, "slot" + i);
                }
            }
        }

        private void EquipSlot_Clicked(object sender, WidgetClickedEventArgs e)
        {
            if (e.MouseButton == MouseButtons.Right)
            {
                // Get the slot number, which is stored in the tag property.
                int slotNum = int.Parse(((Picture)sender).Tag);

                // Unequip the item
                var packet = new Packet(PacketType.REQ_UNEQUIP_ITEM);
                packet.Message.Write(slotNum);
                Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
            }
        }

        private void InvSlot_Clicked(object sender, WidgetClickedEventArgs e)
        {
            var inventoryWidget = this.GuiManager.GetWidget<WidgetContainer>("inventoryWidget");

            // Calculate slot number
            Picture slotItemPic = (Picture)sender;
            Vector2 normalizedPos = slotItemPic.Position - inventoryWidget.Position;
            int col = ((int)normalizedPos.X - Constants.INVENTORY_OFFSET_X) / Constants.INV_SLOT_OFFSET;
            int row = ((int)normalizedPos.Y - Constants.INVENTORY_OFFSET_Y) / Constants.INV_SLOT_OFFSET;

            int slotNum = (row * 5) + col;

            if (e.MouseButton == MouseButtons.Right)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.LeftShift) || Keyboard.GetState().IsKeyDown(Keys.RightShift))
                {
                    // Drop the item
                    var packet = new Packet(PacketType.DROP_ITEM);
                    packet.Message.Write(slotNum);
                    Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
                }
                else
                {
                    // Equip the item
                    var packet = new Packet(PacketType.REQ_USE_ITEM);
                    packet.Message.Write(slotNum);
                    Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
                }
            }
        }


        private void Handle_PlayerMessage(PacketReceivedEventArgs args)
        {
            Color color;
            ChatMessageType messageType = (ChatMessageType)args.Message.ReadByte();
            switch (messageType)
            {
                case ChatMessageType.Regular:
                    color = Color.White;
                    break;

                case ChatMessageType.Announcement:
                    color = new Color(0, 255, 255);
                    break;

                case ChatMessageType.Alert:
                    color = Color.Red;
                    break;

                default:
                    color = Color.White;
                    break;
            }

            this.GuiManager.GetWidget<Chatbox>("chatbox").AddEntry("[" + DateTime.Now.ToString("h:mm tt") + "] " + args.Message.ReadString(), color);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState newMouseState = Mouse.GetState();

            if (newMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                this.HandleClick();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                this.GuiManager.GetWidget<Textbox>("messageEntry").Active = !this.GuiManager.GetWidget<Textbox>("messageEntry").Active;
                _worldManager.Player.InChat = this.GuiManager.GetWidget<Textbox>("messageEntry").Active;
            }

            _oldMouseState = newMouseState; // this reassigns the old state so that it is ready for next time

            _worldManager.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleClick()
        {
            Point mousePos = Mouse.GetState().Position;

            Vector2 worldPos = _camera.ScreenToWorldCoords(new Vector2(mousePos.X, mousePos.Y));

            if (_worldManager.Map != null)
            {
                bool foundTarget = false;

                foreach (var entity in _worldManager.Map.GetEntities())
                {
                    int left = (int)(entity.Position.X);
                    int top = (int)(entity.Position.Y);

                    var entitySpace = new Rectangle(left, top, (int)entity.SpriteSheet.FrameSize.X, (int)entity.SpriteSheet.FrameSize.Y);

                    if (entitySpace.Contains(worldPos))
                    {
                        var selectPacket = new Packet(PacketType.REQ_TARGET);
                        selectPacket.Message.Write(entity.UniqueID);
                        Client.ServiceLocator.GetService<NetHandler>().SendMessage(selectPacket.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                        foundTarget = true;
                    }
                }

                if (!foundTarget)
                {
                    // If we reached this point there is no valid target. We should deselect the NPC and hide the target portrait.
                    this.GuiManager.GetWidget<WidgetContainer>("targetPortraitContainer").Visible = false;

                    var deselectPacket = new Packet(PacketType.DESELECT_TARGET);
                    Client.ServiceLocator.GetService<NetHandler>().SendMessage(deselectPacket.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
                }
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _worldManager.Draw(spriteBatch);

            base.Draw(gameTime, spriteBatch);
        }

        protected override void OnEnter()
        {
            _loadingScreen = false;

            base.OnEnter();
        }

        protected override void OnExit()
        {
            if (!_loadingScreen)
            {
                this.GuiManager.ClearWidgets();
            }

            base.OnExit();
        }

        public void InitalizeInterface()
        {
            this.GuiManager.LoadFromFile(Constants.FILEPATH_DATA + "Interface/game/game_interface.xml", this.ContentManager);

            var chat = this.GuiManager.GetWidget<Chatbox>("chatbox");
            var messageEntry = this.GuiManager.GetWidget<Textbox>("messageEntry");
            var logoutButton = this.GuiManager.GetWidget<Button>("btnLogout");
            var toggleInventoryButton = this.GuiManager.GetWidget<Button>("btnInventory");
            var toggleCharacterWindowButton = this.GuiManager.GetWidget<Button>("btnCharacter");
            var healthStatusBar = this.GuiManager.GetWidget<StatusBar>("healthStatusBar");
            var manaStatusBar = this.GuiManager.GetWidget<StatusBar>("manaStatusBar");
            var targetHealthBar = this.GuiManager.GetWidget<WidgetContainer>("targetPortraitContainer").GetWidget<StatusBar>("targetHealthBar");

            messageEntry.BindTo(chat);
            messageEntry.Visible = true;
            messageEntry.ReturnPressed += messageEntry_ReturnPressed;
            
            logoutButton.Clicked += logoutButton_ButtonClicked;

            toggleInventoryButton.Clicked += toggleInventoryButton_ButtonClicked;

            toggleCharacterWindowButton.Clicked += toggleCharacterWindowButton_ButtonClicked;


            healthStatusBar.TextOffset =
                new Vector2(healthStatusBar.FillSprite.Width - healthStatusBar.Font.MeasureString(healthStatusBar.Text).X,
                    healthStatusBar.FillSprite.Height / 2f);


            manaStatusBar.TextOffset =
               new Vector2(manaStatusBar.FillSprite.Width - manaStatusBar.Font.MeasureString(manaStatusBar.Text).X,
                   manaStatusBar.FillSprite.Height / 2f);


            targetHealthBar.TextOffset =
                new Vector2(targetHealthBar.FillSprite.Width - targetHealthBar.Font.MeasureString(targetHealthBar.Text).X,
                    targetHealthBar.FillSprite.Height / 2f);

        }

        private void toggleCharacterWindowButton_ButtonClicked(object sender, EventArgs e)
        {
            this.GuiManager.GetWidget<WidgetContainer>("characterWindow").Visible = !this.GuiManager.GetWidget<WidgetContainer>("characterWindow").Visible;
        }

        private void toggleInventoryButton_ButtonClicked(object sender, EventArgs e)
        {
            this.GuiManager.GetWidget<WidgetContainer>("inventoryWidget").Visible = !this.GuiManager.GetWidget<WidgetContainer>("inventoryWidget").Visible;
        }

        private void logoutButton_ButtonClicked(object sender, EventArgs e)
        {
            var packet = new Packet(PacketType.QUIT_GAME);
            Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
        }

        private void messageEntry_ReturnPressed(object sender, EventArgs e)
        {
            string text = (sender as Textbox)?.Text;

            if (!string.IsNullOrEmpty(text))
            {
                var packet = new Packet(PacketType.PLAYER_MSG);
                packet.Message.Write(text);
                Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.Unreliable, ChannelType.UNASSIGNED);
                ((Textbox)sender).Text = string.Empty;
                ((Textbox) sender).Active = false;
            }
        }
    }
}