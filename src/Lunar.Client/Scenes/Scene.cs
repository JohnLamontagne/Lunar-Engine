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
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Lunar.Client.GUI;
using Lunar.Client.Net;
using Lunar.Client.Utilities.Services;
using Lunar.Core.Net;
using Lunar.Core.Utilities;

namespace Lunar.Client.Scenes
{
    public abstract class Scene : ISubject, IGameComponentContainer
    {
        private GUIManager _guiManager;
        private ContentManager _contentManager;
        private List<GameComponent> _gameComponents;

        protected GUIManager GuiManager => _guiManager;
        protected ContentManager ContentManager => _contentManager;

        public bool Active { get; private set; }

        protected Scene(ContentManager contentManager, GameWindow gameWindow)
        {
            _contentManager = contentManager;
            _guiManager = new GUIManager();
            _gameComponents = new List<GameComponent>();

            // Allow the server to demand that the client play music.
            // We handle this here so that we may easily play music in any scene.
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.PLAY_MUSIC, this.Handle_PlayMusic);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.STOP_MUSIC, this.Handle_StopMusic);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.PLAY_SOUND, this.Handle_PlaySound);
        }

        private void Handle_PlaySound(PacketReceivedEventArgs args)
        {
            string fileName = args.Message.ReadString();
            float volume = args.Message.ReadFloat();

            SoundEffect soundEffect = _contentManager.Load<SoundEffect>(Constants.FILEPATH_SFX + "/" + fileName);
            soundEffect.Play();
        }

        private void Handle_PlayMusic(PacketReceivedEventArgs args)
        {
            string fileName = args.Message.ReadString();

            Song song = _contentManager.Load<Song>(Constants.FILEPATH_MUSIC + "/" + fileName);
            MediaPlayer.Play(song);
        }

        private void Handle_StopMusic(PacketReceivedEventArgs args)
        {
            MediaPlayer.Stop();
        }

        internal void Exit()
        {
            this.Active = false;

            this.OnExit();
        }

        internal void Enter()
        {
            this.Active = true;

            this.OnEnter();
        }

        protected virtual void OnEnter()
        {
           
        }

        protected virtual void OnExit()
        {
            
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var gameComponent in _gameComponents)
                gameComponent.Update(gameTime);

            this.GuiManager.Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {

            // Prepare the world for lighting.
            // This also ensures that GuiManager can call Begin and render outside of worldspace
            spriteBatch.End();

            Client.ServiceLocator.GetService<LightManagerService>().Component.Draw(gameTime);

            this.GuiManager.Begin(spriteBatch);
            this.GuiManager.Draw(spriteBatch);
            this.GuiManager.End(spriteBatch);
        }

        public event EventHandler<SubjectEventArgs> EventOccured;

        public void AddGameComponent(IGameComponent gameComponent)
        {
            throw new NotImplementedException();
        }

        public void RemoveGameComponent(IGameComponent gameComponent)
        {
            throw new NotImplementedException();
        }

        public IGameComponent GetGameComponent(string name)
        {
            throw new NotImplementedException();
        }
    }
}