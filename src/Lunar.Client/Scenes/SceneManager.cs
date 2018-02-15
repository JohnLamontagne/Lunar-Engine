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