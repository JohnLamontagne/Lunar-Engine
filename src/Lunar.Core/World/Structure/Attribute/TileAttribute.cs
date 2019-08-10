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
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Lunar.Core.World.Structure.Attribute
{
    [Serializable]
    public abstract class TileAttribute
    {
        [NonSerialized]
        private ITileAttributeActionHandler _actionHandler;

        /// <summary>
        /// Handles incoming actions from the tile in which the attribute lives.
        /// Setting to null indicates no processing needed.
        /// </summary>
        public ITileAttributeActionHandler ActionHandler { get => _actionHandler; set => _actionHandler = value; }

        /// <summary>
        /// Used for marking on map when attribute overlay is enabled.
        /// </summary>
        public abstract Color Color { get; }

        public virtual byte[] Serialize()
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(memoryStream, this);
            return memoryStream.ToArray();
        }

        public static TileAttribute Deserialize(byte[] data)
        {
            MemoryStream memoryStream = new MemoryStream(data);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            return (TileAttribute)binaryFormatter.Deserialize(memoryStream);
        }
    }
}