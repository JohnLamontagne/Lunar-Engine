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

namespace Lunar.Server.Utilities
{
    public class GameTimer
    {
        private long _endTime;
        private bool _finished;
        private bool _reset;
        private long _duration;

        public bool Finished { get { return _finished; } }
        public long Duration { get { return _duration; } }

        /// <summary>
        /// Creates instance of GameTimer
        /// </summary>
        /// <param name="duration">Duration of timer in milliseconds</param>
        public GameTimer(long duration)
        {
            _duration = duration;
            _reset = true;
        }

        /// <summary>
        /// Resets the GameTimer with a new duration
        /// </summary>
        /// <param name="duration">Duration of timer in milliseconds</param>
        public void Reset(long duration)
        {
            _duration = duration;
            _reset = true;
        }

        /// <summary>
        /// Resets the GameTimer
        /// </summary>
        public void Reset()
        {
            _reset = true;
        }

        internal void Update(GameTime gameTime)
        {
            if (gameTime.TotalElapsedTime > _endTime)
            {
                _finished = true;
                this.TimerFinished?.Invoke(this, new EventArgs());
            } else if (_reset)
            {
                _endTime = gameTime.TotalElapsedTime + _duration;
                _reset = false;
            }
        }

        public event EventHandler<EventArgs> TimerFinished;
    }
}
