/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using System;

namespace Lunar.Server.World.Actors.Actions.Player
{
    class PlayerInteractAction : IAction<Actors.Player>
    {
        public void Execute(Actors.Player player)
        {
            foreach (var mapObject in player.Layer.GetCollidingMapObjects(player.Descriptor.Position, player.Descriptor.CollisionBounds))
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
