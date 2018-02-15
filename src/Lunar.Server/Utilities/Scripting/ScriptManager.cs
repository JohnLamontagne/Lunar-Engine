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
