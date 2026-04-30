using LunarColor = Lunar.Core.Content.Graphics.Color;
using LunarRect = Lunar.Core.Utilities.Data.Rect;
using LunarVector = Lunar.Core.Utilities.Data.Vector;
using XnaColor = Microsoft.Xna.Framework.Color;
using XnaRectangle = Microsoft.Xna.Framework.Rectangle;
using XnaVector2 = Microsoft.Xna.Framework.Vector2;

namespace Lunar.Graphics
{
    /// <summary>
    /// Conversions between Lunar.Core's renderer-agnostic data types and MonoGame primitives.
    /// Lives in Lunar.Graphics so Lunar.Core has no MonoGame dependency.
    /// </summary>
    public static class MonoGameInterop
    {
        public static XnaColor ToXna(this LunarColor c) => new XnaColor(c.R, c.G, c.B, c.A);

        public static LunarColor ToLunar(this XnaColor c) => new LunarColor(c.R, c.G, c.B, c.A);

        public static XnaVector2 ToXna(this LunarVector v) => new XnaVector2(v.X, v.Y);

        public static LunarVector ToLunar(this XnaVector2 v) => new LunarVector(v.X, v.Y);

        public static XnaRectangle ToXna(this LunarRect r) => new XnaRectangle(r.X, r.Y, r.Width, r.Height);

        public static LunarRect ToLunar(this XnaRectangle r) => new LunarRect(r.X, r.Y, r.Width, r.Height);
    }
}
