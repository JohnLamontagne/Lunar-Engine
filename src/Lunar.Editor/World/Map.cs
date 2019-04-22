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

namespace Lunar.Editor.World
{
    public class Map
    {
        private MapDescriptor _descriptor;

        private Dictionary<string, Layer> _layers;

        private Dictionary<string, Texture2D> _tilesets;

        public string Name
        {
            get { return this.Descriptor.Name; }

            set { this.Descriptor.Name = value; }
        }

        public MapDescriptor Descriptor => _descriptor;

        public IEnumerable<Texture2D> Tilesets => _tilesets.Values;

        [Browsable(false)]
        public Dictionary<string, Layer> Layers => _layers;



        [Browsable(false)]
        public Rectangle Bounds { get; private set; }

        public bool Dark { get; set; }

        public Map(Vector2 dimensions, string name)
        {
            _descriptor = new MapDescriptor(dimensions, name);

            _layers = new Dictionary<string, Layer>();
            _tilesets = new Dictionary<string, Texture2D>();

           
            this.AddLayer("Ground", new Layer(this.Descriptor.Dimensions, "Ground", 0));
            this.AddLayer("Mask1", new Layer(this.Descriptor.Dimensions, "Mask1", 1));
            this.AddLayer("Mask2", new Layer(this.Descriptor.Dimensions, "Mask2", 2));
            this.AddLayer("Fringe", new Layer(this.Descriptor.Dimensions, "Fringe", 3));
        }

        public Map(MapDescriptor descriptor, TextureLoader textureLoader)
            : this()
        {
            _descriptor = descriptor;
            _descriptor.DimensionsChanged += (sender, args) =>
            {
                if (_layers != null)
                {
                    foreach (var layer in _layers.Values)
                        layer.Resize(this.Descriptor.Dimensions);
                }

                this.Map_Resized?.Invoke(this, new EventArgs());
            };
        }
       

        private Map()
        {
            _layers = new Dictionary<string, Layer>();
            _tilesets = new Dictionary<string, Texture2D>();
        }

        public void AddTileset(Texture2D texture)
        {
            _tilesets.Add(Path.GetFileName(texture.Tag.ToString()), texture);

            if (!this.Descriptor.TilesetPaths.Contains(texture.Tag.ToString()))
                this.Descriptor.TilesetPaths.Add(texture.Tag.ToString());
        }

        public bool TilesetExists(string tilesetPath)
        {
            return _tilesets.ContainsKey(tilesetPath);
        }

        public void RemoveTileset(string tilesetPath)
        {
            _tilesets.Remove(tilesetPath);
            this.Descriptor.TilesetPaths.Remove(tilesetPath);
        }

        public Texture2D GetTileset(string tilesetPath)
        {
            return _tilesets[tilesetPath];
        }

        public void AddLayer(string layerName, Layer layer)
        {
            _layers.Add(layerName, layer);

            _descriptor.Layers.Add(layerName, _layers[layerName].Descriptor);
        }

        public void RemoveLayer(string layerName)
        {
            _layers.Remove(layerName);
            _descriptor.Layers.Remove(layerName);
        }


        
        public void Update(GameTime gameTime)
        {
            foreach (var layer in _layers.Values)
                layer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch, Camera camera)
        {
            foreach (var layer in _layers.Values)
            {
                layer.Draw(spriteBatch, camera);
            }
        }

        public void Initalize(Project project, TextureLoader textureLoader)
        {
            foreach (var tilesetPath in this.Descriptor.TilesetPaths)
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

            foreach (var layerDesc in this.Descriptor.Layers.Values)
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

                            if (tileDesc.SpriteInfo != null && _tilesets.ContainsKey(Path.GetFileName(tileDesc.SpriteInfo.TextureName)))
                            {
                                tile.Sprite = new Sprite(_tilesets[Path.GetFileName(tileDesc.SpriteInfo.TextureName)])
                                {
                                    LayerDepth = tileDesc.SpriteInfo.Transform.LayerDepth,
                                    SourceRectangle = new Rectangle(tileDesc.SpriteInfo.Transform.Rect.Left, tileDesc.SpriteInfo.Transform.Rect.Top, tileDesc.SpriteInfo.Transform.Rect.Width, tileDesc.SpriteInfo.Transform.Rect.Height),
                                    Position = new Vector2(tileDesc.Position.X, tileDesc.Position.Y)
                                };
                            }
                            layer.SetTile(x, y, tile);
                        }
                        else
                        {
                            layer.SetTile(x, y, new Tile());
                        }
                        
                    }
                }
                
                this.Layers.Add(layer.Descriptor.Name, layer);
            }
        }

        public event EventHandler Map_Resized;
    }
}