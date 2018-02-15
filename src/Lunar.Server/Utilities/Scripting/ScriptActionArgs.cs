using System;

namespace Lunar.Server.Utilities.Scripting
{
    public class ScriptActionArgs : EventArgs
    {
        private object _invoker;
        private object[] _args;

        public object Invoker { get { return _invoker; } }

        public object this[int index]
        {
            get
            {
                if (index >= _args.Length)
                {
                    Console.WriteLine($"Script error: attempting to access invalid argument at {index}!");
                }

                return _args[index];
            }
        }

        public ScriptActionArgs(object invoker, params object[] args)
        {
            _invoker = invoker;
            _args = args;
        }
    }
}
