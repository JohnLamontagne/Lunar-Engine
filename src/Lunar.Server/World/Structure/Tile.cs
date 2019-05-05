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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Lidgren.Network;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class Tile
    {
        private TileDescriptor _descriptor;
        private NPCHeartbeatListener _heartbeatListener;
        private long _nextNPCSpawnTime;
        private Rect _collisionArea;

        public TileDescriptor Descriptor => _descriptor;

        public Tile(TileDescriptor descriptor)
        {
            _descriptor = descriptor;

            if (descriptor.SpriteInfo != null)
                _collisionArea = new Rect(descriptor.SpriteInfo.Transform.Position.X, descriptor.SpriteInfo.Transform.Position.Y, Settings.TileSize, Settings.TileSize);
            else
                _collisionArea = new Rect(descriptor.Position.X, descriptor.Position.Y, Settings.TileSize, Settings.TileSize);

            _heartbeatListener = new NPCHeartbeatListener();
        }

        public Tile(Vector position)
        {
            _descriptor = new TileDescriptor(position);
            _collisionArea = new Rect(position.X, position.Y, Settings.TileSize, Settings.TileSize);
            _heartbeatListener = new NPCHeartbeatListener();
        }

        public void Update(GameTime gameTime)
        {
            if (this.Descriptor.Attribute == TileAttributes.NPCSpawn)
            {
                var attributeData = ((NPCSpawnAttributeData)this.Descriptor.AttributeData);

                if (_nextNPCSpawnTime <= gameTime.TotalElapsedTime && _heartbeatListener.NPCs.Count <= attributeData.MaxSpawns)
                {
                    this.NPCSpawnerEvent?.Invoke(this, new NPCSpawnerEventArgs(attributeData.NPCID, attributeData.MaxSpawns, this.Descriptor.Position, _heartbeatListener));

                    _nextNPCSpawnTime = gameTime.TotalElapsedTime + ((NPCSpawnAttributeData)this.Descriptor.AttributeData).RespawnTime * 1000;
                }
            }
        }

        public bool CheckCollision(Vector position, Rect collisionBounds)
        {
            Rect collisionArea = new Rect((int)(position.X + collisionBounds.Left), (int)(position.Y + collisionBounds.Top),
                collisionBounds.Width, collisionBounds.Height);

            return (_collisionArea.Intersects(collisionArea));
        }

        public void OnPlayerEntered(Player player)
        {
            switch (this.Descriptor.Attribute)
            {
                case TileAttributes.Warp:
                    WarpAttributeData attributeData = (WarpAttributeData)this.Descriptor.AttributeData;
                    if (player.MapID != attributeData.WarpMap)
                    {
                        var map  = Server.ServiceLocator.Get<WorldManager>().GetMap(attributeData.WarpMap);

                        if (map != null)
                        {
                            player.JoinMap(map);

                            var newLayer = map.Layers.FirstOrDefault(l => l.Descriptor.Name == attributeData.LayerName);

                            if (newLayer != null)
                            {
                                player.Layer = newLayer;
                            }
                        }
                        else
                        {

                            Logger.LogEvent($"Player {player.Descriptor.Name} stepped on warp tile where destination does not exist!", LogTypes.ERROR, Environment.StackTrace);
                            return;
                        }
                       
                    }
                    player.WarpTo(new Vector(attributeData.X, attributeData.Y));
                    break;
            }
        }

        public void OnPlayerLeft(Player player)
        {
            
        }

        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            // Tell the client whether it's a blank tile
            netBuffer.Write(this.Descriptor.SpriteInfo == null);

            // Is this a blank tile (determined based on the existence of a Sprite)
            if (this.Descriptor.SpriteInfo != null)
            {
                netBuffer.Write(this.Descriptor.LightSource);
                netBuffer.Write(this.Descriptor.LightRadius);
                netBuffer.Write(this.Descriptor.LightColor);
                netBuffer.Write(this.Descriptor.Teleporter);
                netBuffer.Write(this.Descriptor.SpriteInfo.TextureName);
                netBuffer.Write(this.Descriptor.SpriteInfo.Transform.Color);
                netBuffer.Write(this.Descriptor.SpriteInfo.Transform.Rect);
                netBuffer.Write(this.Descriptor.SpriteInfo.Transform.Position);
                netBuffer.Write(this.Descriptor.Animated);
                netBuffer.Write(this.Descriptor.FrameCount);
            }

            return netBuffer;
        }

        public void Load(BinaryReader bR, Vector tilePosition)
        {
           
        }

        public event EventHandler<NPCSpawnerEventArgs> NPCSpawnerEvent;

        /// <summary>
        /// Keeps track of the npcs which have been spawned as a result of this tile.
        /// </summary>
        public class NPCHeartbeatListener
        {
            public ObservableCollection<NPC> NPCs { get; }

            public NPCHeartbeatListener()
            {
                this.NPCs = new ObservableCollection<NPC>();

                this.NPCs.CollectionChanged += (sender, args) =>
                {
                    List<NPC> npcsToRemove = new List<NPC>(this.NPCs.Count);

                    foreach (NPC npc in args.NewItems)
                    {
                        npc.Died += (o, eventArgs) =>
                        {
                            npcsToRemove.Add(npc);
                        };
                    }

                    foreach (var npc in npcsToRemove)
                        this.NPCs.Remove(npc);
                };
            }
        }

        public class NPCSpawnerEventArgs : EventArgs
        {
            public string Name { get; }

            public int Count { get; }

            public Vector Position { get; }

            public NPCHeartbeatListener HeartbeatListener { get; }

            public NPCSpawnerEventArgs(string name, int count, Vector position, NPCHeartbeatListener heartBeatListener)
            {
                this.Name = name;
                this.Count = count;
                this.Position = position;

                this.HeartbeatListener = heartBeatListener;
            }
        }
    }
}