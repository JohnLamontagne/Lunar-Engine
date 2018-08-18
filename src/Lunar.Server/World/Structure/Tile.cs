/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

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
using Lunar.Server.Content.Graphics;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class Tile
    {
        private Sprite _sprite;
        private Rect _collisionArea;

        
        private NPCHeartbeatListener _heartbeatListener;

        private int _nextNPCSpawnTime;

        public Vector Position { get; }

        public bool Animated { get; set; }

        public int FrameCount { get; set; }

        public bool LightSource { get; set; }

        public int LightRadius { get; set; }

        public Color LightColor { get; set; }

        public bool Teleporter { get; set; }

        public bool Blocked { get; set; }

        public TileAttributes Attribute { get; private set; }

        public AttributeData AttributeData { get; private set; }

        public Tile(Sprite sprite)
        {
            _sprite = sprite;

            _collisionArea = new Rect(sprite.Transform.Position.X, sprite.Transform.Position.Y, Settings.TileSize, Settings.TileSize);

            this.Animated = false;
            //this.LightColor = Color.White;
        }

        public Tile(Vector position)
        {
            this.Position = position;
            _collisionArea = new Rect(position.X, position.Y, Settings.TileSize, Settings.TileSize);
            _heartbeatListener = new NPCHeartbeatListener();
        }

        public void Update(GameTime gameTime)
        {
            if (this.Attribute == TileAttributes.NPCSpawn)
            {
                var attributeData = ((NPCSpawnAttributeData)this.AttributeData);

                if (_nextNPCSpawnTime <= gameTime.TotalElapsedTime && _heartbeatListener.NPCs.Count <= attributeData.MaxSpawns)
                {
                    this.NPCSpawnerEvent?.Invoke(this, new NPCSpawnerEventArgs(attributeData.NPCID, attributeData.MaxSpawns, this.Position, _heartbeatListener));
                    _nextNPCSpawnTime = ((NPCSpawnAttributeData)this.AttributeData).RespawnTime * 1000;
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
            switch (this.Attribute)
            {
                case TileAttributes.Warp:
                    WarpAttributeData attributeData = (WarpAttributeData) this.AttributeData;
                    if (player.MapID != attributeData.WarpMap)
                    {
                        var map  = Server.ServiceLocator.GetService<WorldManager>().GetMap(attributeData.WarpMap);

                        if (map != null)
                        {
                            player.JoinMap(map);

                            var newLayer = map.Layers.FirstOrDefault(l => l.Name == attributeData.LayerName);

                            if (newLayer != null)
                            {
                                player.Layer = newLayer;
                            }
                        }
                        else
                        {

                            Logger.LogEvent($"Player {player.Name} stepped on warp tile where destination does not exist!", LogTypes.ERROR, Environment.StackTrace);
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
            netBuffer.Write(_sprite == null);

            // Is this a blank tile (determined based on the existence of a Sprite)
            if (_sprite != null)
            {
                netBuffer.Write(this.Animated);
                netBuffer.Write(this.LightSource);
                netBuffer.Write(this.LightRadius);
                netBuffer.Write(this.LightColor);
                netBuffer.Write(this.Teleporter);
                netBuffer.Write(_sprite.TextureName);
                netBuffer.Write(_sprite.Transform.Color);
                netBuffer.Write(_sprite.Transform.Rect);
                netBuffer.Write(_sprite.Transform.Position);
                netBuffer.Write(this.FrameCount);
            }

            return netBuffer;
        }

        public void Load(BinaryReader bR, Vector tilePosition)
        {
            this.Attribute = (TileAttributes)bR.ReadByte();

            int attributeDataLength = bR.ReadInt32();
            byte[] attributeData = bR.ReadBytes(attributeDataLength);
            this.AttributeData = AttributeData.Deserialize(attributeData);

            if (bR.ReadBoolean())
            {
                this.Animated = bR.ReadBoolean();
                this.LightSource = bR.ReadBoolean();

                string spriteName = bR.ReadString();
                float zIndex = bR.ReadSingle(); // We can throw this away

                _sprite = new Sprite(spriteName)
                {
                    Transform =
                    {
                        Position = tilePosition,
                        Color = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte()),
                        Rect = new Rect(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32())
                    }
                };

                this.FrameCount = bR.ReadInt32();
            }
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