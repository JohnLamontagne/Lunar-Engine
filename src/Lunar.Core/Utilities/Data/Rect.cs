/** Copyright 2018 John Lamontagne https://www.mmorpgcreation.com

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