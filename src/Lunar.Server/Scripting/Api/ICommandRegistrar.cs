using System;
using Lunar.Server.World.Actors;

namespace Lunar.Server.Scripting.Api
{
    public interface ICommandRegistrar
    {
        void Add(string command, Action<CommandContext> handler);
    }

    public sealed class CommandContext
    {
        public Player Player { get; }
        public string[] Args { get; }

        public CommandContext(Player player, string[] args)
        {
            Player = player;
            Args = args;
        }

        public string this[int i] => i >= 0 && i < Args.Length ? Args[i] : string.Empty;
    }
}
