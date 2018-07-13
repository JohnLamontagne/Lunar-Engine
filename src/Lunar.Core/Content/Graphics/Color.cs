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
namespace Lunar.Core.Content.Graphics
{
    public struct Color
    {
        private byte _r;
        private byte _g;
        private byte _b;
        private byte _a;

        public byte R
        {
            get
            {
                return _r;
            }
        }

        public byte G
        {
            get
            {
                return _g;
            }
        }

        public byte B
        {
            get
            {
                return _b;
            }
        }

        public byte A
        {
            get
            {
                return _a;
            }
        }

        public Color(byte r, byte g, byte b, byte a)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = a;
        }

        public Color(byte r, byte g, byte b)
        {
            _r = r;
            _g = g;
            _b = b;
            _a = 255;
        }

        public static Color FromString(string value)
        {
            value = value.TrimStart('{').TrimEnd('}');

            var components = value.Split(' ');

            components[0] = components[0].Remove(0, 2);
            components[1] = components[1].Remove(0, 2);
            components[2] = components[2].Remove(0, 2);
            components[3] = components[3].Remove(0, 2);

            return new Color(byte.Parse(components[0]), byte.Parse(components[1]), byte.Parse(components[2]), byte.Parse(components[3]));
        }

        public static Color White => new Color(255, 255, 255);

        public static Color Transparent => new Color(255, 255, 255, 0);

        public static Color Red => new Color(255, 0, 0);

        public static Color Green => new Color(0, 255, 0);

        public static Color Blue => new Color(0, 0, 255);

        public static Color Black => new Color(0, 0, 0);
    }
}