using System;
using System.Collections.Generic;

namespace Lunar.Server.Utilities.Plugin
{
    public abstract class Plugin
    {
        public abstract string Author { get; }

        public abstract string Description { get; }

        public abstract string Version { get; }

        public Dictionary<string, List<Action>> Hooks { get; }

        protected Plugin()
        {
            this.Hooks = new Dictionary<string, List<Action>>();
        }

        public abstract void Initalize();
    }
}
