/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
 */

using Microsoft.Xna.Framework;

namespace Lunar.Core.Utilities.Data
{
    /// <summary>
    /// Exposes functionality of XNA Rectangle to the server while minimizing exposure of other unneeded bloat
    /// </summary>
    public struct Rect
    {
        private Rectangle _rectangle;

        public int Left => _rectangle.Left;

        public int Top => _rectangle.Top;

        public int Width => _rectangle.Width;

        public int Height => _rectangle.Height;

        public Rect(int left, int top, int width, int height)
        { 
            _rectangle = new Rectangle(left, top, width, height);
        }

        public Rect(float left, float top, float width, float height)
        {
            _rectangle = new Rectangle((int)left, (int)top, (int)width, (int)height);
        }

        public override string ToString()
        {
            return $"[{this.Left}, {this.Top}, {this.Width}, {this.Height}]";
        }

        public bool Contains(Vector point)
        {
            return _rectangle.Contains(point.X, point.Y);
        }

        public bool Intersects(Rect rect)
        {
            return _rectangle.Intersects(new Rectangle(rect.Left, rect.Top, rect.Width, rect.Height));
        }
    }
}