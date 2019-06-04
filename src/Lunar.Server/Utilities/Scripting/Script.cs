using Microsoft.Scripting.Hosting;
using System;

namespace Lunar.Server.Utilities.Scripting
{
    public class Script
    {
        private ScriptEngine _scriptEngine;
        private ScriptSource _compiledScript;
        private ScriptScope _scope;

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
            }
            catch (Microsoft.Scripting.SyntaxErrorException ex)
            {
                Logger.LogEvent($"Script Error on line {ex.Line}: {ex.Message} in {compiledScript.Path}: ", LogTypes.ERROR, ex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Script Error: {ex.Message} in {compiledScript.Path}: ", LogTypes.ERROR, ex.StackTrace);
            }
        }

        public T GetVariable<T>(string varName)
        {
            try
            {
                return _scope.GetVariable<T>(varName);
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Script Error: {ex.Message} in {this._compiledScript.Path}: ", LogTypes.ERROR, ex.StackTrace);
                return default;
            }
        }

        public void SetVariable<T>(string varName, T value)
        {
            _scope.SetVariable(varName, value);
        }

        public void Invoke(string functionName, GameEventArgs args)
        {
            try
            {
                dynamic funct = _scope.GetVariable(functionName);
                funct(args);
            }
            catch (Microsoft.Scripting.SyntaxErrorException ex)
            {
                Logger.LogEvent($"Script Error on line {ex.Line}: {ex.Message} in {_compiledScript.Path}: ", LogTypes.ERROR, ex.StackTrace);
            }
            catch (Exception ex)
            {
                Logger.LogEvent($"Script Error: {ex.Message} in {_compiledScript.Path}: ", LogTypes.ERROR, ex.StackTrace);
            }
        }

        public T Invoke<T>(string functionName, GameEventArgs args)
        {
            _compiledScript.Execute(_scope);
            dynamic funct = _scope.GetVariable(functionName);
            return funct(args);
        }

    }
}
