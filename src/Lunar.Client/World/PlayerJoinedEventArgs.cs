using Lunar.Client.World.Actors;

namespace Lunar.Client.World
{
    public class PlayerJoinedEventArgs
    {
        public Player Player { get; }

        public PlayerJoinedEventArgs(Player player)
        {
            this.Player = player;
        }
    }
}