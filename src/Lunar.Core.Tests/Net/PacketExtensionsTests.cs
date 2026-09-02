using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Xunit;
using Color = Lunar.Core.Content.Graphics.Color;

namespace Lunar.Core.Tests.Net
{
    /// <summary>
    /// The wire format is positional with no field tags, so every composite type must read back
    /// exactly the bytes it wrote. These tests pin the byte size and round-trip of each helper.
    /// </summary>
    public class PacketExtensionsTests
    {
        private static Packet RoundTrip(Packet written)
        {
            return new Packet(written.ToArray());
        }

        [Fact]
        public void Vector_round_trips_as_two_floats()
        {
            var packet = new Packet().Write(new Vector(1.5f, -2f));
            Assert.Equal(8, packet.LengthBytes);

            var read = RoundTrip(packet).ReadVector();
            Assert.True(new Vector(1.5f, -2f) == read);
        }

        [Fact]
        public void Rect_round_trips_as_four_ints()
        {
            var packet = new Packet().Write(new Rect(1, 2, 3, 4));
            Assert.Equal(16, packet.LengthBytes);

            var read = RoundTrip(packet).ReadRect();
            Assert.Equal(1, read.X);
            Assert.Equal(2, read.Y);
            Assert.Equal(3, read.Width);
            Assert.Equal(4, read.Height);
        }

        [Fact]
        public void Color_round_trips_as_four_bytes()
        {
            var packet = new Packet().Write(new Color(10, 20, 30, 40));
            Assert.Equal(4, packet.LengthBytes);

            var read = RoundTrip(packet).ReadColor();
            Assert.Equal((byte)10, read.R);
            Assert.Equal((byte)20, read.G);
            Assert.Equal((byte)30, read.B);
            Assert.Equal((byte)40, read.A);
        }

        [Fact]
        public void Vector3_round_trips_as_three_floats()
        {
            var value = new Vector3(1.25f, -7f, 300.5f);
            var packet = new Packet().Write(value);
            Assert.Equal(12, packet.LengthBytes);

            Assert.Equal(value, RoundTrip(packet).ReadVector3());
        }

        [Fact]
        public void Box_round_trips_as_six_floats()
        {
            var value = Box.FromFootprint(new Vector3(10, 2, -5), 1f, 2f, 1f);
            var packet = new Packet().Write(value);
            Assert.Equal(24, packet.LengthBytes);

            Assert.Equal(value, RoundTrip(packet).ReadBox());
        }

        [Fact]
        public void Mixed_2d_and_3d_fields_read_back_in_order()
        {
            var packet = new Packet()
                .Write("actor")
                .Write(new Vector(1, 2))
                .Write(new Vector3(3, 4, 5))
                .Write((byte)7)
                .Write(new Box(Vector3.Zero, Vector3.One));

            var read = RoundTrip(packet);
            Assert.Equal("actor", read.ReadString());
            Assert.True(new Vector(1, 2) == read.ReadVector());
            Assert.Equal(new Vector3(3, 4, 5), read.ReadVector3());
            Assert.Equal((byte)7, read.ReadByte());
            Assert.Equal(new Box(Vector3.Zero, Vector3.One), read.ReadBox());
            Assert.Equal(read.LengthBytes, read.Position);
        }
    }
}
