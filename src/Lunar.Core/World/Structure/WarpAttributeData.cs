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

namespace Lunar.Core.World.Structure
{
    [Serializable]
    public class WarpAttributeData : AttributeData
    {
        public int X { get; set; }

        public int Y { get; set; }

        public string WarpMap { get; set; }

        public string LayerName { get; set; }

        public WarpAttributeData(int x, int y, string warpMap, string layerName)
        {
            this.X = x;
            this.Y = y;
            this.WarpMap = warpMap;
            this.LayerName = layerName;
        }
    }
}
