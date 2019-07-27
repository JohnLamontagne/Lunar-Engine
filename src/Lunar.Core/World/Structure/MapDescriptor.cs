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
using Lunar.Core.Utilities.Data;
using Lunar.Core.Utilities.Data.Management;

namespace Lunar.Core.World.Structure
{
    public class MapDescriptor : IContentDescriptor
    {
        private Vector _dimensions;
        private readonly Dictionary<string, LayerDescriptor> _layers;
        private List<string> _tilesetPaths;

        public string Name { get; set; }

        public Vector Dimensions
        {
            get => _dimensions;
            set
            {
                _dimensions = value;

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

        public Dictionary<string, LayerDescriptor> Layers => _layers;
        public List<string> TilesetPaths => _tilesetPaths;

        private MapDescriptor()
        {
            _layers = new Dictionary<string, LayerDescriptor>();
            _tilesetPaths = new List<string>();
        }

        public MapDescriptor(Vector dimensions, string name)
        : this()
        {
            this.Dimensions = dimensions;
            this.Name = name;

            this.Bounds = new Rect(0, 0, this.Dimensions.X, this.Dimensions.Y);
        }

        public event EventHandler<EventArgs> DimensionsChanged;
    }
}