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
using Lunar.Core.Utilities.Data;

namespace Lunar.Core.Content.Graphics
{
    public class Transform
    {
        private Vector _position;
        private Rect _rect;
        private Color _color;

        public Vector Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        public Rect Rect
        {
            get { return _rect; }
            set
            {
                _rect = value;
            }
        }

        public Color Color { get { return _color; } set { _color = value; } }

        public Transform(Vector size)
        {
            this.Rect = new Rect(0, 0, size.X, size.Y);
            this.Position = new Vector();
            this.Color = new Color(255, 255, 255);
        }

        public Transform()
        {
            this.Position = new Vector();
            this.Color = new Color(255, 255, 255);
        }
    }
}