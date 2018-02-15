/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using System;

namespace Lunar.Core.Utilities.Data
{
    public struct Vector
    {
        public const float COMPARISON_TOLERANCE = 0.00001f;

        private readonly float _x;
        private readonly float _y;

        public float X => _x;

        public float Y => _y;

        public Vector(float x, float y)
        {
            _x = x;
            _y = y;
        }

        public Vector(int x, int y)
        {
            _x = x;
            _y = y;
        }

        public static Vector FromString(string value)
        {
            try
            {
                value = value.TrimStart('{').TrimEnd('}');

                var components = value.Split(' ');

                // Remove the X:/Y:
                components[0] = components[0].Remove(0, 2);
                components[1] = components[1].Remove(0, 2);

                return new Vector(float.Parse(components[0]), float.Parse(components[1]));
            }
            catch (Exception ex)
            {
                throw new Exception("Invalid string: only strings that have the {X:a Y:b} format can be translated into a vector!", ex);
            }
        }

        public static Vector Zero => new Vector(0, 0);

        public static Vector operator +(Vector vecOne, Vector vecTwo)
        {
            return new Vector(vecOne.X + vecTwo.X, vecOne.Y + vecTwo.Y);
        }

        public static Vector operator -(Vector vecOne, Vector vecTwo)
        {
            return new Vector(vecOne.X - vecTwo.X, vecOne.Y - vecTwo.Y);
        }

        public static Vector operator *(Vector vecOne, Vector vecTwo)
        {
            return new Vector(vecOne.X * vecTwo.X, vecOne.Y * vecTwo.Y);
        }

        public static Vector operator /(Vector vecOne, Vector vecTwo)
        {
            return new Vector(vecOne.X / vecTwo.X, vecOne.Y / vecTwo.Y);
        }

        public static Vector operator %(Vector vecOne, Vector vecTwo)
        {
            return new Vector(vecOne.X % vecTwo.X, vecOne.Y % vecTwo.Y);
        }

        public static Vector operator +(Vector vecOne, int num)
        {
            return new Vector(vecOne.X + num, vecOne.Y + num);
        }

        public static Vector operator -(Vector vecOne, int num)
        {
            return new Vector(vecOne.X - num, vecOne.Y - num);
        }

        public static Vector operator *(Vector vecOne, int num)
        {
            return new Vector(vecOne.X * num, vecOne.Y * num);
        }

        public static Vector operator /(Vector vecOne, int num)
        {
            return new Vector(vecOne.X / num, vecOne.Y / num);
        }

        public static Vector operator %(Vector vecOne, int num)
        {
            return new Vector(vecOne.X % num, vecOne.Y % num);
        }

        public static Vector operator +(Vector vecOne, float num)
        {
            return new Vector(vecOne.X + num, vecOne.Y + num);
        }

        public static Vector operator -(Vector vecOne, float num)
        {
            return new Vector(vecOne.X - num, vecOne.Y - num);
        }

        public static Vector operator *(Vector vecOne, float num)
        {
            return new Vector(vecOne.X * num, vecOne.Y * num);
        }

        public static Vector operator /(Vector vecOne, float num)
        {
            return new Vector(vecOne.X / num, vecOne.Y / num);
        }

        public static Vector operator %(Vector vecOne, float num)
        {
            return new Vector(vecOne.X % num, vecOne.Y % num);
        }

        public static bool operator ==(Vector vecOne, Vector vecTwo)
        {
            return (Math.Abs(vecOne.X - vecTwo.X) < Vector.COMPARISON_TOLERANCE && Math.Abs(vecOne.Y - vecTwo.Y) < Vector.COMPARISON_TOLERANCE);
        }

        public static bool operator !=(Vector vecOne, Vector vecTwo)
        {
            return !(vecOne == vecTwo);
        }

        public static Vector Normalize(Vector A)
        {
            float distance = (float)Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Vector(A.X / distance, A.Y / distance);
        }

        public static float Dot(Vector vectorA, Vector vectorB)
        {
            return (vectorA.X * vectorB.X) + (vectorA.Y * vectorB.Y);
        }

        public override int GetHashCode()
        {  
            return this.X.GetHashCode() ^ this.Y.GetHashCode() << 2;
        }

        public override string ToString()
        {
            return $"{this.X}:{this.Y}";
        }
    }
}