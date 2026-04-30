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

using Lunar.Core.Content.Graphics;
using System.IO;

namespace Lunar.Core.World.Structure.Attribute
{
    public class WarpTileAttribute : TileAttribute
    {
        internal const byte TYPE_ID = 2;

        public override Color Color => new Color(100, 255, 255, 100);

        public int X { get; set; }

        public int Y { get; set; }

        public string WarpMap { get; set; }

        public string LayerName { get; set; }

        public WarpTileAttribute(int x, int y, string warpMap, string layerName)
        {
            this.X = x;
            this.Y = y;
            this.WarpMap = warpMap;
            this.LayerName = layerName;
        }

        protected override byte TypeId => TYPE_ID;

        protected override void WriteData(BinaryWriter writer)
        {
            writer.Write(X);
            writer.Write(Y);
            writer.Write(WarpMap ?? string.Empty);
            writer.Write(LayerName ?? string.Empty);
        }

        internal static WarpTileAttribute ReadData(BinaryReader reader)
        {
            var x = reader.ReadInt32();
            var y = reader.ReadInt32();
            var warpMap = reader.ReadString();
            var layerName = reader.ReadString();
            return new WarpTileAttribute(x, y, warpMap, layerName);
        }
    }
}
