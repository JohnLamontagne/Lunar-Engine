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
using System.Collections.Generic;
using Lunar.Core.Utilities;

namespace Lunar.Server.Utilities.Scripting
{
    public class ScriptManager : IService
    {
        private Dictionary<string, Script> _scripts;

        public ScriptManager()
        {
            _scripts = new Dictionary<string, Script>();
        }

        /// <summary>
        /// Retrieves the script at the specified filepath and caches it if it has not yet been loaded.
        /// </summary>
        /// <param name="scriptPath"></param>
        /// <returns></returns>
        public Script GetScript(string scriptPath)
        {
            // Don't load the same script twice
            if (!_scripts.ContainsKey(scriptPath))
            {
                var script = new Script(scriptPath);
                _scripts.Add(scriptPath, script);
            }

            return _scripts[scriptPath];
        }

        public void Initalize()
        {
        }
    }
}
