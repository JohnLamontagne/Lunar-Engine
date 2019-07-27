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

using System;

namespace Lunar.Server.Utilities
{
    public class GameTimer
    {
        private double _endTime;
        private bool _finished;
        private bool _reset;
        private long _duration;

        private Func<bool> _resetWhen;

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

        public bool ResetWhen(Func<bool> expression)
        {
            _resetWhen = expression;
            return this.ResetIf(expression);
        }

        public bool ResetIf(Func<bool> expression)
        {
            if (expression())
            {
                this.Reset();
                return true;
            }

            return false;
        }

        public bool ResetWhen(Func<bool> expression, int duration)
        {
            _resetWhen = expression;
            _duration = duration;
            return this.ResetIf(expression);
        }

        internal void Update(GameTime gameTime)
        {
            if (_reset)
            {
                if (_resetWhen == null || !_resetWhen())
                {
                    _endTime = gameTime.TotalGameTime.TotalMilliseconds + _duration;
                    _reset = false;
                    _finished = false;
                }
            }
            else if (gameTime.TotalGameTime.TotalMilliseconds > _endTime && !_finished)
            {
                _finished = true;
                this.TimerFinished?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler<EventArgs> TimerFinished;
    }
}