using Lidgren.Network;
using Microsoft.Xna.Framework;
using Lunar.Core.Utilities.Data;
using Color = Lunar.Core.Content.Graphics.Color;

namespace Lunar.Core.Net
{
    public static class LidgrenExtensions
    {
        public static NetBuffer Write(this NetBuffer netBuffer, Color color)
        {
            netBuffer.Write(color.R);
            netBuffer.Write(color.G);
            netBuffer.Write(color.B);
            netBuffer.Write(color.A);

            return netBuffer;
        }

        public static NetBuffer Write(this NetBuffer netBuffer, Rect intRect)
        {
            netBuffer.Write(intRect.Left);
            netBuffer.Write(intRect.Top);
            netBuffer.Write(intRect.Width);
            netBuffer.Write(intRect.Height);

            return netBuffer;
        }

        public static NetBuffer Write(this NetBuffer netBuffer, Vector vector)
        {
            netBuffer.Write(vector.X);
            netBuffer.Write(vector.Y);

            return netBuffer;
        }

        public static Color ReadColor(this NetBuffer netBuffer)
        {
            return new Color(netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte(), netBuffer.ReadByte());
        }

        public static Rect ReadIntRect(this NetBuffer netBuffer)
        {
            return new Rect(netBuffer.ReadInt32(), netBuffer.ReadInt32(), netBuffer.ReadInt32(), netBuffer.ReadInt32());
        }

        public static Vector ReadVector(this NetBuffer netBuffer)
        {
            return new Vector(netBuffer.ReadFloat(), netBuffer.ReadFloat());
        }

        public static Vector2 ReadVector2(this NetBuffer netBuffer)
        {
            return new Vector2(netBuffer.ReadFloat(), netBuffer.ReadFloat());
        }
    }
}