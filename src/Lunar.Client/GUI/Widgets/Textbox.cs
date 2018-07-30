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
using Lunar.Client.Utilities.Input;

namespace Lunar.Client.GUI.Widgets
{
    public class Textbox : IWidget
    {
        private string _text;
        private uint _charSize;
        private SpriteFont _font;
        private Vector2 _textPosition;
        private Vector2 _position;
        private Rectangle _area;
        private Texture2D _sprite;
        private long _nextCursorBlinkTime;
        private bool _cursorVisible;
        private Vector2 _textOffset;
        private string _mask;
        private string _displayText;
        private Vector2 _scale;
        private KeyboardState _prevKeyState;
        private long _nextInputProcessTime;

        // For widget binding.
        private IWidget _boundWidget;

        private Vector2 _lastBoundWidgetPos;
        // end for widget binding

        public bool Visible { get; set; }

        public bool Active { get; set; }

        public event EventHandler<EventArgs> TabPressed;

        public event EventHandler ReturnPressed;

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Text_Entered;

        public event EventHandler Mouse_Hover;

        public bool Selected { get; set; }

        public bool Selectable { get; set; }

        public string Tag { get; set; }

        public int ZOrder { get; set; }

        public Texture2D Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }

        public Vector2 Scale
        {
            get => _scale;
            set => _scale = value;
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;

                _textPosition = new Vector2(_position.X + _textOffset.X, this.Position.Y + ((this._sprite.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2));

                _area = new Rectangle((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);
            }
        }

        public Vector2 Origin { get; set; }

        public string Mask
        {
            get => _mask;
            set
            {
                _mask = value;

                _displayText = string.Empty;

                if (!string.IsNullOrEmpty(_mask) && !string.IsNullOrEmpty(this.Text))
                {
                    foreach (var character in this.Text)
                    {
                        _displayText += _mask;
                    }
                }
                else
                {
                    _displayText = this.Text;
                }
            }
        }

        public SpriteFont Font { get => _font;
            set => _font = value;
        }

        public Color ForeColor { get; set; }

        public string Text
        {
            get => _text;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _text = value;
                    _displayText = value;
                    return;
                }

                string rawText = value;
                string filteredText = "";
                foreach (var c in rawText)
                {
                    if (_font.Characters.Contains(c))
                        filteredText += c;
                }
                _text = filteredText;

                _displayText = string.Empty;

                if (!string.IsNullOrEmpty(_mask))
                {
                    foreach (var character in this.Text)
                    {
                        _displayText += _mask;
                    }
                }
                else
                {
                    _displayText = this.Text;
                }

                while (_font.MeasureString(_text).X > _area.Width)
                {
                    //to ensure that text cannot be larger than the box
                    _text = _text.Substring(0, _text.Length - 1);
                }

                _textPosition = new Vector2(_position.X + _textOffset.X, this.Position.Y + ((this._sprite.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2f) + _textOffset.Y);
            }
        }


        public Textbox(Texture2D sprite, SpriteFont font, Vector2 textOffset, uint charSize = 15)
        {
            this.Text = "";
            this.Sprite = sprite;
            _font = font;
            _charSize = charSize;

            this.ForeColor = Color.White;
            this.Scale = new Vector2(1, 1);
            this.Origin = Vector2.Zero;
            this.Visible = true;

            _textOffset = textOffset;
            _area = new Rectangle((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);

            EventInput.CharEntered += EventInput_CharEntered;
        }

        public Textbox(Texture2D sprite, string caption, SpriteFont font, Vector2 textOffset, uint charSize = 15)
        {
            this.Text = "";
            _sprite = sprite;
            _text = caption;
            _font = font;
            _charSize = charSize;

            this.ForeColor = Color.White;
            this.Scale = new Vector2(1, 1);
            this.Selectable = true;
            this.Origin = Vector2.Zero;

            _textOffset = textOffset;
            _area = new Rectangle((int)_position.X, (int)_position.Y, _sprite.Width, _sprite.Height);
            EventInput.CharEntered += EventInput_CharEntered;
        }

        public void OnMouseHover(MouseState mouseState)
        {
        }


        public void OnLeftMouseDown(MouseState mouseState)
        {
        }

        private void ProcessSpecialInput(GameTime gameTime)
        {
            if (!this.Active)
                return;

            if (_nextInputProcessTime > gameTime.TotalGameTime.TotalMilliseconds)
            {
                return;
            }

            KeyboardState keyState = Keyboard.GetState();

            var pressedKeys = keyState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (_prevKeyState.IsKeyUp(key))
                {
                    if (keyState.IsKeyDown(Keys.Back) && _prevKeyState.IsKeyUp(Keys.Back))
                    {
                        if (this.Text.Length > 0)
                            this.Text = this.Text.Substring(0, Text.Length - 1);

                        _nextInputProcessTime = (long)gameTime.TotalGameTime.TotalMilliseconds + 100;
                        return;
                    }
                    else if (keyState.IsKeyDown(Keys.Enter) && _prevKeyState.IsKeyUp(Keys.Enter))
                    {
                        this.ReturnPressed?.Invoke(this, new EventArgs());
                        _nextInputProcessTime = (long)gameTime.TotalGameTime.TotalMilliseconds + 100;
                        return;
                    }
                }
            }

            _prevKeyState = keyState;
        }

        public void Update(GameTime gameTime)
        {
            if (!this.Visible) return;

            this.ProcessSpecialInput(gameTime);

            var mouseState = Mouse.GetState();

            if (_area.Contains(mouseState.Position))
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    this.Active = true;
                }
            }
            else
            {
                if (mouseState.LeftButton == ButtonState.Pressed)
                {
                    this.Active = false;
                }
            }

            if (this.Active)
            {
                if (_nextCursorBlinkTime < gameTime.ElapsedGameTime.Milliseconds)
                {
                    _cursorVisible = !_cursorVisible;
                    _nextCursorBlinkTime = gameTime.ElapsedGameTime.Milliseconds + 500;
                }
            }

            if (_boundWidget != null)
            {
                if (_lastBoundWidgetPos != _boundWidget.Position)
                {
                    var relX = this.Position.X - _lastBoundWidgetPos.X;
                    var relY = this.Position.Y - _lastBoundWidgetPos.Y;

                    this.Position = new Vector2(_boundWidget.Position.X + relX, _boundWidget.Position.Y + relY);
                    _lastBoundWidgetPos = _boundWidget.Position;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (!this.Visible) return;

            spriteBatch.Draw(this._sprite, this.Position + this.Origin, null, Color.White, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, (float)this.ZOrder / widgetCount);
            spriteBatch.DrawString(_font, _displayText, _textPosition + this.Origin, this.ForeColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, ((float)this.ZOrder / widgetCount) + .01f);
        }

        public bool Contains(Point point)
        {
            return _area.Contains(point);
        }

        public void BindTo(IWidget widget)
        {
            _boundWidget = widget;
            _lastBoundWidgetPos = _boundWidget.Position;
        }

        private void EventInput_CharEntered(object sender, CharacterEventArgs e)
        {
            if (!this.Active)
                return;

            this.Text += e.Character;

            this.Text_Entered?.Invoke(this, new EventArgs());
        }

        public void OnRightMouseDown(MouseState mouseState)
        {
        }

    }
}