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
using System.Collections.Generic;

namespace Lunar.Server.Utilities
{
    public class GameTimerManager
    {
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

        public void Remove(string name)
        {
            _gameTimers.Remove(name);
        }

        public GameTimer Get(string name)
        {
            if (!_gameTimers.ContainsKey(name))
                return null;

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
