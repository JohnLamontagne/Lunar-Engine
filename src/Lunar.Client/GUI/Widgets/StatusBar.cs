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
    public class StatusBar : IWidget
    {
        private float _value;
        private Rectangle _fillBounds;
        private Label _label;
        private Vector2 _textOffset;
        private Vector2 _position;
        private bool _visible;
        private int _zOrder;

        private Rectangle _fillSpriteDest;

        public string Tag { get; set; }

        public int ZOrder
        {
            get
            {
                return _zOrder;
            }
            set
            {
                _zOrder = value;

                _label.ZOrder = this.ZOrder + 1;
            }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _label.Visible = value;
            }
        }

        public bool Active { get; set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _label.Position = new Vector2(this.Position.X + _textOffset.X, this.Position.Y + _textOffset.Y);
            }
        }

        public Vector2 Origin { get; set; }

        public Vector2 TextOffset
        {
            get { return _textOffset; }
            set
            {
                _textOffset = value;

                _label.Position = new Vector2(this.Position.X + _textOffset.X, this.Position.Y + _textOffset.Y);
            }
        }

        public Texture2D BackSprite { get; set; }
        public Texture2D FillSprite { get; set; }

        public string Text
        {
            get { return _label.Text; }
            set { _label.Text = value; }
        }

        public Color ForeColor { get { return _label.Color; } set { _label.Color = value; } }

        public float Value
        {
            get { return _value; ; }
            set
            {
                _value = MathHelper.Clamp(value, 0, 100);

                _fillSpriteDest = new Rectangle(0, 0, (int)((_fillBounds.Width / 100f) * value), _fillBounds.Height);
            }
        }

        public SpriteFont Font
        {
            get => _label.Font;
            set => _label.Font = value;
        }

        public bool Selectable { get; set; }

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public StatusBar(Texture2D sprite, Texture2D fillSprite, Rectangle fillBounds, SpriteFont font)
        {
            _fillBounds = fillBounds;
            _label = new Label(font) {
                Position = new Vector2(this.Position.X, this.Position.Y),
                ZOrder = this.ZOrder + 1
            };

            this.BackSprite = sprite;
            this.FillSprite = fillSprite;

            this.Value = 0;
            this.TextOffset = Vector2.Zero;
            this.ForeColor = Color.White;
            this.Selectable = true;
            this.Origin = Vector2.Zero;
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (!this.Visible)
                return;

            spriteBatch.Draw(this.BackSprite, this.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);

            spriteBatch.Draw(this.FillSprite, new Vector2(this.Position.X + _fillBounds.Left, this.Position.Y + _fillBounds.Top), _fillSpriteDest, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, ((float)this.ZOrder / widgetCount) + .01f);

            _label.Draw(spriteBatch, widgetCount);
        }

        public bool Contains(Point point)
        {
            return false;
        }

        public void BindTo(IWidget widget)
        {
        }

        public void OnMouseHover(MouseState mouseState)
        {
        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
        }

 

        public void OnRightMouseDown(MouseState mouseState)
        {
        }

    }
}