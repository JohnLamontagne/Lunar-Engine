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