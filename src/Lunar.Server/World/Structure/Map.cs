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
using Lunar.Core.World.Structure;
using Lunar.Core.Utilities;
using Lunar.Core.World.Structure.Attribute;
using Lunar.Core.Content.Graphics;

namespace Lunar.Server.World.Structure
{
    public class Map : MapModel<Layer>
    {
        private readonly Dictionary<Layer, Pathfinder> _pathFinders;
        private WorldDictionary<string, IActor> _actors;
        private WorldDictionary<IActor, List<MapObject>> _actorCollidingObjects;

        private List<Tuple<Vector, Layer>> _playerSpawnAreas;
        private List<MapItem> _mapItems;

        public List<Player> Players => this.GetActors<Player>().ToList();

        public Map(MapModel<LayerModel<TileModel<SpriteInfo>>> descriptor)
        {
            _actors = new WorldDictionary<string, IActor>();
            _actorCollidingObjects = new WorldDictionary<IActor, List<MapObject>>();
            _playerSpawnAreas = new List<Tuple<Vector, Layer>>();
            _pathFinders = new Dictionary<Layer, Pathfinder>();
            _mapItems = new List<MapItem>();

            this.Name = descriptor.Name;
            this.Bounds = descriptor.Bounds;
            this.Dimensions = descriptor.Dimensions;
            this.Dark = descriptor.Dark;
            this.TilesetPaths = descriptor.TilesetPaths;

            foreach (var layerDesc in descriptor.Layers)
            {
                Layer layer = new Layer(this, layerDesc);

                this.AddLayer(layerDesc.Name, layer);
            }

            // Look for spawnpoints
            foreach (var layer in this.Layers)
            {
                for (int x = 0; x < descriptor.Dimensions.X; x++)
                {
                    for (int y = 0; y < descriptor.Dimensions.Y; y++)
                    {
                        if (layer.GetTile(x, y) != null && layer.GetTile(x, y).Attribute is PlayerSpawnTileAttribute)
                        {
                            this.AddPlayerStartArea(new Vector(x * Settings.TileSize, y * Settings.TileSize), layer);
                        }
                    }
                }
            }
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
            var packet = new Packet();
            packet.Write(mapItem.Position);
            packet.Write(mapItem.Layer.Name);
            packet.Write(mapItem.Item.PackData());
            this.SendPacket(PacketType.MAP_ITEM_SPAWN, packet, DeliveryMethod.ReliableOrdered);
        }

        public void RemoveItem(Item item)
        {
            var mapItem = _mapItems.FirstOrDefault(mItem => mItem.Item == item);

            if (mapItem != null)
            {
                var packet = new Packet();
                packet.Write(mapItem.Position);
                packet.Write(mapItem.Item.PackData());
                this.SendPacket(PacketType.MAP_ITEM_DESPAWN, packet, DeliveryMethod.ReliableOrdered);

                _mapItems.Remove(mapItem);
            }
            else
            {
                Engine.Services.Get<Logger>().LogEvent($"Specified item does not exist on map; cannot remove: {item.Descriptor.Name}", LogTypes.ERROR, new Exception($"Specified item does not exist on map; cannot remove: {item.Descriptor.Name}"));
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

        public bool ActorInMap<T>(T actor) where T : IActor
        {
            return _actors.ContainsKey(actor.UniqueID);
        }

        public virtual void AddActor<T>(T actor) where T : class, IActor
        {
            _actors.Add(actor.UniqueID, actor);
            _actorCollidingObjects.Add(actor, new List<MapObject>());
        }

        public IActor GetActor(string actorID)
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

        public void SendAnimation()
        {
        }

        public void OnPlayerQuit(Player player)
        {
            var packet = new Packet();
            packet.Write(player.UniqueID);
            this.SendPacket(PacketType.PLAYER_LEFT, packet, DeliveryMethod.ReliableOrdered);

            // Remove the player.
            this.RemoveActor(player.UniqueID);
        }

        public void OnPlayerJoined(Player player)
        {
            // Send map data packet to player.
            var mapDataPacket = new Packet();
            mapDataPacket.Write(this.PackData());
            player.NetworkComponent.SendPacket(PacketType.MAP_DATA, mapDataPacket, DeliveryMethod.ReliableOrdered);

            // Send the joining player to the current map players.
            var joiningPlayerDataPacket = new Packet();
            joiningPlayerDataPacket.Write(player.Pack());
            this.SendPacket(PacketType.PLAYER_JOINED, joiningPlayerDataPacket, DeliveryMethod.ReliableOrdered);

            // Add player to the map
            this.AddActor(player);

            // Send all map players to player.
            foreach (var p in this.GetActors<Player>())
            {
                var playerDataPacket = new Packet();
                playerDataPacket.Write(p.Pack());

                player.NetworkComponent.SendPacket(PacketType.PLAYER_JOINED, playerDataPacket, DeliveryMethod.ReliableOrdered);
            }

            // Send all npcs to the player
            foreach (var npc in this.GetActors<NPC>())
            {
                var npcDataPacket = new Packet();
                npcDataPacket.Write(npc.Pack());

                player.NetworkComponent.SendPacket(PacketType.NPC_DATA, npcDataPacket, DeliveryMethod.ReliableOrdered);
            }

            // Select random starting location
            if (_playerSpawnAreas.Count > 0)
            {
                Random random = new Random();
                int spawnIndex = (int)(random.NextDouble() * _playerSpawnAreas.Count);
                player.Layer = _playerSpawnAreas[spawnIndex].Item2;
                player.WarpTo((Vector)_playerSpawnAreas[spawnIndex].Item1);
            }
            else
            {
                player.Layer = this.Layers.ElementAt(0);
            }
        }

        public virtual void RemoveActor(string actorID)
        {
            if (!_actors.ContainsKey(actorID))
            {
                Engine.Services.Get<Logger>().LogEvent($"Actor {actorID} does not exist in map!", LogTypes.ERROR, new Exception($"Actor {actorID} does not exist in map!"));
                return; ;
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

        public void SendPacket(PacketType packetType, Packet packet, DeliveryMethod deliveryMethod)
        {
            foreach (var player in this.GetActors<Player>())
            {
                player.NetworkComponent.SendPacket(packetType, packet, deliveryMethod);
            }
        }

        public Packet PackData()
        {
            var packet = new Packet();

            packet.Write(this.Name);
            packet.Write(this.Dimensions);
            packet.Write(this.Dark);

            packet.Write(this.Layers.Count);
            foreach (var layer in this.Layers)
            {
                packet.Write(layer.PackData());
            }

            return packet;
        }

        public void Unload()
        {
        }
    }
}