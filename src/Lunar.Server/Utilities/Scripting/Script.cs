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
using NLua;
using System;
using System.Diagnostics;
using System.IO;

namespace Lunar.Server.Utilities.Scripting
{
    public class Script
    {
        /// <summary>
        /// Share one instance of the Lua script engine across scripts.
        /// </summary>
        private Lua _lua;

        private bool _isScriptFile;

        private string _scriptFilePath;
        private string _script;
        private FileSystemWatcher _fsWatcher;
        private Stopwatch _fileWatchStopWatch;
        private long _lastFileWatchChange;

        public event EventHandler<EventArgs> ScriptChanged;

        /// <summary>
        /// Returns specified global value
        /// </summary>
        /// <param name="path">Path to global value</param>
        /// <returns></returns>
        public object this[string path]
        {
            get { return _lua[path]; }
        }

        public LuaTable GetTable(string tablePath)
        {
            return _lua.GetTable(tablePath);
        }

        public LuaFunction GetFunction(string functionPath)
        {
            return _lua.GetFunction(functionPath);
        }

        public Script(string script, bool isScriptFile = true)
        {
            _isScriptFile = isScriptFile;

            if (isScriptFile)
            {
                _scriptFilePath = script;
            }
            else
            {
                _script = script;
            }
 
            _fileWatchStopWatch = new Stopwatch();

            _lua = new Lua();
            _lua.LoadCLRPackage();
            _lua.RegisterFunction("typeof", typeof(ScriptUtilities).GetMethod("GetTypeOf"));

            if (isScriptFile)
            {
                this.ExecuteScriptFile();
                this.CreateFileWatcher();
            }
            else
            {
                this.ExecuteScript();
            }
        }

        private void CreateFileWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            _fsWatcher = new FileSystemWatcher();
            _fsWatcher.Path = Path.GetDirectoryName(_scriptFilePath);
            _fsWatcher.Filter = Path.GetFileName(_scriptFilePath);

            // Add event handlers.
            _fsWatcher.Changed += Watcher_Changed;

            // Begin watching.
            _fsWatcher.EnableRaisingEvents = true;

            _fileWatchStopWatch.Start();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Make sure we don't reload the script multiple times from multiple filechanged events being fired (happens due to way filewatcher works)
            DateTime lastWriteTime = File.GetLastWriteTime(_scriptFilePath);

            if (_fileWatchStopWatch.ElapsedMilliseconds > _lastFileWatchChange + 1000)
            {
                this.ScriptChanged?.Invoke(this, new EventArgs());
                _lastFileWatchChange = _fileWatchStopWatch.ElapsedMilliseconds;

                Console.WriteLine($"Script {_scriptFilePath} reloaded!");
            }
            {

            }
        }

        private void ExecuteScript()
        {
            
        }

        private void ExecuteScriptFile()
        {
            try
            {
                using (FileStream fs = File.Open(_scriptFilePath, FileMode.Open))
                {

                }
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                try
                {
                    _lua.DoFile(_scriptFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Script error: {ex.Message}.");
                }
            }
        }

        /// <summary>
        /// Executes the Script
        /// </summary>
        public void ReExecute()
        {
            if (_isScriptFile)
            {
                this.ExecuteScriptFile();
                this.CreateFileWatcher();
            }
            else
            {
                this.ExecuteScript();
            }
        }
    }
}
