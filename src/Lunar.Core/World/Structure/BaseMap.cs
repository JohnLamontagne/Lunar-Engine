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
using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.World.Structure
{
    [Serializable]
    public class BaseMap<T> : IBaseMap<T> where T : IBaseLayer<IBaseTile<SpriteInfo>>
    {
        private Vector _dimensions;
        private readonly Dictionary<string, T> _layers;
        private List<string> _tilesetPaths;

        public string Name { get; set; }

        public Vector Dimensions
        {
            get => _dimensions;
            set
            {
                _dimensions = value;

                if (this.Bounds.Width >= this.Dimensions.X || this.Bounds.Height >= this.Dimensions.Y)
                    this.Bounds = new Rect(0, 0, (int)this.Dimensions.X, (int)this.Dimensions.Y);

                if (_layers != null)
                {
                    foreach (var layer in _layers.Values)
                        layer.Resize(this.Dimensions);
                }
                this.DimensionsChanged?.Invoke(this, new EventArgs());
            }
        }

        public Rect Bounds { get; set; }

        public bool Dark { get; set; }

        public List<string> TilesetPaths { get => _tilesetPaths; set => _tilesetPaths = value; }

        public IReadOnlyCollection<T> Layers => _layers.Values;

        public BaseMap()
        {
            _layers = new Dictionary<string, T>();
            _tilesetPaths = new List<string>();
        }

        protected BaseMap(List<string> tilesetPaths)
        {
            _layers = new Dictionary<string, T>();
            _tilesetPaths = tilesetPaths;
        }

        public BaseMap(Vector dimensions, string name)
        : this(new List<string>())
        {
            this.Dimensions = dimensions;
            this.Name = name;

            this.Bounds = new Rect(0, 0, this.Dimensions.X, this.Dimensions.Y);
        }

        public virtual bool WithinBounds(float x, float y)
        {
            return x >= this.Bounds.X && y >= this.Bounds.Y && x < this.Bounds.Width
                && y < this.Bounds.Height;
        }

        public virtual bool WithinBounds(Vector position)
        {
            return this.WithinBounds(position.X, position.Y);
        }

        public bool LayerExists(string name)
        {
            return _layers.ContainsKey(name);
        }

        public void RemoveLayer(string name)
        {
            if (this.LayerExists(name))
                _layers.Remove(name);
        }

        public T GetLayer(string name)
        {
            return _layers[name];
        }

        public void AddLayer(string name, IBaseLayer<IBaseTile<SpriteInfo>> layer)
        {
            _layers.Add(name, (T)layer);
        }

        public event EventHandler<EventArgs> DimensionsChanged;
    }
}