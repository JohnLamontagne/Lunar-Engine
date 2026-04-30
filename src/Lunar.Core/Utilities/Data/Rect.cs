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

namespace Lunar.Core.Utilities.Data
{
    public struct Rect
    {
        public int X { get; }

        public int Y { get; }

        public int Width { get; }

        public int Height { get; }

        public int Left => X;
        public int Top => Y;
        public int Right => X + Width;
        public int Bottom => Y + Height;

        public Rect(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public Rect(float x, float y, float width, float height)
            : this((int)x, (int)y, (int)width, (int)height) { }

        public bool Contains(Vector point)
            => point.X >= X && point.X <= Right && point.Y >= Y && point.Y <= Bottom;

        public bool Contains(float pointX, float pointY)
            => pointX >= X && pointX <= Right && pointY >= Y && pointY <= Bottom;

        public bool Intersects(Rect other)
            => Left < other.Right && Right > other.Left && Top < other.Bottom && Bottom > other.Top;

        public Rect Move(float dX, float dY) => new Rect(X + dX, Y + dY, Width, Height);

        public Rect MoveTo(float x, float y) => new Rect(x, y, Width, Height);

        public override string ToString() => $"[{X}, {Y}, {Width}, {Height}]";
    }
}
