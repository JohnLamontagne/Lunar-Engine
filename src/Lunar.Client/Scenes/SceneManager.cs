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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.Utilities;

namespace Lunar.Client.Scenes
{
    public class SceneManager : ISubject, IService
    {
        private readonly Dictionary<string, Scene> _scenes;
        private Scene _activeScreen;

        public Scene ActiveScreen { get { return _activeScreen; } }

        public SceneManager()
        {
            _scenes = new Dictionary<string, Scene>();
        }

        public void AddScene(Scene scene, string name)
        {
            _scenes.Add(name, scene);
        }

        public void RemoveScene(string name)
        {
            _scenes.Remove(name);
        }

        public void RemoveScene(Scene screen)
        {
            var screenName = _scenes.FirstOrDefault(x => x.Value == screen).Key;

            this.RemoveScene(screenName);
        }

        public T GetScene<T>(string screenName) where T : Scene
        {

            if (_scenes.TryGetValue(screenName, out Scene value))
            {
                if (value.GetType() == typeof(T))
                {
                    return (T)value;
                }
            }

            return default(T);
        }

        public void SetActiveScene(string screenName)
        {
            _activeScreen?.Exit();
            _activeScreen = _scenes[screenName];
            _activeScreen.Enter();
        }

        internal void Update(GameTime gameTime)
        {
            _activeScreen?.Update(gameTime);
        }

        internal void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _activeScreen?.Draw(gameTime, spriteBatch);
        }

        public event System.EventHandler<SubjectEventArgs> EventOccured;

        public void Initalize()
        {
            throw new System.NotImplementedException();
        }
    }
}