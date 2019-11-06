using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using DarkUI.Forms;
using Lunar.Core.World.Structure;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Editor.Utilities;
using Lunar.Graphics;
using Lunar.Core.Content.Graphics;

namespace Lunar.Editor.World
{
    public class Map : MapModel<Layer>
    {
        private Dictionary<string, Texture2D> _tilesets;

        [Browsable(false)]
        public IEnumerable<Texture2D> Tilesets => _tilesets.Values;

        public int Height { get => (int)this.Dimensions.Y; set => this.Dimensions = new Core.Utilities.Data.Vector(this.Dimensions.X, value); }

        public int Width { get => (int)this.Dimensions.X; set => this.Dimensions = new Core.Utilities.Data.Vector(value, this.Dimensions.Y); }

        public Map(Vector2 dimensions, string name)
        {
            this.Name = name;
            this.Dimensions = dimensions;

            _tilesets = new Dictionary<string, Texture2D>();

            this.AddLayer("Ground", new Layer(this.Dimensions, "Ground", 0));
            this.AddLayer("Mask1", new Layer(this.Dimensions, "Mask1", 1));
            this.AddLayer("Mask2", new Layer(this.Dimensions, "Mask2", 2));
            this.AddLayer("Fringe", new Layer(this.Dimensions, "Fringe", 3));
        }

        public Map(MapModel<LayerModel<TileModel<SpriteInfo>>> descriptor, TextureLoader textureLoader, Project project)
            : base(descriptor.TilesetPaths)
        {
            _tilesets = new Dictionary<string, Texture2D>();

            this.DimensionsChanged += (sender, args) =>
            {
                if (this.Layers != null)
                {
                    foreach (var layer in this.Layers)
                        layer.Resize(this.Dimensions);
                }

                this.Map_Resized?.Invoke(this, new EventArgs());
            };

            this.Initalize(project, textureLoader, descriptor);
        }

        private Map()
        {
            _tilesets = new Dictionary<string, Texture2D>();
        }

        public void AddTileset(Texture2D texture)
        {
            if (!_tilesets.ContainsKey(Path.GetFileName(texture.Tag.ToString())))
                _tilesets.Add(Path.GetFileName(texture.Tag.ToString()), texture);

            if (!this.TilesetPaths.Contains(texture.Tag.ToString()))
                this.TilesetPaths.Add(texture.Tag.ToString());
        }

        public bool TilesetExists(string tilesetPath)
        {
            return _tilesets.ContainsKey(tilesetPath);
        }

        public void RemoveTileset(string tilesetPath)
        {
            _tilesets.Remove(tilesetPath);
            this.TilesetPaths.Remove(tilesetPath);
        }

        public Texture2D GetTileset(string tilesetPath)
        {
            return _tilesets[tilesetPath];
        }

        public void Update(GameTime gameTime)
        {
            foreach (var layer in this.Layers)
                layer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var layer in this.Layers)
            {
                layer.Draw(spriteBatch, camera);
            }
        }

        private void Initalize(Project project, TextureLoader textureLoader, MapModel<LayerModel<TileModel<SpriteInfo>>> descriptor)
        {
            this.Name = descriptor.Name;
            this.Bounds = descriptor.Bounds;
            this.Dark = descriptor.Dark;
            this.Dimensions = descriptor.Dimensions;

            foreach (var tilesetPath in descriptor.TilesetPaths)
            {
                if (File.Exists(project.ClientRootDirectory + "/" + tilesetPath))
                {
                    var texture = textureLoader.LoadFromFile(project.ClientRootDirectory + "/" + tilesetPath);
                    texture.Tag = tilesetPath;

                    this.AddTileset(texture);
                }
                else
                {
                    DarkMessageBox.ShowError($"Could not load tileset {tilesetPath}!", "Error loading tileset!",
                        DarkDialogButton.Ok);
                }
            }

            foreach (var layerDesc in descriptor.Layers)
            {
                var layer = new Layer(layerDesc);

                for (int x = 0; x < layerDesc.Tiles.GetLength(0); x++)
                {
                    for (int y = 0; y < layerDesc.Tiles.GetLength(1); y++)
                    {
                        var tileDesc = layerDesc.Tiles[x, y];

                        if (tileDesc != null)
                        {
                            Tile tile = new Tile(tileDesc);

                            if (tileDesc.Sprite != null && _tilesets.ContainsKey(Path.GetFileName(tileDesc.Sprite.TextureName)))
                            {
                                tile.Sprite = new Sprite(_tilesets[Path.GetFileName(tileDesc.Sprite.TextureName)]);
                                tile.Sprite.Transform.LayerDepth = tileDesc.Sprite.Transform.LayerDepth;
                                tile.Sprite.Transform.Rect = tileDesc.Sprite.Transform.Rect;
                                tile.Sprite.Transform.Position = tileDesc.Position;
                            }
                            layer.SetTile(x, y, tile);
                        }
                        else
                        {
                            layer.SetTile(x, y, new Tile());
                        }
                    }
                }

                this.AddLayer(layer.Name, layer);
            }
        }

        public event EventHandler Map_Resized;
    }
}