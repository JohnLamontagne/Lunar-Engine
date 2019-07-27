using Lunar.Core;
using Lunar.Core.Utilities;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Lunar.Server.Utilities.Scripting
{
    public class Script
    {
        private ScriptEngine _scriptEngine;
        private ScriptSource _compiledScript;
        private ScriptScope _scope;
        private FileSystemWatcher _fsWatcher;
        private Stopwatch _fileWatchStopWatch;
        private long _lastFileWatchChange;
        private bool _liveSource;

        public bool LiveSource
        {
            get => _liveSource;
            set
            {
                if (_fsWatcher == null)
                    _fsWatcher = new FileSystemWatcher();

                _fsWatcher.EnableRaisingEvents = value;
                _liveSource = value;
            }
        }

        public event EventHandler<EventArgs> ScriptChanged;

        public Script(ScriptEngine scriptEngine, ScriptSource compiledScript)
        {
            try
            {
                _scriptEngine = scriptEngine;
                _compiledScript = compiledScript;
                _scope = _scriptEngine.CreateScope();
                _scope.SetVariable("gameTimeManager", new GameTimerManager());
                _compiledScript.Execute(_scope);
                Console.WriteLine($"Successfully loaded script {compiledScript.Path}");

                this.LiveSource = false;
            }
            catch (Microsoft.Scripting.SyntaxErrorException ex)
            {
                Engine.Services.Get<Logger>().LogEvent($"Script Error on line {ex.Line}: {ex.Message} in {compiledScript.Path}: ", LogTypes.ERROR, ex);
            }
            catch (MissingMemberException ex)
            {
                Engine.Services.Get<Logger>().LogEvent($"Script Error: {ex.Message} in {compiledScript.Path}: ", LogTypes.ERROR, ex);
            }
        }

        private void CreateFileWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            _fsWatcher = new FileSystemWatcher();
            _fsWatcher.Path = Path.GetDirectoryName(_compiledScript.Path);
            _fsWatcher.Filter = Path.GetFileName(_compiledScript.Path);

            // Add event handlers.
            _fsWatcher.Changed += Watcher_Changed;

            // Begin watching.
            _fsWatcher.EnableRaisingEvents = true;

            _fileWatchStopWatch.Start();
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            // Make sure we don't reload the script multiple times from multiple filechanged events being fired (happens due to way filewatcher works)
            DateTime lastWriteTime = File.GetLastWriteTime(_compiledScript.Path);

            if (_fileWatchStopWatch.ElapsedMilliseconds > _lastFileWatchChange + 1000)
            {
                this.ScriptChanged?.Invoke(this, new EventArgs());
                _lastFileWatchChange = _fileWatchStopWatch.ElapsedMilliseconds;

                this.Reload(e.FullPath);
            }
        }

        public void Reload(string scriptPath)
        {
            _compiledScript = _compiledScript.Engine.CreateScriptSourceFromFile(scriptPath);
            _compiledScript.Execute(_scope);

            Console.WriteLine($"Script {_compiledScript.Path} reloaded!");

            this.ScriptChanged?.Invoke(this, new EventArgs());
        }

        public T GetVariable<T>(string varName)
        {
            try
            {
                return _scope.GetVariable<T>(varName);
            }
            catch (Exception ex)
            {
                Engine.Services.Get<Logger>().LogEvent($"Script Error: {ex.Message} in {this._compiledScript.Path}: ", LogTypes.ERROR, ex);
                return default;
            }
        }

        public List<KeyValuePair<string, T>> GetVariables<T>()
        {
            return new List<KeyValuePair<string, T>>(
                from i in _scope.GetItems()
                .Where(pair => pair.Value is T)
                select new KeyValuePair<string, T>(i.Key, i.Value));
        }

        public void SetVariable<T>(string varName, T value)
        {
            _scope.SetVariable(varName, value);
        }

        public void Invoke(string functionName, ServerArgs args)
        {
            try
            {
                dynamic funct = _scope.GetVariable(functionName);
                funct(args);
            }
            catch (Microsoft.Scripting.SyntaxErrorException ex)
            {
                Engine.Services.Get<Logger>().LogEvent($"Script Error on line {ex.Line}: {ex.Message} in {_compiledScript.Path}: ", LogTypes.ERROR, ex);
            }
            catch (Exception ex)
            {
                Engine.Services.Get<Logger>().LogEvent($"Script Error: {ex.Message} in {_compiledScript.Path}: ", LogTypes.ERROR, ex);
            }
        }

        public T Invoke<T>(string functionName, ServerArgs args)
        {
            _compiledScript.Execute(_scope);
            dynamic funct = _scope.GetVariable(functionName);
            return funct(args);
        }
    }
}