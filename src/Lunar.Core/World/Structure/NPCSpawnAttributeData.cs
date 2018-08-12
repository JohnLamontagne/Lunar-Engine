using System;
using Lunar.Core.World.Actor.Descriptors;

namespace Lunar.Core.World.Structure
{
    [Serializable]
    public class NPCSpawnAttributeData : AttributeData
    {
        public string NPCID { get; set; }

        public int RespawnTime { get; set; }

        public int MaxSpawns { get; set; }

        public NPCSpawnAttributeData(NPCDescriptor npc, int respawnTime, int maxSpawns)
        {
            this.NPCID = npc.Name;
            this.RespawnTime = respawnTime;
            this.MaxSpawns = maxSpawns;
        }
    }
}
