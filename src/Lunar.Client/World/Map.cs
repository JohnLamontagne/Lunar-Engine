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

using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Client.Utilities.Services;
using Lunar.Client.World.Actors;
using Lunar.Core.Net;
using Lunar.Graphics;
using Lunar.Core;

namespace Lunar.Client.World
{
    public class Map
    {
        private readonly Dictionary<string, Layer> _layers;
        private readonly Dictionary<string, IActor> _entities;
        private readonly Dictionary<Vector2, List<MapItem>> _mapItems;
        private readonly List<MapObject> _mapObjects;

        public string Name { get; private set; }

        public Vector2 Dimensions { get; private set; }

        public Rectangle Bounds { get; private set; }

        public bool Dark { get; private set; }

        public IEnumerable<Layer> Layers => _layers.Values;

        public List<Vector2> Path { get; set; }

        public Map(Vector2 dimensions, string name)
        {
            this.Dimensions = dimensions;
            this.Name = name;

            this.Bounds = new Rectangle(0, 0, (int)this.Dimensions.X, (int)this.Dimensions.Y);

            _layers = new Dictionary<string, Layer>();
            _entities = new Dictionary<string, IActor>();
            _mapItems = new Dictionary<Vector2, List<MapItem>>();
            _mapObjects = new List<MapObject>();

            Engine.Services.Get<NetHandler>().AddPacketHandler(PacketType.MAP_ITEM_SPAWN, this.Handle_MapItemSpawn);
            Engine.Services.Get<NetHandler>().AddPacketHandler(PacketType.MAP_ITEM_DESPAWN, this.Handle_MapItemDeSpawn);
        }

        private void Handle_MapItemDeSpawn(PacketReceivedEventArgs args)
        {
        }

        private void Handle_MapItemSpawn(PacketReceivedEventArgs args)
        {
            MapItem mapItem = new MapItem();
            string layerName = args.Message.ReadString();

            mapItem.Unpack(args.Message, this.GetLayer(layerName));

            if (!_mapItems.ContainsKey(mapItem.Position))
                _mapItems.Add(mapItem.Position, new List<MapItem>());

            _mapItems[mapItem.Position].Add(mapItem);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var layer in this.Layers)
                layer.Update(gameTime);

            foreach (var entity in _entities.Values)
                entity.Update(gameTime);

            foreach (var mapObject in _mapObjects)
                mapObject.Update(gameTime);
        }

        public bool CheckCollision(IActor entity)
        {
            bool collidesWithTile = entity.Layer.CheckCollision(entity.Position, entity.CollisionBounds);
            bool collidesWithEntity = false;

            foreach (var ent in _entities.Values)
            {
                if (ent == entity)
                    continue;

                if (ent.Position == entity.Position)
                {
                    collidesWithEntity = true;
                    break;
                }
            }

            return (collidesWithTile || collidesWithEntity);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var layer in this.Layers)
            {
                layer.Draw(spriteBatch, camera);
            }

            foreach (var entity in _entities.Values.OrderBy(entity => entity.Position.Y).Reverse())
                entity.Draw(spriteBatch);

            foreach (var mapObject in _mapObjects)
                mapObject.Draw(spriteBatch);

            foreach (var mapItemPos in _mapItems.Values)
            {
                for (int i = 0; i < mapItemPos.Count; i++)
                {
                    mapItemPos[i].Draw(spriteBatch);
                }
            }
        }

        public Layer GetLayer(string name)
        {
            return _layers[name];
        }

        public void Unpack(NetBuffer netBuffer)
        {
            Engine.Services.Get<LightManagerService>().Component.Lights.Clear();

            this.Dark = netBuffer.ReadBoolean();

            if (this.Dark)
            {
                Engine.Services.Get<LightManagerService>().Component.AmbientColor = new Color(100, 100, 100, 20);
            }
            else
            {
                Engine.Services.Get<LightManagerService>().Component.AmbientColor = Color.White;
            }

            int layerCount = netBuffer.ReadInt32();

            for (int i = 0; i < layerCount; i++)
            {
                string layerName = netBuffer.ReadString();
                int lIndex = netBuffer.ReadInt32();
                var layer = new Layer(this.Dimensions, lIndex, layerName);
                layer.Unpack(netBuffer);
                _layers.Add(layerName, layer);
            }
        }

        public void Unload()
        {
            _entities.Clear();
        }

        public IActor GetEntity(string uniqueID)
        {
            return _entities[uniqueID];
        }

        public T GetEntity<T>(string uniqueID) where T : IActor
        {
            var value = _entities[uniqueID];

            if (value.GetType() == typeof(T))
            {
                return (T)value;
            }

            return default(T);
        }

        public IEnumerable<IActor> GetEntities()
        {
            return _entities.Values;
        }

        public bool EntityExists(string uniqueID)
        {
            return _entities.ContainsKey(uniqueID);
        }

        public void AddEntity(string uniqueID, IActor actor)
        {
            _entities.Add(uniqueID, actor);

            Engine.Services.Get<LightManagerService>().Component.Lights.Add(actor.Light);
        }

        public void RemoveEntity(string uniqueID)
        {
            Engine.Services.Get<LightManagerService>().Component.Lights.Remove(_entities[uniqueID].Light);

            _entities.Remove(uniqueID);
        }
    }
}