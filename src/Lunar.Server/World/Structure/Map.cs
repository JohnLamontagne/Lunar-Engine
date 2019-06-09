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
using Lidgren.Network;
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Pathfinding;
using Lunar.Server.World.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Core.World.Structure;
using Lunar.Core.Utilities;

namespace Lunar.Server.World.Structure
{
    public class Map
    {
        private MapDescriptor _mapDescriptor;

        public MapDescriptor Descriptor => _mapDescriptor;

        private readonly Dictionary<string, Layer> _layers;
        private readonly Dictionary<Layer, Pathfinder> _pathFinders;
        private WorldDictionary<long, IActor<IActorDescriptor>> _actors;
        private WorldDictionary<IActor<IActorDescriptor>, List<MapObject>> _actorCollidingObjects;

        private List<Tuple<Vector, Layer>> _playerSpawnAreas;
        private List<MapItem> _mapItems;
        private List<string> _tilesetPaths;

        public IEnumerable<Layer> Layers => _layers.Values;

        public List<Player> Players => this.GetActors<Player>().ToList();

        public Map(MapDescriptor descriptor)
        {
            _mapDescriptor = descriptor;

            _layers = new Dictionary<string, Layer>();
            _actors = new WorldDictionary<long, IActor<IActorDescriptor>>();
            _actorCollidingObjects = new WorldDictionary<IActor<IActorDescriptor>, List<MapObject>>();
            _playerSpawnAreas = new List<Tuple<Vector, Layer>>();
            _pathFinders = new Dictionary<Layer, Pathfinder>();
            _mapItems = new List<MapItem>();

            this.Initalize();
        }

        private void Initalize()
        {
            foreach (var layerDesc in this.Descriptor.Layers.Values)
            {
                Layer layer = new Layer(layerDesc);

                layer.NPCSpawnerEvent += (sender, args) =>
                {
                    var npcDesc = Server.ServiceLocator.Get<NPCManager>().GetNPC(args.NPCID);

                    if (npcDesc == null)
                    {
                        Logger.LogEvent($"Error spawning NPC: {args.NPCID} does not exist!", LogTypes.ERROR, new Exception($"Error spawning NPC: {args.NPCID} does not exist!"));
                        return;
                    }

                    NPC npc = new NPC(npcDesc, this)
                    {
                        Layer = (Layer)sender
                    };
                    npc.WarpTo(args.Position);

                    // This allows the tile spawner to keep track of npcs that exist, and respawn if neccessary (i.e., they die).
                    args.HeartbeatListener.NPCs.Add(npc);
                };

                _layers.Add(layerDesc.Name, layer);
            }

            // Look for spawnpoints
            foreach (var layer in this.Layers)
            {
                for (int x = 0; x < this.Descriptor.Dimensions.X; x++)
                {
                    for (int y = 0; y < this.Descriptor.Dimensions.Y; y++)
                    {
                        if (layer.GetTile(x, y) != null && layer.GetTile(x, y).Descriptor.Attribute == TileAttributes.PlayerSpawn)
                        {
                            this.AddPlayerStartArea(new Vector(x * Settings.TileSize, y * Settings.TileSize), layer);
                        }
                    }
                }
            }
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
            packet.Message.Write(mapItem.Layer.Descriptor.Name);
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
                Logger.LogEvent($"Specified item does not exist on map; cannot remove: {item.Descriptor.Name}", LogTypes.ERROR, new Exception($"Specified item does not exist on map; cannot remove: {item.Descriptor.Name}"));
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

        public bool ActorInMap<T>(T actor) where T : IActor<IActorDescriptor>
        {
            return _actors.ContainsKey(actor.UniqueID);
        }

        public virtual void AddActor<T>(IActor<T> actor) where T : class, IActorDescriptor
        {
            _actors.Add(actor.UniqueID, actor);
            _actorCollidingObjects.Add(actor, new List<MapObject>());
        }

        public IActor<IActorDescriptor> GetActor(long actorID)
        {
            if (_actors.ContainsKey(actorID))
                return _actors[actorID];
            else
                return null;
        }

        public virtual IEnumerable<T> GetActors<T>() where T : IActor<IActorDescriptor>
        {
            return from actor in _actors
                   where actor is T
                   select (T)actor;
        }

        public void SendAnimation()
        {

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
            player.NetworkComponent.SendPacket(mapDataPacket, NetDeliveryMethod.ReliableOrdered);

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

                player.NetworkComponent.SendPacket(playerDataPacket, NetDeliveryMethod.ReliableOrdered);
            }

            // Send all npcs to the player
            foreach (var npc in this.GetActors<NPC>())
            {
                var npcDataPacket = new Packet(PacketType.NPC_DATA, ChannelType.UNASSIGNED);
                npcDataPacket.Message.Write(npc.Pack());
                
                player.NetworkComponent.SendPacket(npcDataPacket, NetDeliveryMethod.ReliableOrdered);
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
                Logger.LogEvent($"Actor {actorID} does not exist in map!", LogTypes.ERROR, new Exception($"Actor {actorID} does not exist in map!"));
                return;;
            }

            _actorCollidingObjects.Remove(_actors[actorID]);
            _actors.Remove(actorID);
        }

        public void SendChatMessage(string message, ChatMessageType messageType)
        {
            foreach (Player player in this.GetActors<Player>())
            {
                player.NetworkComponent.SendChatMessage(message, messageType);
            }
        }

        public void SendPacket(Packet packet, NetDeliveryMethod method)
        {
            foreach (var player in this.GetActors<Player>())
            {
                player.NetworkComponent.SendPacket(packet, method);
                packet.Reset();
            }
        }

        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.Descriptor.Name);
            netBuffer.Write(this.Descriptor.Dimensions);
            netBuffer.Write(this.Descriptor.Dark);

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
    }
}