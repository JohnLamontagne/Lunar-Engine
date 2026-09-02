using System;
using Lunar.Core.Utilities.Data;
using Xunit;

namespace Lunar.Core.Tests.Utilities.Data
{
    public class Vector3Tests
    {
        [Fact]
        public void Arithmetic_operators_are_componentwise()
        {
            var a = new Vector3(1, 2, 3);
            var b = new Vector3(4, 5, 6);

            Assert.Equal(new Vector3(5, 7, 9), a + b);
            Assert.Equal(new Vector3(-3, -3, -3), a - b);
            Assert.Equal(new Vector3(4, 10, 18), a * b);
            Assert.Equal(new Vector3(2, 4, 6), a * 2f);
            Assert.Equal(new Vector3(2, 4, 6), 2f * a);
            Assert.Equal(new Vector3(0.5f, 1f, 1.5f), a / 2f);
            Assert.Equal(new Vector3(-1, -2, -3), -a);
        }

        [Fact]
        public void Equality_uses_tolerance()
        {
            var a = new Vector3(1, 2, 3);
            var almost = new Vector3(1 + Vector3.COMPARISON_TOLERANCE / 2f, 2, 3);
            var different = new Vector3(1.001f, 2, 3);

            Assert.True(a == almost);
            Assert.True(a.Equals((object)almost));
            Assert.False(a == different);
            Assert.True(a != different);
        }

        [Fact]
        public void Length_dot_and_cross_follow_the_right_hand_rule()
        {
            Assert.Equal(5f, new Vector3(3, 4, 0).Length, 5);
            Assert.Equal(0f, Vector3.Dot(Vector3.UnitX, Vector3.UnitY), 5);
            Assert.Equal(Vector3.UnitZ, Vector3.Cross(Vector3.UnitX, Vector3.UnitY));
            Assert.Equal(Vector3.UnitX, Vector3.Cross(Vector3.UnitY, Vector3.UnitZ));
            Assert.Equal(Vector3.Forward, Vector3.Cross(Vector3.UnitY, Vector3.UnitX));
        }

        [Fact]
        public void Normalize_returns_unit_length_and_never_NaN()
        {
            var n = Vector3.Normalize(new Vector3(0, 10, 0));
            Assert.Equal(Vector3.Up, n);
            Assert.Equal(1f, n.Length, 5);

            Assert.Equal(Vector3.Zero, Vector3.Normalize(Vector3.Zero));
        }

        [Fact]
        public void Distance_and_planar_distance_differ_by_height()
        {
            var ground = new Vector3(0, 0, 0);
            var elevated = new Vector3(3, 10, 4);

            Assert.Equal(5f, Vector3.PlanarDistance(ground, elevated), 5);
            Assert.Equal((float)Math.Sqrt(125), Vector3.Distance(ground, elevated), 4);
            Assert.Equal(125f, Vector3.DistanceSquared(ground, elevated), 4);
        }

        [Fact]
        public void Lerp_min_max_and_clamp()
        {
            var a = new Vector3(0, 0, 0);
            var b = new Vector3(10, -10, 20);

            Assert.Equal(new Vector3(5, -5, 10), Vector3.Lerp(a, b, 0.5f));
            Assert.Equal(new Vector3(0, -10, 0), Vector3.Min(a, b));
            Assert.Equal(new Vector3(10, 0, 20), Vector3.Max(a, b));
            Assert.Equal(new Vector3(1, -1, 1), Vector3.Clamp(b, new Vector3(-1), new Vector3(1)));
        }

        [Fact]
        public void Ground_plane_round_trip_maps_2d_Y_to_3d_Z()
        {
            var planar = new Vector(32, 64);
            var lifted = Vector3.FromGroundPlane(planar, 5f);

            Assert.Equal(new Vector3(32, 5, 64), lifted);
            Assert.True(planar == lifted.ToGroundPlane());
        }

        [Fact]
        public void ToString_and_Parse_round_trip()
        {
            var v = new Vector3(1.5f, -2f, 3.25f);
            Assert.Equal("1.5:-2:3.25", v.ToString());
            Assert.Equal(v, Vector3.Parse(v.ToString()));
            Assert.Throws<FormatException>(() => Vector3.Parse("1:2"));
            Assert.Throws<ArgumentNullException>(() => Vector3.Parse(null));
        }

        [Fact]
        public void With_helpers_replace_a_single_component()
        {
            var v = new Vector3(1, 2, 3);
            Assert.Equal(new Vector3(9, 2, 3), v.WithX(9));
            Assert.Equal(new Vector3(1, 9, 3), v.WithY(9));
            Assert.Equal(new Vector3(1, 2, 9), v.WithZ(9));
            Assert.Equal(new Vector3(2, 4, 6), v.Move(1, 2, 3));
        }
    }
}
