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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Core.World.Structure;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;

namespace Lunar.Server.World.Structure
{
    public class Layer
    {
        private LayerDescriptor _layerDescriptor;

        public LayerDescriptor Descriptor => _layerDescriptor;

        private readonly Tile[,] _tiles;

        private Dictionary<Vector, CollisionBody> _collisionDescriptors;
        private Dictionary<Player, List<Tile>> _playerCollidingTiles;
        private List<MapObject> _mapObjects;

        public Dictionary<Vector, CollisionBody> CollisionDescriptors { get { return _collisionDescriptors; } }

        public Layer(LayerDescriptor descriptor)
        {
            _layerDescriptor = descriptor;
            _tiles = new Tile[descriptor.Tiles.GetLength(0), descriptor.Tiles.GetLength(0)];
            _playerCollidingTiles = new Dictionary<Player, List<Tile>>(); ;
            _collisionDescriptors = new Dictionary<Vector, CollisionBody>();
            _mapObjects = new List<MapObject>();

            this.LoadData();
        }

        private void LoadData()
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (this.Descriptor.Tiles[x, y] != null)
                    {
                        _tiles[x, y] = new Tile(this.Descriptor.Tiles[x, y]);
                        _tiles[x, y].NPCSpawnerEvent += OnNpcSpawnerEvent;
                    }
                    else
                    {
                        _tiles[x, y] = new Tile(new Vector(x * EngineConstants.TILE_SIZE, y * EngineConstants.TILE_SIZE));
                    }
                }
            }
        }

        public CollisionBody GetCollisionDescriptor(Vector position)
        {
            if (_collisionDescriptors.Keys.Contains(position))
                return _collisionDescriptors[position];
            else
                return null;
        }

        public CollisionBody GetCollisionDescriptor(int x, int y)
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

        public void AddCollisionDescriptor(Vector position, CollisionBody descriptor)
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

        public Tile GetTile(Vector position)
        {
            if (position.X >= _tiles.GetLength(0) || position.X < 0 || position.Y >= _tiles.GetLength(1) || position.Y < 0)
                return null;

            return _tiles[(int)position.X, (int)position.Y];
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

        public List<Tile> GetCollidingTiles(IActor<IActorDescriptor> actor)
        {
            return this.GetCollidingTiles(actor.CollisionBody.CollisionArea);
        }

        public List<Tile> GetCollidingTiles(Rect collisionArea)
        {
            List<Tile> collidingTiles = new List<Tile>();

            int leftCheck = collisionArea.Left / Settings.TileSize;
            int topCheck =  collisionArea.Top / Settings.TileSize;
            int tilesWidth = (((collisionArea.Left + collisionArea.Width) / Settings.TileSize) + 1) - leftCheck;
            int tilesHeight = (((collisionArea.Top + collisionArea.Height) / Settings.TileSize) + 1) - topCheck;


            for (int x = 0; x < tilesWidth; x++)
            {
                if (this.GetTile(leftCheck + x, topCheck) != null && this.GetTile(leftCheck + x, topCheck).CheckCollision(collisionArea))
                {
                    collidingTiles.Add(this.GetTile(leftCheck + x, topCheck));
                }
              
                for (int y = 0; y < tilesHeight; y++)
                {
                    if (this.GetTile(leftCheck + x, topCheck + y) != null && this.GetTile(leftCheck + x, topCheck + y).CheckCollision(collisionArea))
                    {
                        collidingTiles.Add(this.GetTile(leftCheck + x, topCheck + y));
                    }
                }
            }

            return collidingTiles; // Returns the resultant list with null tiles removed
        }

        public bool CheckCollision(Rect collisionArea)
        {
            if (collisionArea.Left < 0 || collisionArea.Top < 0 ||
                collisionArea.Left + collisionArea.Width >= (_tiles.GetLength(0) * Settings.TileSize) || collisionArea.Top + collisionArea.Height >= (_tiles.GetLength(1) * Settings.TileSize))
                return true;

            foreach (var collisionDescriptor in _collisionDescriptors.Values)
            {
                if (collisionDescriptor.Collides(collisionArea))
                {
                    return true;
                }
            }

            var collidingTiles = this.GetCollidingTiles(collisionArea);

            foreach (var collidingTile in collidingTiles)
            {
                if (collidingTile.Descriptor.Attribute == TileAttributes.Blocked)
                    return true;

            }

            return false;
        }

        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.Descriptor.Name);
            netBuffer.Write(this.Descriptor.LayerIndex);

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


 


        private void OnNpcSpawnerEvent(object sender, Tile.NPCSpawnerEventArgs e)
        {
            this.NPCSpawnerEvent?.Invoke(this, e);
        }

        public event EventHandler<Tile.NPCSpawnerEventArgs> NPCSpawnerEvent;

    }
}