/* Copyright (C) 2015 John Lamontagne - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by John Lamontagne <jdlamont@asu.edu>.
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