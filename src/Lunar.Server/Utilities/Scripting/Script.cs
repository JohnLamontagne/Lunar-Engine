using Lunar.Core;
using Lunar.Core.Utilities;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                Engine.Services.Get<Logger>().LogEvent($"Script Error on line {ex.Line}: {ex.Message} in {compiledScript.Path}: ", LogTypes.ERROR, ex);
            }
            catch (MissingMemberException ex)
            {
                Engine.Services.Get<Logger>().LogEvent($"Script Error: {ex.Message} in {compiledScript.Path}: ", LogTypes.ERROR, ex);
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
                Engine.Services.Get<Logger>().LogEvent($"Script Error: {ex.Message} in {this._compiledScript.Path}: ", LogTypes.ERROR, ex);
                return default;
            }
        }

        public List<KeyValuePair<string, T>> GetVariables<T>()
        {
            return (_scope.GetItems().Where(pair => pair.Value is T)
                .Cast<KeyValuePair<string, T>>()
                .ToList());
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