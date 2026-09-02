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

using Lunar.Core.Utilities.Data;
using Color = Lunar.Core.Content.Graphics.Color;

namespace Lunar.Core.Net
{
    public static class PacketExtensions
    {
        public static Packet Write(this Packet packet, Color color)
        {
            packet.Write(color.R);
            packet.Write(color.G);
            packet.Write(color.B);
            packet.Write(color.A);
            return packet;
        }

        public static Packet Write(this Packet packet, Rect rect)
        {
            packet.Write(rect.X);
            packet.Write(rect.Y);
            packet.Write(rect.Width);
            packet.Write(rect.Height);
            return packet;
        }

        public static Packet Write(this Packet packet, Vector vector)
        {
            packet.Write(vector.X);
            packet.Write(vector.Y);
            return packet;
        }

        public static Packet Write(this Packet packet, Vector3 vector)
        {
            packet.Write(vector.X);
            packet.Write(vector.Y);
            packet.Write(vector.Z);
            return packet;
        }

        public static Packet Write(this Packet packet, Box box)
        {
            packet.Write(box.Min);
            packet.Write(box.Max);
            return packet;
        }

        public static Color ReadColor(this Packet packet)
        {
            return new Color(packet.ReadByte(), packet.ReadByte(), packet.ReadByte(), packet.ReadByte());
        }

        public static Rect ReadRect(this Packet packet)
        {
            return new Rect(packet.ReadInt32(), packet.ReadInt32(), packet.ReadInt32(), packet.ReadInt32());
        }

        public static Vector ReadVector(this Packet packet)
        {
            return new Vector(packet.ReadFloat(), packet.ReadFloat());
        }

        public static Vector3 ReadVector3(this Packet packet)
        {
            return new Vector3(packet.ReadFloat(), packet.ReadFloat(), packet.ReadFloat());
        }

        public static Box ReadBox(this Packet packet)
        {
            return new Box(packet.ReadVector3(), packet.ReadVector3());
        }
    }
}
