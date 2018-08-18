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
using System.ComponentModel;
using System.IO;
using DarkUI.Forms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Editor.Utilities;

namespace Lunar.Editor.World
{
    public class Map
    {
        private Dictionary<string, Layer> _layers;
        private Vector2 _dimensions;
        private Dictionary<string, Texture2D> _tilesets;

        public string Name { get; set; }

        [Browsable(false)]
        public Dictionary<string, Texture2D> Tilesets => _tilesets;

        [Browsable(false)]
        public Dictionary<string, Layer> Layers => _layers;

        public Vector2 Dimensions
        {
            get => _dimensions;
            set
            {
                _dimensions = value;

                this.Bounds = new Rectangle(0, 0, (int)this.Dimensions.X, (int)this.Dimensions.Y);

                if (_layers != null)
                {
                    foreach (var layer in _layers.Values)
                        layer.Resize(this.Dimensions);
                }

                this.Map_Resized?.Invoke(this, new EventArgs());
            }
        }

        [Browsable(false)]
        public Rectangle Bounds { get; private set; }

        public bool Dark { get; set; }

        public Map(Vector2 dimensions, string name)
        {
            this.Dimensions = dimensions;
            this.Name = name;

            _layers = new Dictionary<string, Layer>();
            _tilesets = new Dictionary<string, Texture2D>();

            _layers.Add("Ground", new Layer(this.Dimensions, "Ground", 0));
            _layers.Add("Mask1", new Layer(this.Dimensions, "Mask1", 1));
            _layers.Add("Mask2", new Layer(this.Dimensions, "Mask2", 2));
            _layers.Add("Fringe", new Layer(this.Dimensions, "Fringe", 3));
        }

        private Map()
        {
            _layers = new Dictionary<string, Layer>();
            _tilesets = new Dictionary<string, Texture2D>();
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

        public void Save(string path)
        {
            using (var fileStream = new FileStream(path, FileMode.OpenOrCreate))
            {
                using (var bW = new BinaryWriter(fileStream))
                {

                    bW.Write(_tilesets.Count);
                    foreach (var tileset in _tilesets.Values)
                    {
                        bW.Write(tileset.Tag.ToString());
                    }

                    bW.Write(this.Name);
                    bW.Write((int)this.Dimensions.X);
                    bW.Write((int)this.Dimensions.Y);
                    bW.Write(this.Dark);

                    bW.Write(_layers.Count);
                    foreach (var layer in _layers.Values)
                    {
                        layer.Save(bW);
                    }
                }
            }
        }

        public static Map Load(string path, Project project, TextureLoader textureLoader)
        {
            var map = new Map();

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                using (var bR = new BinaryReader(fileStream))
                {

                    // Load the tileset information
                    int tilesetCount = bR.ReadInt32();

                    for (int i = 0; i < tilesetCount; i++)
                    {
                        string tilesetPath = bR.ReadString();

                        if (File.Exists(project.ClientRootDirectory + "/" + tilesetPath))
                        {
                            Texture2D tileset = textureLoader.LoadFromFile(project.ClientRootDirectory + "/" + tilesetPath);
                            tileset.Tag = tilesetPath;
                            map.Tilesets.Add(Path.GetFileName(tilesetPath), tileset);
                        }
                        else
                        {
                            DarkMessageBox.ShowError($"Could not load tileset {tilesetPath}!", "Error loading tileset!",
                                DarkDialogButton.Ok);
                        }

                        
                    }

                    map.Name = bR.ReadString();
                    map.Dimensions = new Vector2(bR.ReadInt32(), bR.ReadInt32());
                    map.Dark = bR.ReadBoolean();

                    map.Bounds = new Rectangle(0, 0, (int)map.Dimensions.X, (int)map.Dimensions.Y);

                    int layerCount = bR.ReadInt32();
                    for (int i = 0; i < layerCount; i++)
                    {
                        string layerName = bR.ReadString();
                        int layerIndex = bR.ReadInt32();

                        var layer = new Layer(map.Dimensions, layerName, layerIndex);
                        layer.Load(bR, textureLoader, project, map.Tilesets);

                        map.Layers.Add(layerName, layer);
                    }
                }
            }

            return map;
        }

        public event EventHandler Map_Resized;
    }
}