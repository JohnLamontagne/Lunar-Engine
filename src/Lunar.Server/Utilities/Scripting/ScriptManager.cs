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
using System.Collections.Generic;
using IronPython.Hosting;
using Lunar.Core;
using Lunar.Core.Utilities;
using Microsoft.Scripting.Hosting;

namespace Lunar.Server.Utilities.Scripting
{
    public class ScriptManager : IService
    {
        private Dictionary<string, Script> _scripts;
        private ScriptEngine _scriptEngine;

        public ScriptManager()
        {
            _scripts = new Dictionary<string, Script>();

            _scriptEngine = Python.CreateEngine();
            var oldSearchPaths = _scriptEngine.GetSearchPaths();
            oldSearchPaths.Add(Constants.FILEPATH_SCRIPTS);
            _scriptEngine.SetSearchPaths(oldSearchPaths);
        }

        public Script CreateScript(string scriptPath)
        {
            if (_scripts.ContainsKey(scriptPath))
                return _scripts[scriptPath];

            ScriptSource compiledScript = _scriptEngine.CreateScriptSourceFromFile(scriptPath);
            Script script = new Script(_scriptEngine, compiledScript);
            _scripts.Add(scriptPath, script);

            return script;
        }

        public Script CreateScriptFromSource(string source)
        {
            string key = source.GetHashCode().ToString();

            if (_scripts.ContainsKey(key))
                return _scripts[key];

            ScriptSource compiledScript = _scriptEngine.CreateScriptSourceFromString(source);
            Script script = new Script(_scriptEngine, compiledScript);
            _scripts.Add(key, script);

            return script;
        }

        public void HandleException(Exception ex)
        {
            ExceptionOperations eo = _scriptEngine.GetService<ExceptionOperations>();
            string error = eo.FormatException(ex);
            Logger.LogEvent($"Script error: {error}", LogTypes.ERROR, ex.StackTrace);
        }

        public void Initalize()
        {
        }
    }
}
