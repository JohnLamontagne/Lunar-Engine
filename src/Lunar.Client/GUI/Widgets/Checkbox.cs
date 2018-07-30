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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    public class Checkbox : IWidget
    {
        private Rectangle _checkBoxArea;
        private Vector2 _position;
        private WidgetStates _state;

        public bool Visible { get; set; }
        public bool Active { get; set; }

        public bool Value { get; set; }

        public string Tag { get; set; }

        public int ZOrder { get; set; }

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                _checkBoxArea = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.CheckedSprite.Width, this.CheckedSprite.Height);
            }
        }

        public Vector2 Origin { get; set; }

        public Texture2D CheckedSprite { get; set; }

        public Texture2D UncheckedSprite { get; set; }

        public bool Selectable { get; set; }

        public Checkbox(Texture2D checkedSprite, Texture2D uncheckedSprite)
        {
            this.CheckedSprite = checkedSprite;
            this.UncheckedSprite = uncheckedSprite;
            this.Selectable = true;
            this.Origin = Vector2.Zero;

            _checkBoxArea = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.CheckedSprite.Width, this.CheckedSprite.Height);
        }

        public void Update(GameTime gameTime)
        {
            if (!this.Visible) return;

            if (!this.Contains(Mouse.GetState().Position))
                _state = WidgetStates.Idle;
            else if (_state != WidgetStates.Pressed && _state != WidgetStates.Hover)
                _state = WidgetStates.Hover;
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (!this.Visible)
                return;

            if (this.Value)
                spriteBatch.Draw(this.CheckedSprite, this.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);
            else
                spriteBatch.Draw(this.UncheckedSprite, this.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);
        }

        public bool Contains(Point point)
        {
            return _checkBoxArea.Contains(point);
        }

        public void BindTo(IWidget widget)
        {
        }

        public void OnMouseHover(MouseState mouseState)
        {
            if (!this.Visible)
                return;

            if (_state == WidgetStates.Pressed)
            {
                this.Value = !this.Value;

                this.Clicked?.Invoke(this, new WidgetClickedEventArgs(MouseButtons.Left));

                _state = WidgetStates.Hover;
            }
        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
            if (!this.Visible)
                return;

            _state = WidgetStates.Pressed;
        }

        public void OnRightMouseDown(MouseState mouseState)
        {
        }

        public bool Selected { get; set; }
    }
}