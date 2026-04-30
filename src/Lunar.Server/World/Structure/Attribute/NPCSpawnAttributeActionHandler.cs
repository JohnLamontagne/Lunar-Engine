using Lunar.Core;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure.Attribute;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Conversation;
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
        private readonly NPCHeartbeatListener _heartbeatListener;
        private readonly NPCManager _npcManager;
        private readonly Logger _logger;
        private readonly ScriptManager _scriptManager;
        private readonly DialogueManager _dialogueManager;

        public NPCSpawnAttributeActionHandler(NPCManager npcManager, Logger logger, ScriptManager scriptManager, DialogueManager dialogueManager)
        {
            _heartbeatListener = new NPCHeartbeatListener();
            _npcManager = npcManager;
            _logger = logger;
            _scriptManager = scriptManager;
            _dialogueManager = dialogueManager;
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
                var npcDesc = _npcManager.Get(attribute.NPCID);

                if (npcDesc == null)
                {
                    _logger.LogEvent($"Error spawning NPC: {attribute.NPCID} does not exist!", LogTypes.ERROR, new Exception($"Error spawning NPC: {attribute.NPCID} does not exist!"));
                    return;
                }

                NPC npc = new NPC(npcDesc, tile.Layer.Map, _scriptManager, _logger, _dialogueManager)
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