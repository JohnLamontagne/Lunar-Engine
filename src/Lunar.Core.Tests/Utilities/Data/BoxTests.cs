using Lunar.Core.Utilities.Data;
using Xunit;

namespace Lunar.Core.Tests.Utilities.Data
{
    public class BoxTests
    {
        [Fact]
        public void Constructor_normalises_corners()
        {
            var box = new Box(new Vector3(5, 5, 5), new Vector3(-1, 0, 2));

            Assert.Equal(new Vector3(-1, 0, 2), box.Min);
            Assert.Equal(new Vector3(5, 5, 5), box.Max);
            Assert.Equal(new Vector3(6, 5, 3), box.Size);
            Assert.Equal(6f, box.Width);
            Assert.Equal(5f, box.Height);
            Assert.Equal(3f, box.Depth);
            Assert.Equal(new Vector3(2, 2.5f, 3.5f), box.Center);
        }

        [Fact]
        public void FromCenterSize_and_FromFootprint()
        {
            var centered = Box.FromCenterSize(new Vector3(0, 0, 0), new Vector3(2, 4, 6));
            Assert.Equal(new Vector3(-1, -2, -3), centered.Min);
            Assert.Equal(new Vector3(1, 2, 3), centered.Max);

            var actor = Box.FromFootprint(new Vector3(10, 1, 10), width: 1f, height: 2f, depth: 1f);
            Assert.Equal(new Vector3(9.5f, 1f, 9.5f), actor.Min);
            Assert.Equal(new Vector3(10.5f, 3f, 10.5f), actor.Max);
        }

        [Fact]
        public void Contains_is_inclusive_on_edges()
        {
            var box = new Box(Vector3.Zero, new Vector3(10, 10, 10));

            Assert.True(box.Contains(new Vector3(0, 0, 0)));
            Assert.True(box.Contains(new Vector3(10, 10, 10)));
            Assert.True(box.Contains(new Vector3(5, 5, 5)));
            Assert.False(box.Contains(new Vector3(5, 10.01f, 5)));
            Assert.True(box.Contains(new Box(new Vector3(1, 1, 1), new Vector3(9, 9, 9))));
            Assert.False(box.Contains(new Box(new Vector3(1, 1, 1), new Vector3(11, 9, 9))));
        }

        [Fact]
        public void Intersects_is_strict_like_Rect()
        {
            var box = new Box(Vector3.Zero, new Vector3(10, 10, 10));

            Assert.True(box.Intersects(new Box(new Vector3(9, 9, 9), new Vector3(20, 20, 20))));
            Assert.False(box.Intersects(new Box(new Vector3(10, 0, 0), new Vector3(20, 10, 10))), "touching faces should not intersect");
            Assert.False(box.Intersects(new Box(new Vector3(0, 11, 0), new Vector3(10, 20, 10))), "boxes stacked with a gap in Y should not intersect");
            Assert.True(new Rect(0, 0, 10, 10).Intersects(new Rect(9, 9, 10, 10)));
            Assert.False(new Rect(0, 0, 10, 10).Intersects(new Rect(10, 0, 10, 10)));
        }

        [Fact]
        public void Move_MoveTo_Inflate_Union()
        {
            var box = new Box(Vector3.Zero, new Vector3(2, 2, 2));

            Assert.Equal(new Box(new Vector3(1, 1, 1), new Vector3(3, 3, 3)), box.Move(new Vector3(1)));
            Assert.Equal(new Box(new Vector3(5, 5, 5), new Vector3(7, 7, 7)), box.MoveTo(new Vector3(5)));
            Assert.Equal(new Box(new Vector3(-1, -1, -1), new Vector3(3, 3, 3)), box.Inflate(1f));
            Assert.Equal(new Box(new Vector3(-4, 0, 0), new Vector3(2, 2, 9)),
                Box.Union(box, new Box(new Vector3(-4, 0, 0), new Vector3(0, 1, 9))));
        }
    }
}
