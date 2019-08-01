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
    internal class PlayerInteractAction : IAction<Actors.Player>
    {
        public void Execute(Actors.Player player)
        {
            foreach (var mapObject in player.Layer.GetCollidingMapObjects(player.Descriptor.Position, player.Descriptor.CollisionBounds))
            {
                mapObject.OnInteract(player);
            }

            var target = player.Target != null ? player.Target : player.FindTarget();

            // Try to attack the target
            if (target != null)
            {
                if (target.Attackable)
                {
                    target.OnAttacked(player, (int)(player.Descriptor.Stats.Strength * new Random().NextDouble()));
                }
                else
                {
                    // Is it an NPC that has dialogue?
                    var npc = target as NPC;

                    if (npc != null && npc.Dialogue != null)
                    {
                        if (npc.Dialogue.BranchExists(npc.DialogueBranch))
                        {
                            npc.Dialogue.Start(npc.DialogueBranch, player);
                        }
                    }
                }
            }
        }
    }
}