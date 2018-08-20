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
using Lidgren.Network;
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Pathfinding;
using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World;
using Lunar.Core.World.Structure;

namespace Lunar.Server.World.Structure
{
    public class Map
    {
        private readonly Dictionary<string, Layer> _layers;
        private readonly Dictionary<Layer, Pathfinder> _pathFinders;
        private WorldDictionary<long, IActor> _actors;
        private WorldDictionary<IActor, List<MapObject>> _actorCollidingObjects;

        private List<Tuple<Vector, Layer>> _playerSpawnAreas;
        private List<MapItem> _mapItems;
        private List<string> _tilesetPaths;

        public string Name { get; set; }

        public Vector Dimensions { get; private set; }

        public Rect Bounds { get; set; }

        public bool Dark { get; set; }

        public IEnumerable<Layer> Layers => _layers.Values;

        public List<Player> Players => this.GetActors<Player>().ToList();

        public Map(Vector dimensions, string name) 
            : this()
        {
            this.Dimensions = dimensions;
            this.Name = name;

            this.Bounds = new Rect(0, 0, this.Dimensions.X, this.Dimensions.Y);
        }

        private Map()
        {
            _layers = new Dictionary<string, Layer>();
            _actors = new WorldDictionary<long, IActor>();
            _actorCollidingObjects = new WorldDictionary<IActor, List<MapObject>>();
            _playerSpawnAreas = new List<Tuple<Vector, Layer>>();
            _pathFinders = new Dictionary<Layer, Pathfinder>();
            _mapItems = new List<MapItem>();
        }

        public void AddLayer(string name, Layer layer)
        {
            _layers.Add(name, layer);
        }

        public IEnumerable<MapItem> GetMapItems()
        {
            return _mapItems;
        }

        public void SpawnItem(Item item, Vector position, Layer layer)
        {
            foreach (var mapItem in _mapItems)
            {
                if (mapItem.Item == item && mapItem.Position == position && mapItem.Layer == layer)
                {
                    mapItem.Amount++;
                    this.SendMapItem(mapItem);
                    return;
                }
            }

            var mItem = new MapItem(item, 1)
            {
                Position = position,
                Layer = layer
            };
            _mapItems.Add(mItem);
            this.SendMapItem(mItem);
        }

        private void SendMapItem(MapItem mapItem)
        {
            var packet = new Packet(PacketType.MAP_ITEM_SPAWN, ChannelType.UNASSIGNED);
            packet.Message.Write(mapItem.Position);
            packet.Message.Write(mapItem.Layer.Name);
            packet.Message.Write(mapItem.Item.PackData());
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void RemoveItem(Item item)
        {
            var mapItem = _mapItems.FirstOrDefault(mItem => mItem.Item == item);

            if (mapItem != null)
            {
                var packet = new Packet(PacketType.MAP_ITEM_DESPAWN, ChannelType.UNASSIGNED);
                packet.Message.Write(mapItem.Position);
                packet.Message.Write(mapItem.Item.PackData());
                this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);

                _mapItems.Remove(mapItem);
            }
            else
            {
                Logger.LogEvent($"Specified item does not exist on map; cannot remove: {item.Name}", LogTypes.ERROR, Environment.StackTrace);
            }

          
        }

        public void AddPlayerStartArea(Vector playerStartArea, Layer layer)
        {
            _playerSpawnAreas.Add(new Tuple<Vector, Layer>(playerStartArea, layer));
        }

        public void ConstructPathfinder()
        {   
            _pathFinders.Clear();

            foreach (var layer in this.Layers)
            {
                _pathFinders.Add(layer, new Pathfinder(this, layer));
            }

        }

        public Pathfinder GetPathfinder(Layer layer)
        {
            return _pathFinders[layer];
        }

        public virtual void Update(GameTime gameTime)
        {
            foreach (var t in this.Layers)
            {
                t?.Update(gameTime);
            }

            foreach (var actor in _actors)
            {
                actor.Update(gameTime);
                
                if (_actorCollidingObjects.ContainsKey(actor))
                {
                    for (int x = _actorCollidingObjects[actor].Count - 1; x >= 0; x--)
                    {
                        if (!_actorCollidingObjects[actor][x].CollisionDescriptor.Collides(actor))
                        {
                            _actorCollidingObjects[actor][x].OnLeft(actor);
                            _actorCollidingObjects[actor].RemoveAt(x);
                        }
                    }
                }


            }

        }

        public Layer GetLayer(string name)
        {
            return _layers[name];
        }

        public bool ActorInMap(IActor actor)
        {
            return _actors.ContainsKey(actor.UniqueID);
        }

        public virtual void AddActor(IActor actor)
        {
            _actors.Add(actor.UniqueID, actor);
            _actorCollidingObjects.Add(actor, new List<MapObject>());
        }

        public IActor GetActor(long actorID)
        {
            if (_actors.ContainsKey(actorID))
                return _actors[actorID];
            else
                return null;
        }

        public virtual IEnumerable<T> GetActors<T>() where T : IActor
        {
            return from actor in _actors
                   where actor is T
                   select (T)actor;
        }




        public void OnPlayerQuit(Player player)
        {
            var packet = new Packet(PacketType.PLAYER_LEFT, ChannelType.UNASSIGNED);
            packet.Message.Write(player.UniqueID);
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);

            // Remove the player.
            this.RemoveActor(player.UniqueID);
        }

        public void OnPlayerJoined(Player player)
        {
            // Send map data packet to player.
            var mapDataPacket = new Packet(PacketType.MAP_DATA, ChannelType.UNASSIGNED);
            mapDataPacket.Message.Write(this.PackData());
            player.SendPacket(mapDataPacket, NetDeliveryMethod.ReliableOrdered);

            // Send the joining player to the current map players.
            var joiningPlayerDataPacket = new Packet(PacketType.PLAYER_JOINED, ChannelType.UNASSIGNED);
            joiningPlayerDataPacket.Message.Write(player.Pack());
            this.SendPacket(joiningPlayerDataPacket, NetDeliveryMethod.ReliableOrdered);

            // Add player to the map
            this.AddActor(player);

            // Send all map players to player.
            foreach (var p in this.GetActors<Player>())
            {
                var playerDataPacket = new Packet(PacketType.PLAYER_JOINED, ChannelType.UNASSIGNED);
                playerDataPacket.Message.Write(p.Pack());

                player.SendPacket(playerDataPacket, NetDeliveryMethod.ReliableOrdered);
            }

            // Send all npcs to the player
            foreach (var npc in this.GetActors<NPC>())
            {
                var npcDataPacket = new Packet(PacketType.NPC_DATA, ChannelType.UNASSIGNED);
                npcDataPacket.Message.Write(npc.Pack());
                
                player.SendPacket(npcDataPacket, NetDeliveryMethod.ReliableOrdered);
            }

            // Select random starting location
            if (_playerSpawnAreas.Count > 0)
            {
                Random random = new Random();
                int spawnIndex = (int) (random.NextDouble() * _playerSpawnAreas.Count);
                player.Layer = _playerSpawnAreas[spawnIndex].Item2;
                player.WarpTo((Vector) _playerSpawnAreas[spawnIndex].Item1);
            }
            else
            {
                player.Layer = this.Layers.ElementAt(0);
            }


        }

        public virtual void RemoveActor(long actorID)
        {
            if (!_actors.ContainsKey(actorID))
            {
                Logger.LogEvent($"Actor {actorID} does not exist in map!", LogTypes.ERROR, Environment.StackTrace);
                return;;
            }

            _actorCollidingObjects.Remove(_actors[actorID]);
            _actors.Remove(actorID);
        }

        public void SendChatMessage(string message, ChatMessageType messageType)
        {
            foreach (Player player in this.GetActors<Player>())
            {
                player.SendChatMessage(message, messageType);
            }
        }

        public void SendPacket(Packet packet, NetDeliveryMethod method)
        {
            foreach (var player in this.GetActors<Player>())
            {
                player.SendPacket(packet, method);
                packet.Reset();
            }
        }

        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.Name);
            netBuffer.Write(this.Dimensions);
            netBuffer.Write(this.Dark);

            netBuffer.Write(_layers.Count);
            foreach (var layer in this.Layers)
            {
                netBuffer.Write(layer.PackData());
            }

            return netBuffer;
        }

        public void Unload()
        {
            
        }

        public static Map Load(string path)
        {
            var map = new Map();

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var bR = new BinaryReader(fileStream))
                {
                    // Load the tileset information
                    int tilesetCount = bR.ReadInt32();
                    for (int i = 0; i < tilesetCount; i++)
                    {
                        // We can throw this information away as it is used only in the editor suite.
                        string tilesetPath = bR.ReadString();
                    }

                    map.Name = bR.ReadString();
                    map.Dimensions = new Vector(bR.ReadInt32(), bR.ReadInt32());
                    map.Dark = bR.ReadBoolean();

                    map.Bounds = new Rect(0, 0, (int)map.Dimensions.X, (int)map.Dimensions.Y);

                    int layerCount = bR.ReadInt32();
                    for (int i = 0; i < layerCount; i++)
                    {
                        string layerName = bR.ReadString();
                        int lIndex = bR.ReadInt32();

                        var layer = new Layer(map.Dimensions, layerName, lIndex);
                        layer.Load(bR);
                        layer.NPCSpawnerEvent += (sender, args) =>
                        {
                            var npcDesc = Server.ServiceLocator.GetService<NPCManager>().GetNPC(args.Name);
                            NPC npc = new NPC(npcDesc, map)
                            {
                                Layer = (Layer) sender
                            };
                            npc.WarpTo(args.Position);

                            // This allows the tile spawner to keep track of npcs that exist, and respawn if neccessary (i.e., they die).
                            args.HeartbeatListener.NPCs.Add(npc);
                        };

                        map.AddLayer(layerName, layer);
                    }
                }
            }

            // Look for spawnpoints
            foreach (var layer in map.Layers)
            {
                for (int x = 0; x < map.Dimensions.X; x++)
                {
                    for (int y = 0; y < map.Dimensions.Y; y++)
                    {
                        if (layer.GetTile(x, y) != null && layer.GetTile(x, y).Attribute == TileAttributes.PlayerSpawn)
                        {
                            map.AddPlayerStartArea(new Vector(x * Settings.TileSize, y * Settings.TileSize), layer);
                        }
                    }
                }
            }

            return map;
        }
    }
}