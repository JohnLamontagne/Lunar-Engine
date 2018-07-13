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

namespace Lunar.Server.Utilities.Scripting
{
    public class ScriptActionArgs : EventArgs
    {
        private object _invoker;
        private object[] _args;

        public object Invoker { get { return _invoker; } }

        public object this[int index]
        {
            get
            {
                if (index >= _args.Length)
                {
                    Console.WriteLine($"Script error: attempting to access invalid argument at {index}!");
                }

                return _args[index];
            }
        }

        public ScriptActionArgs(object invoker, params object[] args)
        {
            _invoker = invoker;
            _args = args;
        }
    }
}
