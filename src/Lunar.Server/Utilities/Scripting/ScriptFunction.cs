using System;

namespace Lunar.Server.Utilities.Scripting
{
    public class ScriptFunction
    {
        private Func<ScriptActionArgs, object> _func;

        public ScriptFunction(Func<ScriptActionArgs, object> func)
        {
            _func = func;
        }

        public object Invoke(ScriptActionArgs args)
        {
            return _func.Invoke(args);
        }
    }
}
