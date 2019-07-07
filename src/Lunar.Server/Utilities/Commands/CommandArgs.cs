using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;

namespace Lunar.Server.Utilities.Commands
{
    public class CommandArgs : ServerArgs
    {
        private object[] _args;

        public Player Player { get; }

        public object Invoker { get; }

        public object this[int i]
        {
            get
            {
                return _args[i];
            }
        }

        public CommandArgs(object invoker, Player player, object[] args)
        {
            _args = args;

            this.Invoker = invoker;
            this.Player = player;
        }
    }
}