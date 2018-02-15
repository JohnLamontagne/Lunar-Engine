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
