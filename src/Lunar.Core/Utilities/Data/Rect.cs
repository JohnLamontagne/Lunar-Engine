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

using Microsoft.Xna.Framework;
using XNARectangle = Microsoft.Xna.Framework.Rectangle;

namespace Lunar.Core.Utilities.Data
{
    /// <summary>
    /// Exposes functionality of XNA Rectangle to the server while minimizing exposure of other unneeded bloat
    /// </summary>
    public struct Rect
    {
        private XNARectangle _rectangle;

        public int X => _rectangle.Left;

        public int Y => _rectangle.Top;

        public int Width => _rectangle.Width;

        public int Height => _rectangle.Height;

        public Rect(int left, int top, int width, int height)
        {
            _rectangle = new XNARectangle(left, top, width, height);
        }

        public Rect(float left, float top, float width, float height)
        {
            _rectangle = new XNARectangle((int)left, (int)top, (int)width, (int)height);
        }

        public override string ToString()
        {
            return $"[{this.X}, {this.Y}, {this.Width}, {this.Height}]";
        }

        public bool Contains(Vector point)
        {
            return _rectangle.Contains(point.X, point.Y);
        }

        public bool Intersects(Rect rect)
        {
            return _rectangle.Intersects(rect);
        }

        public Rect Move(float dX, float dY)
        {
            return new Rect(this.X + dX, this.Y + dY, this.Width, this.Height);
        }

        public Rect MoveTo(float x, float y)
        {
            return new Rect(x, y, this.Width, this.Height);
        }

        public static implicit operator Rect(XNARectangle rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static implicit operator XNARectangle(Rect rect)
        {
            return new XNARectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }
    }
}