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

namespace Lunar.Core.Utilities.Data
{
    /// <summary>
    /// Axis-aligned bounding box in world units. The 3D counterpart of <see cref="Rect"/>,
    /// used for actor collision volumes, zone extents and interest-management cells.
    /// Both corners are inclusive for containment tests; intersection is strict (touching boxes do not intersect),
    /// matching the behaviour of <see cref="Rect.Intersects"/>.
    /// </summary>
    public readonly struct Box : IEquatable<Box>
    {
        public Vector3 Min { get; }

        public Vector3 Max { get; }

        public Vector3 Size => Max - Min;

        public Vector3 Center => Min + (this.Size * 0.5f);

        public float Width => Max.X - Min.X;

        public float Height => Max.Y - Min.Y;

        public float Depth => Max.Z - Min.Z;

        /// <summary>
        /// Creates a box from two arbitrary corners; the corners are normalised so that
        /// <see cref="Min"/> is component-wise less than or equal to <see cref="Max"/>.
        /// </summary>
        public Box(Vector3 a, Vector3 b)
        {
            Min = Vector3.Min(a, b);
            Max = Vector3.Max(a, b);
        }

        public static Box Empty => new Box(Vector3.Zero, Vector3.Zero);

        public static Box FromCenterSize(Vector3 center, Vector3 size)
        {
            var half = size * 0.5f;
            return new Box(center - half, center + half);
        }

        /// <summary>
        /// Builds a box standing on the ground plane at <paramref name="footPosition"/> (the actor's feet),
        /// extending <paramref name="height"/> upwards and centred horizontally. This is the natural
        /// collision volume for a character.
        /// </summary>
        public static Box FromFootprint(Vector3 footPosition, float width, float height, float depth)
        {
            var min = new Vector3(footPosition.X - width * 0.5f, footPosition.Y, footPosition.Z - depth * 0.5f);
            var max = new Vector3(footPosition.X + width * 0.5f, footPosition.Y + height, footPosition.Z + depth * 0.5f);
            return new Box(min, max);
        }

        public bool Contains(Vector3 point)
        {
            return point.X >= Min.X && point.X <= Max.X
                && point.Y >= Min.Y && point.Y <= Max.Y
                && point.Z >= Min.Z && point.Z <= Max.Z;
        }

        public bool Contains(Box other)
        {
            return this.Contains(other.Min) && this.Contains(other.Max);
        }

        public bool Intersects(Box other)
        {
            return Min.X < other.Max.X && Max.X > other.Min.X
                && Min.Y < other.Max.Y && Max.Y > other.Min.Y
                && Min.Z < other.Max.Z && Max.Z > other.Min.Z;
        }

        public Box Move(Vector3 delta) => new Box(Min + delta, Max + delta);

        public Box MoveTo(Vector3 min) => new Box(min, min + this.Size);

        /// <summary>
        /// Grows the box uniformly by <paramref name="amount"/> on every side.
        /// </summary>
        public Box Inflate(float amount)
        {
            var delta = new Vector3(amount);
            return new Box(Min - delta, Max + delta);
        }

        public static Box Union(Box a, Box b) => new Box(Vector3.Min(a.Min, b.Min), Vector3.Max(a.Max, b.Max));

        public bool Equals(Box other) => Min == other.Min && Max == other.Max;

        public override bool Equals(object obj) => obj is Box other && this.Equals(other);

        public override int GetHashCode() => HashCode.Combine(Min, Max);

        public static bool operator ==(Box a, Box b) => a.Equals(b);

        public static bool operator !=(Box a, Box b) => !a.Equals(b);

        public override string ToString() => $"[{Min} -> {Max}]";
    }
}
