using System;
using Microsoft.Xna.Framework;

namespace Lunar.Core.Utilities.Data
{
    public static class Vector2Helper
    {
        public static Vector2 FromString(this Vector2 vec, string value)
        {
            try
            {
                value = value.TrimStart('{').TrimEnd('}');

                var components = value.Split(' ');

                // Remove the X:/Y:
                components[0] = components[0].Remove(0, 2);
                components[1] = components[1].Remove(0, 2);

                return new Vector2(float.Parse(components[0]), float.Parse(components[1]));
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid string: only strings that have the {X:a Y:b} format can be translated into a vector!", ex);
            }
        }
    }
}