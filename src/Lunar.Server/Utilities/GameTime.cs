/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using System.Diagnostics;

namespace Lunar.Server.Utilities
{
    public class GameTime
    {

        private readonly Stopwatch _runningStopWatch;
        private readonly Stopwatch _updateStopWatch;

        public long TotalElapsedTime => _runningStopWatch.ElapsedMilliseconds;

        public float UpdateTimeInMilliseconds { get; private set; }

        public float UpdateTimeInSeconds => this.UpdateTimeInMilliseconds / 1000;

        public GameTime()
        {
            _runningStopWatch = new Stopwatch();
            _updateStopWatch = new Stopwatch();

            _runningStopWatch.Start();
        }

        public GameTime Start()
        {
            _runningStopWatch.Start();
            return this;
        }

        public GameTime Restart()
        {
            _runningStopWatch.Restart();
            return this;
        }


        public void Update()
        {
            _updateStopWatch.Stop();

            // Store the last update time
            this.UpdateTimeInMilliseconds = _updateStopWatch.Elapsed.Milliseconds;

            // Start running it again
            _updateStopWatch.Reset();
            _updateStopWatch.Start();
        }
    }
}