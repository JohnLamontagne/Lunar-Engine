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
using Lunar.Core.Utilities.Logic;

namespace Lunar.Core.Utilities.Data
{
    /// <summary>
    /// Renderer-neutral three component vector used for world positions, directions
    /// and extents in the 3D world model. Lives alongside the legacy 2D <see cref="Vector"/>
    /// so that 2D and 3D code can coexist while the engine is migrated.
    /// Coordinate convention: X = east, Y = up, Z = south (right-handed, Y-up).
    /// </summary>
    public readonly struct Vector3 : IEquatable<Vector3>
    {
        public const float COMPARISON_TOLERANCE = Vector.COMPARISON_TOLERANCE;

        public float X { get; }

        public float Y { get; }

        public float Z { get; }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Vector3(float value)
            : this(value, value, value)
        {
        }

        /// <summary>
        /// Lifts a legacy 2D vector into 3D. The 2D Y axis (screen "down") maps onto the
        /// 3D Z axis (world "south") so that a top-down 2D map keeps its layout on the ground plane.
        /// </summary>
        public static Vector3 FromGroundPlane(Vector planar, float height = 0f)
        {
            return new Vector3(planar.X, height, planar.Y);
        }

        /// <summary>
        /// Projects this vector onto the ground plane, dropping height. Inverse of <see cref="FromGroundPlane"/>.
        /// </summary>
        public Vector ToGroundPlane()
        {
            return new Vector(X, Z);
        }

        public static Vector3 Zero => new Vector3(0f, 0f, 0f);
        public static Vector3 One => new Vector3(1f, 1f, 1f);
        public static Vector3 UnitX => new Vector3(1f, 0f, 0f);
        public static Vector3 UnitY => new Vector3(0f, 1f, 0f);
        public static Vector3 UnitZ => new Vector3(0f, 0f, 1f);
        public static Vector3 Up => UnitY;
        public static Vector3 Down => new Vector3(0f, -1f, 0f);
        public static Vector3 Right => UnitX;
        public static Vector3 Left => new Vector3(-1f, 0f, 0f);
        public static Vector3 Forward => new Vector3(0f, 0f, -1f);
        public static Vector3 Backward => UnitZ;

        public float LengthSquared => X * X + Y * Y + Z * Z;

        public float Length => (float)Math.Sqrt(this.LengthSquared);

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

        public static Vector3 operator -(Vector3 a) => new Vector3(-a.X, -a.Y, -a.Z);

        public static Vector3 operator *(Vector3 a, Vector3 b) => new Vector3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);

        public static Vector3 operator /(Vector3 a, Vector3 b) => new Vector3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);

        public static Vector3 operator *(Vector3 a, float scalar) => new Vector3(a.X * scalar, a.Y * scalar, a.Z * scalar);

        public static Vector3 operator *(float scalar, Vector3 a) => a * scalar;

        public static Vector3 operator /(Vector3 a, float scalar) => new Vector3(a.X / scalar, a.Y / scalar, a.Z / scalar);

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return Math.Abs(a.X - b.X) < COMPARISON_TOLERANCE
                && Math.Abs(a.Y - b.Y) < COMPARISON_TOLERANCE
                && Math.Abs(a.Z - b.Z) < COMPARISON_TOLERANCE;
        }

        public static bool operator !=(Vector3 a, Vector3 b) => !(a == b);

        public static float Dot(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(
                a.Y * b.Z - a.Z * b.Y,
                a.Z * b.X - a.X * b.Z,
                a.X * b.Y - a.Y * b.X);
        }

        /// <summary>
        /// Returns the unit-length vector pointing in the same direction, or <see cref="Zero"/>
        /// when the input has no length (unlike <see cref="Vector.Normalize"/>, this never produces NaN).
        /// </summary>
        public static Vector3 Normalize(Vector3 a)
        {
            float length = a.Length;
            return length < COMPARISON_TOLERANCE ? Zero : a / length;
        }

        public static float Distance(Vector3 a, Vector3 b) => (b - a).Length;

        public static float DistanceSquared(Vector3 a, Vector3 b) => (b - a).LengthSquared;

        /// <summary>
        /// Distance measured on the ground plane only (ignores height). Useful for
        /// aggro ranges, spell ranges and interest management where height should not count.
        /// </summary>
        public static float PlanarDistance(Vector3 a, Vector3 b)
        {
            float dx = b.X - a.X;
            float dz = b.Z - a.Z;
            return (float)Math.Sqrt(dx * dx + dz * dz);
        }

        public static Vector3 Lerp(Vector3 a, Vector3 b, float amount)
        {
            return new Vector3(
                Helpers.Lerp(a.X, b.X, amount),
                Helpers.Lerp(a.Y, b.Y, amount),
                Helpers.Lerp(a.Z, b.Z, amount));
        }

        public static Vector3 Min(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
        }

        public static Vector3 Max(Vector3 a, Vector3 b)
        {
            return new Vector3(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
        }

        public static Vector3 Clamp(Vector3 value, Vector3 min, Vector3 max)
        {
            return new Vector3(
                Helpers.Clamp(value.X, min.X, max.X),
                Helpers.Clamp(value.Y, min.Y, max.Y),
                Helpers.Clamp(value.Z, min.Z, max.Z));
        }

        public Vector3 Move(float dX, float dY, float dZ) => new Vector3(X + dX, Y + dY, Z + dZ);

        public Vector3 WithX(float x) => new Vector3(x, Y, Z);

        public Vector3 WithY(float y) => new Vector3(X, y, Z);

        public Vector3 WithZ(float z) => new Vector3(X, Y, z);

        public bool Equals(Vector3 other) => this == other;

        public override bool Equals(object obj) => obj is Vector3 other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);

        public override string ToString() => $"{X}:{Y}:{Z}";

        /// <summary>
        /// Parses the <c>x:y:z</c> form produced by <see cref="ToString"/>.
        /// </summary>
        public static Vector3 Parse(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            var parts = value.Split(':');

            if (parts.Length != 3)
                throw new FormatException($"'{value}' is not a valid Vector3. Expected the form x:y:z.");

            return new Vector3(
                float.Parse(parts[0], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(parts[1], System.Globalization.CultureInfo.InvariantCulture),
                float.Parse(parts[2], System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
