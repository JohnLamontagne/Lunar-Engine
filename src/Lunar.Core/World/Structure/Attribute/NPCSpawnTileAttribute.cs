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
using Lunar.Core.Content.Graphics;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Core.World.Structure.Attribute
{
    [Serializable]
    public class NPCSpawnTileAttribute : TileAttribute
    {
        public override Color Color => new Color(Color.Blue, 100);

        public string NPCID { get; set; }

        public int RespawnTime { get; set; }

        public int MaxSpawns { get; set; }

        public NPCSpawnTileAttribute(string npcID, int respawnTime, int maxSpawns)
        {
            this.NPCID = npcID;
            this.RespawnTime = respawnTime;
            this.MaxSpawns = maxSpawns;
        }
    }
}