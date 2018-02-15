using System;

namespace Lunar.Server.Utilities.Scripting
{
    public class ScriptAction
    {
        private Action<ScriptActionArgs> _action;

        public ScriptAction(Action<ScriptActionArgs> action)
        {
            _action = action;
        }

        public void Invoke(ScriptActionArgs args)
        {
            try
            {
                _action.Invoke(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Script error: {ex.Message}.");
            }
        }
    }
}
