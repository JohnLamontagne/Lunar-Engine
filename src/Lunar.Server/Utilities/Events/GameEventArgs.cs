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
namespace Lunar.Server.Utilities.Events
{
    public class GameEventArgs
    {
        /// <summary>
        /// Name of the event which occurred
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Object at which the game event occurred
        /// </summary>
        public IGameEventSource EventSource { get; }

        public GameEventArgs(string eventName, IGameEventSource invoker)
        {
            this.EventName = eventName;
            this.EventSource = invoker;
        }
    }
}
