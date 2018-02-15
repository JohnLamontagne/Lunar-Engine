using System.Collections.Generic;

namespace Lunar.Server.Utilities
{
    public class GameTimerManager
    {
        private static GameTimerManager _instance;
        public static GameTimerManager Instance {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameTimerManager();
                }

                return _instance;
            }
        }

        private Dictionary<string, GameTimer> _gameTimers;

        public GameTimerManager()
        {
            _gameTimers = new Dictionary<string, GameTimer>();
        }

        public void Register(string name, GameTimer gameTimer)
        {
            if (!_gameTimers.ContainsKey(name))
                _gameTimers.Add(name, gameTimer);
        }

        public void DeRegister(string name)
        {
            _gameTimers.Remove(name);
        }

        public GameTimer GetTimer(string name)
        {
            return _gameTimers[name];
        }

        public void Update(GameTime gameTime)
        {
            foreach (var gameTimer in _gameTimers.Values)
            {
                gameTimer.Update(gameTime);
            }
        }
    }
}
