using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;
using Lunar.Editor.Utilities;
using Lunar.Graphics;

namespace Lunar.Editor.World
{
    public class Layer
    {
        private Tile[,] _tiles;
        private List<MapObject> _mapObjects;
        private string _name;
        private float _zIndex;

        public string Name { get; set; }

        public bool Visible { get; set; }

        public float ZIndex
        {
            get => _zIndex;
            set
            {
                _zIndex = value;

                foreach (var tile in _tiles)
                {
                    if (tile != null && tile.Sprite != null)
                        tile.Sprite.LayerDepth = value;
                }
            }
        }

        public List<MapObject> MapObjects
        {
            get => _mapObjects;
        }

        public Layer(Vector2 dimensions, string name, float zIndex)
        {
            _mapObjects = new List<MapObject>();
            _tiles = new Tile[(int)dimensions.X, (int)dimensions.Y];

            this.Name = name;
            this.ZIndex = zIndex;

            this.Visible = true;
        }

        public void Resize(Vector2 dimensions)
        {
            _tiles = HelperFunctions.ResizeArray<Tile>(_tiles, (int)dimensions.X, (int)dimensions.Y);
        }

        public MapObject TryGetMapObject(Vector2 position)
        {
            return this.MapObjects.FirstOrDefault(x => x.Contains(position));
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

            foreach (var mapObject in this.MapObjects)
                mapObject.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            if (!this.Visible)
                return;

            int left = (int)MathHelper.Clamp(camera.Position.X / 32, 0, float.MaxValue);
            int width = (camera.Bounds.Width / 32) + 2;
            int top = (int)MathHelper.Clamp(camera.Position.Y / 32, 0, float.MaxValue);
            int height = (camera.Bounds.Height / 32) + 2;

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

            foreach (var mapObject in this.MapObjects)
                mapObject.Draw(spriteBatch);
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

        public void Save(BinaryWriter bW)
        {
            bW.Write(this.Name);
            bW.Write(this.ZIndex);

            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (_tiles[x, y] != null)
                    {
                        bW.Write(true);

                        _tiles[x, y].Save(bW);
                    }
                    else
                        bW.Write(false);
                }
            }

            bW.Write(_mapObjects.Count);
            foreach (var mapObject in _mapObjects)
            {
               mapObject.Save(bW);
            }
        }

        public void Load(BinaryReader bR, TextureLoader textureLoader, Project project, Dictionary<string, Texture2D> tilesets)
        {
            for (int x = 0; x < _tiles.GetLength(0); x++)
            {
                for (int y = 0; y < _tiles.GetLength(1); y++)
                {
                    if (bR.ReadBoolean())
                    {
                        _tiles[x, y] = new Tile();
                        _tiles[x,y].Load(bR, tilesets, new Vector2(x * Constants.TILE_SIZE, y * Constants.TILE_SIZE));
                    }
                }
            }

            int mapObjectCount = bR.ReadInt32();
            for (int i = 0; i < mapObjectCount; i++)
            {
                _mapObjects.Add(MapObject.Load(bR, this, project, textureLoader));
            }
        }
    }
}