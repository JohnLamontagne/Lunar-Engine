using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Lunar.Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World.Structure;
using Lunar.Editor.Utilities;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities;

namespace Lunar.Editor.World
{
    public class Layer : LayerModel<Tile>
    {
        private List<MapObject> _mapObjects;

        public bool Visible { get; set; }

        public List<MapObject> MapObjects
        {
            get => _mapObjects;
        }

        public Layer(Vector2 dimensions, string name, int layerIndex)
        {
            _mapObjects = new List<MapObject>();
            this.Tiles = new Tile[(int)dimensions.X, (int)dimensions.Y];

            this.Name = name;
            this.LayerIndex = layerIndex;

            this.Visible = true;
        }

        public Layer(LayerModel<TileModel<SpriteInfo>> layerDescriptor)
        {
            this.Name = layerDescriptor.Name;

            this.Tiles = new Tile[layerDescriptor.Tiles.GetLength(0), layerDescriptor.Tiles.GetLength(1)];
            for (int x = 0; x < layerDescriptor.Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < layerDescriptor.Tiles.GetLength(1); y++)
                {
                    if (layerDescriptor.Tiles[x, y] != null)
                        this.Tiles[x, y] = new Tile(layerDescriptor.Tiles[x, y]);
                }
            }

            this.LayerIndex = layerDescriptor.LayerIndex;

            _mapObjects = new List<MapObject>();
        }

        public void Resize(Vector2 dimensions)
        {
            this.Tiles = Helpers.ResizeArray<Tile>(this.Tiles, (int)dimensions.X, (int)dimensions.Y);
        }

        public MapObject TryGetMapObject(Vector2 position)
        {
            return this.MapObjects.FirstOrDefault(x => x.Contains(position));
        }

        public void Update(GameTime gameTime)
        {
            for (int x = 0; x < this.Tiles.GetLength(0); x++)
            {
                for (int y = 0; y < this.Tiles.GetLength(1); y++)
                {
                    if (this.Tiles[x, y] != null)
                    {
                        this.Tiles[x, y].Update(gameTime);
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
                    if (x < this.Tiles.GetLength(0) && y < this.Tiles.GetLength(1))
                    {
                        if (this.Tiles[x, y] != null)
                            this.Tiles[x, y].Draw(spriteBatch);
                    }
                }
            }

            foreach (var mapObject in this.MapObjects)
                mapObject.Draw(spriteBatch);
        }

        public Tile GetTile(int x, int y)
        {
            if (x >= this.Tiles.GetLength(0) || x < 0 || y >= this.Tiles.GetLength(1) || y < 0)
            {
                Engine.Services.Get<Logger>().LogEvent("Fatal error: attempted to access an invalid tile!", LogTypes.ERROR);
                return null;
            }

            return this.Tiles[x, y];
        }

        public void SetTile(int x, int y, Tile tile)
        {
            if (x >= this.Tiles.GetLength(0) || x < 0 || y >= this.Tiles.GetLength(1) || y < 0)
            {
                Engine.Services.Get<Logger>().LogEvent("Fatal error: attempted to access an invalid tile!", LogTypes.ERROR);
            }

            this.Tiles[x, y] = tile;
        }
    }
}