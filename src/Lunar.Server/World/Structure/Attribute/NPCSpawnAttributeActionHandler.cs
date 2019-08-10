using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure.Attribute;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Structure.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lunar.Server.World.Structure.Attribute
{
    internal class NPCSpawnAttributeActionHandler : TileAttributeActionHandler
    {
        private double _nextNPCSpawnTime;
        private NPCHeartbeatListener _heartbeatListener;

        public NPCSpawnAttributeActionHandler()
        {
            _heartbeatListener = new NPCHeartbeatListener();
        }

        public override void OnInitalize(ITileAttributeArgs args)
        {
        }

        public override void OnPlayerEntered(ITileAttributeArgs args)
        {
        }

        public override void OnPlayerLeft(ITileAttributeArgs args)
        {
        }

        public override void OnUpdate(ITileAttributeArgs args)
        {
            Tile tile = (args as TileAttributeUpdateArgs).Tile;
            GameTime gameTime = (args as TileAttributeUpdateArgs).GameTime;
            var attribute = args.Attribute as NPCSpawnTileAttribute;

            if (_nextNPCSpawnTime <= gameTime.TotalGameTime.TotalMilliseconds && _heartbeatListener.NPCs.Count < attribute.MaxSpawns)
            {
                var npcDesc = Engine.Services.Get<NPCManager>().GetNPC(attribute.NPCID);

                if (npcDesc == null)
                {
                    Engine.Services.Get<Logger>().LogEvent($"Error spawning NPC: {attribute.NPCID} does not exist!", LogTypes.ERROR, new Exception($"Error spawning NPC: {attribute.NPCID} does not exist!"));
                    return;
                }

                NPC npc = new NPC(npcDesc, tile.Layer.Map)
                {
                    Layer = tile.Layer
                };
                npc.WarpTo(tile.Position);

                // This allows the tile spawner to keep track of npcs that exist, and respawn if neccessary (i.e., they die).
                _heartbeatListener.NPCs.Add(npc);

                _nextNPCSpawnTime = gameTime.TotalGameTime.TotalMilliseconds + (attribute.RespawnTime * 1000);
            }
        }

        public class NPCSpawnerEventArgs : EventArgs
        {
            public string NPCID { get; }

            public int Count { get; }

            public Vector Position { get; }

            public NPCHeartbeatListener HeartbeatListener { get; }

            public NPCSpawnerEventArgs(string npcID, int count, Vector position, NPCHeartbeatListener heartBeatListener)
            {
                this.NPCID = npcID;
                this.Count = count;
                this.Position = position;

                this.HeartbeatListener = heartBeatListener;
            }
        }
    }
}