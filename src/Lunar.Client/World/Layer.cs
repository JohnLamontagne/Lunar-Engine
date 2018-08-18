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
using System.Linq;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.Utilities;
using Lunar.Core;
using Lunar.Core.Utilities.Data;

namespace Lunar.Client.World
{
    public class Layer
    {
        private readonly Tile[,] _tiles;

        private Dictionary<Vector2, CollisionDescriptor> _collisionDescriptors;
        private List<MapObject> _mapObjects;

        public string Name { get; }

        public float ZIndex { get; }

        public int LayerIndex { get; }

        public Layer(Vector2 dimensions, int lIndex, string name)
        {
            _tiles = new Tile[(int)dimensions.X, (int)dimensions.Y];
            this.Name = name;
            this.LayerIndex = lIndex;
            this.ZIndex = lIndex * EngineConstants.PARTS_PER_LAYER;

            _collisionDescriptors = new Dictionary<Vector2, CollisionDescriptor>();
            _mapObjects = new List<MapObject>();
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
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            int left = (int)MathHelper.Clamp(camera.Position.X / 32, 0, float.MaxValue);
            int width = (Settings.ResolutionX / 32) + 2;
            int top = (int)MathHelper.Clamp(camera.Position.Y / 32, 0, float.MaxValue);
            int height = (Settings.ResolutionY / 32) + 2;

            for (int x = left; x < (left + width); x++)
            {
                for (int y = top; y < (top + height); y++)
                {
                    if (x < _tiles.GetLength(0) && y < _tiles.GetLength(1))
                    {
                        if (_tiles[x, y] != null)
                            _tiles[x, y].Draw(spriteBatch);
                    }
                }
            }

            foreach (var mapObject in _mapObjects)
                mapObject.Draw(spriteBatch);
        }

        public bool CheckCollision(Vector2 position, Rect collisionBounds)
        {
            return this.CheckCollision(new Vector2(position.X, position.Y), collisionBounds);
        }

        public bool CheckCollision(Vector2 position, Rectangle collisionBounds)
        {
            Rectangle collisionArea = new Rectangle((int)(position.X + collisionBounds.Left), (int)(position.Y + collisionBounds.Top),
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

            return false;
        }


        public CollisionDescriptor GetCollisionDescriptor(Vector2 position)
        {
            if (_collisionDescriptors.Keys.Contains(position))
                return _collisionDescriptors[position];
            else
                return null;
        }


        public CollisionDescriptor GetCollisionDescriptor(int x, int y)
        {
            return this.GetCollisionDescriptor(new Vector2(x, y));
        }

        public void RemoveCollisionDescriptor(Vector2 position)
        {
            _collisionDescriptors.Remove(position);
        }

        public void RemoveCollisionDescriptor(int x, int y)
        {
            this.RemoveCollisionDescriptor(new Vector2(x, y));
        }

        public void AddCollisionDescriptor(Vector2 position, CollisionDescriptor descriptor)
        {
            _collisionDescriptors.Add(position, descriptor);
        }


        public Tile GetTile(int x, int y)
        {
            if (x >= _tiles.GetLength(0) || x < 0 || y >= _tiles.GetLength(1) || y < 0)
                throw new Exception("Fatal error: attempted to access an invalid tile!");

            return _tiles[x, y];
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x >= _tiles.GetLength(0) || x < 0 || y >= _tiles.GetLength(1) || y < 0)
                throw new Exception("Fatal error: attempted to access an invalid tile!");

            _tiles[x, y] = tile;
        }

        public void Unpack(NetBuffer netBuffer)
        {

            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    var tile = Tile.Unpack(netBuffer);
                    if (tile != null)
                    {
                        tile.ZIndex = this.ZIndex;
                        this.SetTile(x, y, tile);
                    }
                }
            }

            int collisionDescriptorCount = netBuffer.ReadInt32();
            for (int c = 0; c < collisionDescriptorCount; c++)
            {
                var position = new Vector2(netBuffer.ReadInt32(), netBuffer.ReadInt32());

                var collisionArea = new Rectangle(netBuffer.ReadInt32(), netBuffer.ReadInt32(), netBuffer.ReadInt32(), netBuffer.ReadInt32());

                var collisionDescriptor = new CollisionDescriptor(collisionArea);

                this.AddCollisionDescriptor(position, collisionDescriptor);
            }

            var mapObjectCount = netBuffer.ReadInt32();

            for (int i = 0; i < mapObjectCount; i++)
            {
                var mapObject = MapObject.Unpack(netBuffer);

                if (mapObject != null)
                    _mapObjects.Add(mapObject);
            }

        }
    }
}