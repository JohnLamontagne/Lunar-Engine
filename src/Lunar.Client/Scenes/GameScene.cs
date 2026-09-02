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
using Lunar.Client.World;
using Lunar.Client.World.Actors;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Lunar.Client.Scenes
{
    public class GameScene : Scene
    {
        private WorldManager _worldManager;
        private Camera _camera;
        private MouseState _oldMouseState;
        private IActor _target;

        private bool _loadingScreen;

        private string _dialogueBranchName;

        private readonly SceneManager _sceneManager;

        public GameScene(ContentManagerService contentManagerService, GameWindow gameWindow, Camera camera, NetHandler netHandler, LightManagerService lightManager, GUIManager guiManager, SceneManager sceneManager, WorldManager worldManager)
            : base(contentManagerService, gameWindow, netHandler, lightManager, guiManager)
        {
            _camera = camera;
            _worldManager = worldManager;
            _sceneManager = sceneManager;

            netHandler.AddPacketHandler(PacketType.PLAYER_MSG, this.Handle_PlayerMessage);
            netHandler.AddPacketHandler(PacketType.INVENTORY_UPDATE, this.Handle_InventoryUpdate);
            netHandler.AddPacketHandler(PacketType.EQUIPMENT_UPDATE, this.Handle_EquipmentUpdate);
            netHandler.AddPacketHandler(PacketType.TARGET_ACQ, this.Handle_TargetAcquired);
            netHandler.AddPacketHandler(PacketType.QUIT_GAME, this.Handle_QuitGame);
            netHandler.AddPacketHandler(PacketType.DIALOGUE, this.Handle_Dialogue);
            netHandler.AddPacketHandler(PacketType.DIALOGUE_END, this.Handle_DialogueEnd);
            netHandler.AddPacketHandler(PacketType.LOADING_SCREEN, this.Handle_LoadingScreen);

            netHandler.Disconnected += Handle_Disconnected;

            _worldManager.LoadingMap += _worldManager_LoadingMap;
            _worldManager.LoadedMap += _worldManager_LoadedMap;
            _worldManager.PlayerJoined += _worldManager_PlayerJoined;
        }

        private void _worldManager_PlayerJoined(object sender, PlayerJoinedEventArgs e)
        {
            if (_worldManager.Player != null)
            {
                _worldManager.Player.DataChanged += Player_StatsChanged;
            }
        }

        private void _worldManager_LoadedMap(object sender, EventArgs e)
        {
            _sceneManager.GetScene<LoadingScene>("loadingScene").OnFinishedLoading();
        }

        private void _worldManager_LoadingMap(object sender, EventArgs e)
        {
            _loadingScreen = true;
            _sceneManager.SetActiveScene("loadingScene");
        }

        private void Player_StatsChanged(object sender, EventArgs e)
        {
            var player = (Player)sender;

            this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Value = ((float)player.Health / player.MaximumHealth) * 100;
            this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Text = $"HP {player.Health}/{player.MaximumHealth}";
            this.GuiManager.GetWidget<StatusBar>("healthStatusBar").TextOffset =
                new Vector2(this.GuiManager.GetWidget<StatusBar>("healthStatusBar").FillSprite.Width - this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Font.MeasureString(this.GuiManager.GetWidget<StatusBar>("healthStatusBar").Text).X,
                    this.GuiManager.GetWidget<StatusBar>("healthStatusBar").FillSprite.Height / 2f);

            this.GuiManager.GetWidget<StatusBar>("experienceBar").Value = (player.Experience / player.NextLevelExperience) * 100;

            this.GuiManager.GetWidget<Label>("lblExperience").Text = $"{this.GuiManager.GetWidget<StatusBar>("experienceBar").Value:P2}";

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
                    (int)((72 / player.Speed) / (player.SpriteSheet.Sprite.Texture.Width / 52f));
            }
        }

        private void Handle_LoadingScreen(PacketReceivedEventArgs obj)
        {
            _loadingScreen = true;
            _sceneManager.SetActiveScene("loadingScene");
        }

        private void Handle_DialogueEnd(PacketReceivedEventArgs obj)
        {
            this.GuiManager.GetWidget<WidgetContainer>("dialogueWindow").Visible = false;
        }

        private void Handle_Dialogue(PacketReceivedEventArgs args)
        {
            _dialogueBranchName = args.Packet.ReadString();

            // Used to dynamically size the dialogue window.
            float dialogueWindowWidth = 0;

            var dialogueWindow = this.GuiManager.GetWidget<WidgetContainer>("dialogueWindow");

            dialogueWindow.Visible = true;
            dialogueWindow.ClearWidgets();

            var font = this.ContentManager.LoadAsset<SpriteFont>(Engine.ROOT_PATH + "gfx/Fonts/dialogueFont");

            var dialogueTextLabel = new Label(font);
            dialogueTextLabel.Position = new Vector2(dialogueWindow.Position.X + 20, dialogueWindow.Position.Y + 30);
            dialogueTextLabel.Text = '"' + args.Packet.ReadString() + '"';
            dialogueTextLabel.WrapText(dialogueWindow.Size.X - 20);
            dialogueTextLabel.Visible = true;

            dialogueWindow.AddWidget(dialogueTextLabel, "dialogueText");

            int responseCount = args.Packet.ReadInt32();
            var responseLabels = new Label[responseCount];
            for (int i = 0; i < responseCount; i++)
            {
                var responseLabel = new Label(font)
                {
                    Text = args.Packet.ReadString(),
                    Visible = true,
                    Tag = args.Packet.ReadString()
                };
                responseLabel.Clicked += ResponseLabel_Clicked;
                responseLabel.Mouse_Hover += ResponseLabel_Mouse_Hover;
                responseLabel.Mouse_Left += ResponseLabel_Mouse_Left;
                responseLabels[i] = responseLabel;

                dialogueWindowWidth += font.MeasureString(responseLabels[i].Text).X + Constants.DIALOGUE_SEP_X;
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

            // Make sure the dialogue window width isn't smaller than the dialogue text.
            if (dialogueWindowWidth < font.MeasureString(dialogueTextLabel.Text).X)
                dialogueWindowWidth = font.MeasureString(dialogueTextLabel.Text).X + Constants.DIALOGUE_SEP_X;

            dialogueWindow.Size = new Vector2(dialogueWindowWidth, dialogueWindow.Size.Y);
        }

        private void ResponseLabel_Mouse_Left(object sender, EventArgs e)
        {
            // Make the text regular
            ((Label)sender).Font = this.ContentManager.LoadAsset<SpriteFont>(Engine.ROOT_PATH + "/gfx/Fonts/dialogueFont");
        }

        private void ResponseLabel_Mouse_Hover(object sender, EventArgs e)
        {
            // Make the text bold
            ((Label)sender).Font = this.ContentManager.LoadAsset<SpriteFont>(Engine.ROOT_PATH + "/gfx/Fonts/dialogueFont_B");
        }

        private void ResponseLabel_Clicked(object sender, WidgetClickedEventArgs e)
        {
            var packet = new Packet();
            packet.Write(_dialogueBranchName);
            packet.Write(((IWidget)sender).Tag.ToString());
            packet.Write(((Label)sender).Text);
            this.NetHandler.SendPacket(PacketType.DIALOGUE_RESP, packet, DeliveryMethod.ReliableOrdered);
        }

        private void Handle_TargetAcquired(PacketReceivedEventArgs args)
        {
            var enemyPortraitContainer = this.GuiManager.GetWidget<WidgetContainer>("targetPortraitContainer");

            string uniqueID = args.Packet.ReadString();

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

            _sceneManager.SetActiveScene("menuScene");
        }

        private void Handle_Disconnected(object sender, EventArgs e)
        {
            if (!this.Active)
                return;

            this.GuiManager.GetWidget<Chatbox>("chatbox").Clear();

            // Unload the world.
            _worldManager.Unload();

            _sceneManager.SetActiveScene("menuScene");
        }

        private void Handle_EquipmentUpdate(PacketReceivedEventArgs args)
        {
            var equipmentContainer = this.GuiManager.GetWidget<WidgetContainer>("characterWindow").GetWidget<WidgetContainer>("equipmentContainer");

            equipmentContainer.ClearWidgets();

            for (int i = 0; i < Enum.GetNames(typeof(EquipmentSlots)).Length; i++)
            {
                bool hasItem = args.Packet.ReadBoolean();

                if (!hasItem)
                    continue;

                var itemName = args.Packet.ReadString();
                var texturePath = args.Packet.ReadString();
                EquipmentSlots slotType = (EquipmentSlots)args.Packet.ReadInt32();

                Texture2D texture2D = this.ContentManager.LoadTexture2D(Engine.ROOT_PATH + texturePath);

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
                equipSlot.Name = i.ToString();

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
                bool slotOccupied = args.Packet.ReadBoolean();

                if (slotOccupied)
                {
                    var itemName = args.Packet.ReadString();
                    var texturePath = args.Packet.ReadString();
                    var slotType = args.Packet.ReadInt32();
                    var amount = args.Packet.ReadInt32();

                    Texture2D texture2D = this.ContentManager.LoadTexture2D(Engine.ROOT_PATH + texturePath);

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
                int slotNum = int.Parse(((Picture)sender).Name);

                // Unequip the item
                var packet = new Packet();
                packet.Write(slotNum);
                this.NetHandler.SendPacket(PacketType.REQ_UNEQUIP_ITEM, packet, DeliveryMethod.ReliableOrdered);
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
                if (Lunar.Client.Utilities.Input.Input.Keyboard.IsKeyDown(Keys.LeftShift) || Lunar.Client.Utilities.Input.Input.Keyboard.IsKeyDown(Keys.RightShift))
                {
                    // Drop the item
                    var packet = new Packet();
                    packet.Write(slotNum);
                    this.NetHandler.SendPacket(PacketType.DROP_ITEM, packet, DeliveryMethod.ReliableOrdered);
                }
                else
                {
                    // Equip the item
                    var packet = new Packet();
                    packet.Write(slotNum);
                    this.NetHandler.SendPacket(PacketType.REQ_USE_ITEM, packet, DeliveryMethod.ReliableOrdered);
                }
            }
        }

        private void Handle_PlayerMessage(PacketReceivedEventArgs args)
        {
            Color color;
            ChatMessageType messageType = (ChatMessageType)args.Packet.ReadByte();
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

            this.GuiManager.GetWidget<Chatbox>("chatbox")?.AddEntry("[" + DateTime.Now.ToString("h:mm tt") + "] " + args.Packet.ReadString(), color);
        }

        public override void Update(GameTime gameTime)
        {
            MouseState newMouseState = Lunar.Client.Utilities.Input.Input.Mouse;

            if (newMouseState.LeftButton == ButtonState.Pressed && _oldMouseState.LeftButton == ButtonState.Released)
            {
                this.HandleClick();
            }

            if (Lunar.Client.Utilities.Input.Input.Keyboard.IsKeyDown(Keys.Enter))
            {
                var messageBox = this.GuiManager.GetWidget<Chatbox>("chatbox").GetWidget<Textbox>("messageEntry");
                messageBox.Active = messageBox.Active;
                _worldManager.Player.InChat = messageBox.Active;
            }

            _oldMouseState = newMouseState; // this reassigns the old state so that it is ready for next time

            _worldManager.Update(gameTime);

            base.Update(gameTime);
        }

        private void HandleClick()
        {
            Point mousePos = Lunar.Client.Utilities.Input.Input.Mouse.Position;

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
                        var selectPacket = new Packet();
                        selectPacket.Write(entity.UniqueID);
                        this.NetHandler.SendPacket(PacketType.REQ_TARGET, selectPacket, DeliveryMethod.ReliableOrdered);

                        foundTarget = true;
                    }
                }

                if (!foundTarget)
                {
                    // If we reached this point there is no valid target. We should deselect the NPC and hide the target portrait.
                    this.GuiManager.GetWidget<WidgetContainer>("targetPortraitContainer").Visible = false;

                    var deselectPacket = new Packet();
                    this.NetHandler.SendPacket(PacketType.DESELECT_TARGET, deselectPacket, DeliveryMethod.ReliableOrdered);
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
            base.OnExit();
        }

        public override void Initalize()
        {
            base.Initalize();

            this.InitalizeInterface();
        }

        private void InitalizeInterface()
        {
            this.GuiManager.LoadFromFile(Constants.FILEPATH_DATA + "interface/game/game_interface.xml", this.ContentManager);

            var chat = this.GuiManager.GetWidget<Chatbox>("chatbox");
            var messageEntry = chat.GetWidget<Textbox>("messageEntry");
            var logoutButton = this.GuiManager.GetWidget<Button>("btnLogout");
            var toggleInventoryButton = this.GuiManager.GetWidget<Button>("btnInventory");
            var toggleCharacterWindowButton = this.GuiManager.GetWidget<Button>("btnCharacter");
            var healthStatusBar = this.GuiManager.GetWidget<StatusBar>("healthStatusBar");
            var manaStatusBar = this.GuiManager.GetWidget<StatusBar>("manaStatusBar");
            var targetHealthBar = this.GuiManager.GetWidget<WidgetContainer>("targetPortraitContainer").GetWidget<StatusBar>("targetHealthBar");

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
            var packet = new Packet();
            this.NetHandler.SendPacket(PacketType.QUIT_GAME, packet, DeliveryMethod.ReliableOrdered);
        }

        private void messageEntry_ReturnPressed(object sender, EventArgs e)
        {
            string text = (sender as Textbox)?.Text;

            if (!string.IsNullOrEmpty(text))
            {
                var packet = new Packet();
                packet.Write(text);
                this.NetHandler.SendPacket(PacketType.PLAYER_MSG, packet, DeliveryMethod.Unreliable);
                ((Textbox)sender).Text = string.Empty;
                ((Textbox)sender).Active = false;
            }
        }
    }
}