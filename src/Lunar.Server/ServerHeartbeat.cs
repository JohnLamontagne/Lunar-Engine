using Lunar.Server.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server
{
    // The following code is heavily derived from the logic driving GameTimers in Monogame. It was implemented as a way to best keep synchronization between clients and the server.
    public class ServerHeartbeat
    {
        private Stopwatch _serverTimer;

        private long _currentTicks;
        private long _previousTicks;
        private TimeSpan _accumulatedElapsedTime;

        private TimeSpan _targetElapsedTime;
        private TimeSpan _maxElapsedTime;
        private int _updateFrameLag = 0;

        private Action<GameTime> DoUpdate { get; set; }

        public ServerHeartbeat(Action<GameTime> doUpdateHandler)
        {
            this.DoUpdate = doUpdateHandler;

            _serverTimer = Stopwatch.StartNew();
            _currentTicks = 0;
            _previousTicks = 0;
            _accumulatedElapsedTime = TimeSpan.Zero;
            _targetElapsedTime =  TimeSpan.FromTicks(83334);
            _maxElapsedTime = TimeSpan.FromMilliseconds(500);
        }

        public void Update(GameTime gameTime)
        {
RESTART:
            _currentTicks = _serverTimer.Elapsed.Ticks;
            _accumulatedElapsedTime += TimeSpan.FromTicks(_currentTicks - _previousTicks);
            _previousTicks = _currentTicks;

            if (_accumulatedElapsedTime < _targetElapsedTime)
            {
                var sleepTime = (_targetElapsedTime - _accumulatedElapsedTime).TotalMilliseconds;

                TimerHelper.SleepForNoMoreThan(sleepTime);
                goto RESTART; // bad bad bad. but again, this is based off of MonoGame, and I don't want to bother figuring out a way to do it differently. Blugh
            }

            if (_accumulatedElapsedTime > _maxElapsedTime)
                _accumulatedElapsedTime = _maxElapsedTime;

            gameTime.ElapsedGameTime = _targetElapsedTime;
            var stepCount = 0;

            while (_accumulatedElapsedTime >= _targetElapsedTime && !Server.ShutDown)
            {
                gameTime.TotalGameTime += _targetElapsedTime;
                _accumulatedElapsedTime -= _targetElapsedTime;
                ++stepCount;

                this.DoUpdate.Invoke(gameTime);
            }

            _updateFrameLag += Math.Max(0, stepCount - 1);

            if (gameTime.IsRunningSlowly)
            {
                if (_updateFrameLag == 0)
                    gameTime.IsRunningSlowly = false;
            }
            else if (_updateFrameLag >= 5)
            {
                gameTime.IsRunningSlowly = true;
            }

            if (stepCount == 1 && _updateFrameLag > 0)
                _updateFrameLag--;

            gameTime.ElapsedGameTime = TimeSpan.FromTicks(_targetElapsedTime.Ticks * stepCount);
        }
    }
}
