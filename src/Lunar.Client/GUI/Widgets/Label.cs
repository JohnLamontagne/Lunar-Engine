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
using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    public class Label : IWidget
    {
        private WidgetStates _previousState;
        private MouseButtons _previousPressedButton;
        private Rectangle _area;
        private Vector2 _position;
        private string _text;
        private SpriteFont _font;
        private bool _mouseWithin;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                Vector2 textSize = Font.MeasureString(this.Text);
                _area = new Rectangle((int)_position.X, (int)_position.Y, (int)textSize.X, (int)textSize.Y);
            }
        }

        public Vector2 Origin { get; set; }

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;

                if (this.Font != null)
                {
                    Vector2 textSize = Font.MeasureString(this.Text);
                    _area = new Rectangle((int)_position.X, (int)_position.Y, (int)textSize.X, (int)textSize.Y);
                }
            }
        }

        public virtual SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;

                Vector2 textSize = Font.MeasureString(this.Text);
                _area = new Rectangle((int)_position.X, (int)_position.Y, (int)textSize.X, (int)textSize.Y);
            }
        }

        public Rectangle Area { get { return _area; } }

        public bool Visible { get; set; }

        public bool Active { get; set; }

        public Color Color { get; set; }

        public bool Selectable { get; set; }

        public string Tag { get; set; }

        public int ZOrder { get; set; }

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public event EventHandler Mouse_Left;

        public Label(SpriteFont font)
        {
            this.Text = "";
            this.Color = Color.White;
            this.Font = font;
            this.Selectable = false;
            this.Origin = Vector2.Zero;
            this.Visible = true;
        }

        public void OnMouseHover(MouseState mouseState)
        {
            if (_previousState == WidgetStates.Pressed)
            {
                this.Clicked?.Invoke(this, new WidgetClickedEventArgs(_previousPressedButton));
            }
            else
            {
                this.Mouse_Hover?.Invoke(this, new EventArgs());
            }

            _previousState = WidgetStates.Hover;
            _mouseWithin = true;
        }

        public void OnLeftMouseRelease(MouseState mouseState)
        {
        }


        public void Update(GameTime gameTime)
        {
            if (!this.Contains(Mouse.GetState().Position))
            {
                _previousState = WidgetStates.Idle;

                if (_mouseWithin)
                {
                    this.Mouse_Left?.Invoke(this, new EventArgs());
                }
            }
            else if (_previousState != WidgetStates.Pressed)
                _previousState = WidgetStates.Hover;

        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (this.Visible)
                spriteBatch.DrawString(this.Font, this.Text, this.Position, this.Color, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);

        }

        public bool Contains(Point point)
        {
            return _area.Contains(point);
        }

        public void BindTo(IWidget widget)
        {
            throw new NotImplementedException();
        }

        public void OnRightMouseDown(MouseState mouseState)
        {
            _previousState = WidgetStates.Pressed;
            _previousPressedButton = MouseButtons.Right;
        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
            _previousState = WidgetStates.Pressed;
            _previousPressedButton = MouseButtons.Left;
        }

        public void WrapText(float maxLineWidth)
        {
            string[] words = this.Text.Split(' ');
            StringBuilder sb = new StringBuilder();
            float lineWidth = 0f;
            float spaceWidth = this.Font.MeasureString(" ").X;

            foreach (string word in words)
            {
                Vector2 size = this.Font.MeasureString(word);

                if (lineWidth + size.X < maxLineWidth)
                {
                    sb.Append(word + " ");
                    lineWidth += size.X + spaceWidth;
                }
                else
                {
                    sb.Append("\n" + word + " ");
                    lineWidth = size.X + spaceWidth;
                }
            }

            this.Text = sb.ToString();
        }

        public bool Selected { get; set; }
    }
}