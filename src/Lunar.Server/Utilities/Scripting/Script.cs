using NLua;
using System;
using System.Diagnostics;
using System.IO;

namespace Lunar.Server.Utilities.Scripting
{
    public class Script
    {
        private Lua _lua;
        private string _scriptFilePath;
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

        public Script(string scriptFilePath)
        {
            _scriptFilePath = scriptFilePath;
            _fileWatchStopWatch = new Stopwatch();

            _lua = new Lua();
            _lua.LoadCLRPackage();
            _lua.RegisterFunction("typeof", typeof(ScriptUtilities).GetMethod("GetTypeOf"));

            this.Execute();
            this.CreateFileWatcher();
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

        private void Execute()
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
            this.Execute();
        }
    }
}
