using System;
using System.IO;
using SkiaSharp;

namespace Lunar.E2E.Tests.Harness
{
    /// <summary>
    /// A captured client frame with the image-analysis primitives the tests assert on. Deliberately
    /// simple: region statistics and whole-frame similarity, which are robust to font hinting and
    /// driver differences between a developer machine and the CI container.
    /// </summary>
    public sealed class Frame : IDisposable
    {
        public SKBitmap Bitmap { get; }
        public int Width => Bitmap.Width;
        public int Height => Bitmap.Height;

        private Frame(SKBitmap bitmap) => Bitmap = bitmap;

        public static Frame FromPng(byte[] png)
        {
            var bitmap = SKBitmap.Decode(png) ?? throw new InvalidDataException("Screenshot bytes were not a decodable image.");
            return new Frame(bitmap);
        }

        public static Frame Load(string path) => FromPng(File.ReadAllBytes(path));

        public void Save(string path)
        {
            using var image = SKImage.FromBitmap(Bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var file = File.OpenWrite(path);
            data.SaveTo(file);
        }

        /// <summary>Average colour over a rectangle given in pixels.</summary>
        public SKColor AverageColor(SKRectI rect)
        {
            rect.Intersect(new SKRectI(0, 0, Width, Height));
            long r = 0, g = 0, b = 0, n = 0;
            for (int y = rect.Top; y < rect.Bottom; y++)
                for (int x = rect.Left; x < rect.Right; x++)
                {
                    var c = Bitmap.GetPixel(x, y);
                    r += c.Red; g += c.Green; b += c.Blue; n++;
                }
            if (n == 0) return SKColors.Black;
            return new SKColor((byte)(r / n), (byte)(g / n), (byte)(b / n));
        }

        /// <summary>Fraction of pixels whose brightness exceeds <paramref name="threshold"/> (0-255).</summary>
        public double LitFraction(int threshold = 16)
        {
            long lit = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var c = Bitmap.GetPixel(x, y);
                    if (Math.Max(c.Red, Math.Max(c.Green, c.Blue)) > threshold)
                        lit++;
                }
            return (double)lit / (Width * (double)Height);
        }

        /// <summary>Number of distinct colours after quantising to 4 bits per channel; a proxy for "something is drawn".</summary>
        public int DistinctColorBuckets()
        {
            var seen = new System.Collections.Generic.HashSet<int>();
            for (int y = 0; y < Height; y += 2)
                for (int x = 0; x < Width; x += 2)
                {
                    var c = Bitmap.GetPixel(x, y);
                    seen.Add((c.Red >> 4) << 8 | (c.Green >> 4) << 4 | (c.Blue >> 4));
                }
            return seen.Count;
        }

        /// <summary>
        /// Mean absolute per-channel difference (0-255) against another frame of the same size.
        /// Small values mean visually identical; tolerate a few units for anti-aliasing differences.
        /// </summary>
        public double MeanAbsoluteDifference(Frame other)
        {
            if (other.Width != Width || other.Height != Height)
                throw new ArgumentException($"Frame sizes differ: {Width}x{Height} vs {other.Width}x{other.Height}");
            long total = 0;
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                {
                    var a = Bitmap.GetPixel(x, y);
                    var b = other.Bitmap.GetPixel(x, y);
                    total += Math.Abs(a.Red - b.Red) + Math.Abs(a.Green - b.Green) + Math.Abs(a.Blue - b.Blue);
                }
            return total / (3.0 * Width * Height);
        }

        public void Dispose() => Bitmap.Dispose();
    }
}
