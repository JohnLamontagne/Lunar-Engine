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
    public abstract class TileAttribute
    {
        public ITileAttributeActionHandler ActionHandler { get; set; }

        /// <summary>
        /// Used for marking on map when attribute overlay is enabled.
        /// </summary>
        public abstract Color Color { get; }

        protected abstract byte TypeId { get; }

        protected abstract void WriteData(BinaryWriter writer);

        public byte[] Serialize()
        {
            using var memoryStream = new MemoryStream();
            using var writer = new BinaryWriter(memoryStream);
            writer.Write(TypeId);
            WriteData(writer);
            return memoryStream.ToArray();
        }

        public static TileAttribute Deserialize(byte[] data)
        {
            using var memoryStream = new MemoryStream(data);
            using var reader = new BinaryReader(memoryStream);
            var typeId = reader.ReadByte();
            return typeId switch
            {
                BlockedTileAttribute.TYPE_ID => BlockedTileAttribute.ReadData(reader),
                WarpTileAttribute.TYPE_ID => WarpTileAttribute.ReadData(reader),
                NPCSpawnTileAttribute.TYPE_ID => NPCSpawnTileAttribute.ReadData(reader),
                PlayerSpawnTileAttribute.TYPE_ID => PlayerSpawnTileAttribute.ReadData(reader),
                StartDialogueTileAttribute.TYPE_ID => StartDialogueTileAttribute.ReadData(reader),
                _ => throw new InvalidDataException($"Unknown tile attribute type id: {typeId}")
            };
        }
    }
}
