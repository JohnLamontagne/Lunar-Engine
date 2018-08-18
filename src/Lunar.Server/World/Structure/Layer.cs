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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World.Structure;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class Layer
    {
        private readonly Tile[,] _tiles;
        
        public string Name { get; private set; }

        private Dictionary<Vector, CollisionDescriptor> _collisionDescriptors;
        private Dictionary<Player, List<Tile>> _playerCollidingTiles;
        private List<MapObject> _mapObjects;

        public Dictionary<Vector, CollisionDescriptor> CollisionDescriptors { get { return _collisionDescriptors; } }

        public int LayerIndex { get; set; }

        public Layer(Vector dimensions, string layerName, int lIndex)
        {
            _tiles = new Tile[(int)dimensions.X, (int)dimensions.Y];

            this.Name = layerName;
            this.LayerIndex = lIndex;

            _playerCollidingTiles = new Dictionary<Player, List<Tile>>(); ;
            _collisionDescriptors = new Dictionary<Vector, CollisionDescriptor>();
            _mapObjects = new List<MapObject>();
        }

        public CollisionDescriptor GetCollisionDescriptor(Vector position)
        {
            if (_collisionDescriptors.Keys.Contains(position))
                return _collisionDescriptors[position];
            else
                return null;
        }

        public CollisionDescriptor GetCollisionDescriptor(int x, int y)
        {
            return this.GetCollisionDescriptor(new Vector(x, y));
        }


        public void RemoveCollisionDescriptor(Vector position)
        {
            _collisionDescriptors.Remove(position);
        }

        public void RemoveCollisionDescriptor(int x, int y)
        {
            this.RemoveCollisionDescriptor(new Vector(x, y));
        }

        public void AddCollisionDescriptor(Vector position, CollisionDescriptor descriptor)
        {
            _collisionDescriptors.Add(position, descriptor);
        }


        public List<MapObject> GetCollidingMapObjects(Vector position, Rect collisionBounds)
        {
            List<MapObject> collidedMapObjects = new List<MapObject>();
            Rect collisionArea = new Rect(position.X + collisionBounds.Left, position.Y + collisionBounds.Top, collisionBounds.Width, collisionBounds.Height);

            foreach (var mapObject in _mapObjects)
            {
                if (mapObject.CollisionDescriptor.Collides(collisionArea))
                {
                    collidedMapObjects.Add(mapObject);
                }
            }

            return collidedMapObjects;
        }

        public void OnPlayerMoved(Player player)
        {
            if (!_playerCollidingTiles.ContainsKey(player))
                _playerCollidingTiles.Add(player, new List<Tile>());

            var collidingTiles = this.GetCollidingTiles(player);
            foreach (var collidingTile in collidingTiles)
            {
                if (collidingTile != null && !_playerCollidingTiles[player].Contains(collidingTile))
                {
                    _playerCollidingTiles[player].Add(collidingTile);
                    collidingTile.OnPlayerEntered(player);
                }
            }
            
        }

        public void OnPlayerWarped(Player player)
        {
            // We handle player warping a little different from typical movement, 
            // as we don't want to trigger tile effects when a player warps to that tile

            if (!_playerCollidingTiles.ContainsKey(player))
                _playerCollidingTiles.Add(player, new List<Tile>());

            var collidingTiles = this.GetCollidingTiles(player);
            foreach (var collidingTile in collidingTiles)
            {
                if (collidingTile != null && !_playerCollidingTiles[player].Contains(collidingTile))
                {
                    _playerCollidingTiles[player].Add(collidingTile);
                }
            }
        }

        public void AddMapObject(MapObject mapObject)
        {
            _mapObjects.Add(mapObject);
        }

        public void Update(GameTime gameTime)
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y] != null)
                    {
                        _tiles[x, y].Update(gameTime);
                    }
                }
            }

            foreach (var mapObject in _mapObjects)
                mapObject.Update(gameTime);

            foreach (var actorCollidingPair in _playerCollidingTiles)
            {
                for (int x = actorCollidingPair.Value.Count - 1; x >= 0; x--)
                {
                    if (!this.GetCollidingTiles(actorCollidingPair.Key).Contains(actorCollidingPair.Value[x]))
                    {
                        actorCollidingPair.Value[x].OnPlayerLeft(actorCollidingPair.Key);
                        actorCollidingPair.Value.RemoveAt(x);
                    }
                }
            }
        }

        public Tile[,] GetTiles()
        {
            return _tiles;
        }

        public Tile GetTile(int x, int y)
        {
            if (x >= _tiles.GetLength(0) || x < 0 || y >= _tiles.GetLength(1) || y < 0)
                return null;

            return _tiles[x, y];
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x >= _tiles.GetLength(0) || x < 0 || y >= _tiles.GetLength(1) || y < 0)
                throw new Exception("Fatal error: attempted to access an invalid tile!");

            _tiles[x, y] = tile;
        }

        public List<Tile> GetCollidingTiles(IActor actor)
        {
            return this.GetCollidingTiles(actor.Position, actor.CollisionBounds);
        }

        public List<Tile> GetCollidingTiles(Vector position, Rect collisionBounds)
        {
            List<Tile> collidingTiles = new List<Tile>();

            int leftCheck = (int)(position.X + collisionBounds.Left) / Settings.TileSize;
            int topCheck = (int)(position.Y + collisionBounds.Top) / Settings.TileSize;
            int tilesWidth = ((collisionBounds.Left + collisionBounds.Width) / Settings.TileSize) + 1;
            int tilesHeight = ((collisionBounds.Top + collisionBounds.Height) / Settings.TileSize) + 1;

            if (tilesWidth < 1)
                tilesWidth = 1;

            if (tilesHeight < 1)
                tilesHeight = 1;

            for (int x = 0; x < tilesWidth; x++)
            {
                if (this.GetTile(leftCheck + x, topCheck) != null && this.GetTile(leftCheck + x, topCheck).CheckCollision(position, collisionBounds))
                {
                    collidingTiles.Add(this.GetTile(leftCheck + x, topCheck));
                }
              
                for (int y = 0; y < tilesHeight; y++)
                {
                    if (this.GetTile(leftCheck + x, topCheck + y) != null && this.GetTile(leftCheck + x, topCheck + y).CheckCollision(position, collisionBounds))
                    {
                        collidingTiles.Add(this.GetTile(leftCheck + x, topCheck + y));
                    }
                }
            }

            return collidingTiles; // Returns the resultant list with null tiles removed
        }

        public bool CheckCollision(Vector position, Rect collisionBounds)
        {

            Rect collisionArea = new Rect((int)(position.X + collisionBounds.Left), (int)(position.Y + collisionBounds.Top),
                collisionBounds.Width, collisionBounds.Height);

            if (collisionArea.Left < 0 || collisionArea.Top < 0 ||
                collisionArea.Left + collisionArea.Width >= (_tiles.GetLength(0) * EngineConstants.TILE_WIDTH) || collisionArea.Top + collisionArea.Height >= (_tiles.GetLength(1) * EngineConstants.TILE_HEIGHT))
                return true;

            foreach (var collisionDescriptor in _collisionDescriptors.Values)
            {
                if (collisionDescriptor.Collides(collisionArea))
                {
                    return true;
                }
            }

            var collidingTiles = this.GetCollidingTiles(position, collisionBounds);

            foreach (var collidingTile in collidingTiles)
            {
                if (collidingTile.Attribute == TileAttributes.Blocked)
                    return true;

            }

            return false;
        }

        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.Name);
            netBuffer.Write(this.LayerIndex);

            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y] != null)
                    {
                        netBuffer.Write(true);

                        netBuffer.Write(_tiles[x, y].PackData());
                    }
                    else
                        netBuffer.Write(false);
                }
            }

            netBuffer.Write(_collisionDescriptors.Count);
            foreach (var collisionDescriptorPair in _collisionDescriptors)
            {
                netBuffer.Write(collisionDescriptorPair.Key.X);
                netBuffer.Write(collisionDescriptorPair.Key.Y);
                netBuffer.Write(collisionDescriptorPair.Value.CollisionArea);
            }

            netBuffer.Write(_mapObjects.Count);
            foreach (var mapObject in _mapObjects)
            {
                netBuffer.Write(mapObject.Pack());
            }

            return netBuffer;
        }


        public void Load(BinaryReader bR)
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (bR.ReadBoolean())
                    {
                        _tiles[x, y] = new Tile(new Vector(x * Settings.TileSize, y * Settings.TileSize));
                        _tiles[x, y].Load(bR, new Vector(x * Settings.TileSize, y * Settings.TileSize));
                        _tiles[x, y].NPCSpawnerEvent += OnNpcSpawnerEvent;
                    }
                }
            }

            int mapObjectCount = bR.ReadInt32();
            for (int i = 0; i < mapObjectCount; i++)
            {
                var mapObject = MapObject.Load(bR, this);
                _mapObjects.Add(mapObject);
            }
        }


        private void OnNpcSpawnerEvent(object sender, Tile.NPCSpawnerEventArgs e)
        {
            this.NPCSpawnerEvent?.Invoke(this, e);
        }

        public event EventHandler<Tile.NPCSpawnerEventArgs> NPCSpawnerEvent;

    }
}