using System;

namespace Lunar.Server.World.Actors.Actions.Player
{
    class PlayerInteractAction : IAction<Actors.Player>
    {
        public void Execute(Actors.Player player)
        {
            foreach (var mapObject in player.Layer.GetCollidingMapObjects(player.Descriptor.Position, player.CollisionBounds))
            {
                mapObject.OnInteract(player);
            }

            // Try to attack the target
            if (player.Target != null)
            {
                if (player.Target.Attackable)
                {
                    player.Target.OnAttacked(player, (int)(player.Descriptor.Stats.Strength * new Random().NextDouble()));
                }
            }
        }
    }
}
