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
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Lunar.Core.Utilities.Data;

namespace Lunar.Client.GUI.Widgets
{
    public class Button : IWidget
    {
        #region Fields

        private string _text;
        private SpriteFont _font;
        private uint _charSize;
        private Vector2 _textPosition;
        private readonly Texture2D[] _textures;
        private WidgetStates _state;
        private Rectangle _buttonArea;
        private Vector2 _position;

        #endregion Fields

        #region Properties

        public int ZOrder { get; set; }

        public WidgetStates State { get { return _state; } set { _state = value; } }

        public bool Visible { get; set; }

        public bool Active { get; set; }

        public bool Selectable { get; set; }

        public string Tag { get; set; }

        public Rectangle Area => _buttonArea;

        public Vector2 TrueDimensions
        {
            get
            {
                if (this.IdleTexture != null)
                    return new Vector2(this.IdleTexture.Width, this.IdleTexture.Height);
                else
                    return new Vector2(0, 0);
            }
        }

        public uint CharSize
        {
            get { return _charSize; }
            set
            {
                _charSize = value;

                if (this.IdleTexture != null && this.Font != null)
                {
                    float x = this.Position.X + ((this.IdleTexture.Width) / 2f) - (this.Font.MeasureString(_text).X / 2f);
                    float y = this.Position.Y + ((this.IdleTexture.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2f);

                    _textPosition = new Vector2(x, y);
                }
            }
        }

        public SpriteFont Font
        {
            get { return _font; }
            set
            {
                _font = value;

                if (this.IdleTexture != null && this.Font != null)
                {
                    float x = this.Position.X + ((this.IdleTexture.Width) / 2f) - (this.Font.MeasureString(_text).X / 2f);
                    float y = this.Position.Y + ((this.IdleTexture.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2f);

                    _textPosition = new Vector2(x, y);
                }
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;

                if (this.IdleTexture != null && this.Font != null)
                {
                    float x = this.Position.X + ((this.IdleTexture.Width) / 2f) - (this.Font.MeasureString(_text).X / 2f);
                    float y = this.Position.Y + ((this.IdleTexture.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2f);

                    _textPosition = new Vector2(x, y);
                }
            }
        }

        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                if (this.IdleTexture != null && this.Font != null)
                {
                    _buttonArea = new Rectangle((int)_position.X, (int)_position.Y, (int)(_textures[0].Width), (int)(_textures[0].Height));

                    float x = this.Position.X + ((this.IdleTexture.Width) / 2f) - (this.Font.MeasureString(_text).X / 2f);
                    float y = this.Position.Y + ((this.IdleTexture.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2f);

                    _textPosition = new Vector2(x, y);
                }
            }
        }

        public Vector2 Origin { get; set; }

        public Color ForeColor { get; set; }

        public Texture2D IdleTexture
        {
            get
            {
                return _textures[0];
            }
            set
            {
                _textures[0] = value;

                if (this.IdleTexture != null && this.Font != null)
                {
                    _buttonArea = new Rectangle((int)_position.X, (int)_position.Y, (int)(_textures[0].Width), (int)(_textures[0].Height));

                    float x = this.Position.X + ((this.IdleTexture.Width) / 2f) - (this.Font.MeasureString(_text).X / 2f);
                    float y = this.Position.Y + ((this.IdleTexture.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2f);

                    _textPosition = new Vector2(x, y);
                }
            }
        }

        public Texture2D HoverSprite
        {
            get
            {
                return _textures[1];
            }

            set
            {
                _textures[1] = value;
            }
        }

        public Texture2D MouseDownTexture
        {
            get
            {
                return _textures[2];
            }

            set
            {
                _textures[2] = value;
            }
        }

        public SoundEffect ButtonDown_Sound { get; set; }

        public SoundEffect ButtonHover_Sound { get; set; }

        #endregion Properties

        #region Event Handlers

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        #endregion Event Handlers

        /// <summary>
        /// Only used for loading the button from XML.
        /// </summary>
        internal Button()
        {
            _textures = new Texture2D[3];
            _position = new Vector2();
            _state = WidgetStates.Idle;
            this.ForeColor = Color.Black;
            this.Selectable = false;
            this.Visible = true;
            this.ZOrder = 1;
            this.Origin = Vector2.Zero;
        }

        public Button(Texture2D idleTexture, string text, SpriteFont font, uint charSize = 15)
            : this()
        {
            _textures[0] = idleTexture;
            _font = font;
            _charSize = charSize;
            _text = text;

            float x = this.Position.X + ((this.IdleTexture.Width) / 2f) - (this.Font.MeasureString(_text).X / 2);
            float y = this.Position.Y + ((this.IdleTexture.Height) / 2f) - (this.Font.MeasureString(_text).Y / 2);

            _textPosition = new Vector2(x, y);
        }

        public bool Contains(Point point)
        {
            return _buttonArea.Contains(point);
        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
            if (this.State != WidgetStates.Pressed)
            {
                this.State = WidgetStates.Pressed;

                this.ButtonDown_Sound?.Play();
            }
        }

        public void OnMouseHover(MouseState mouseState)
        {
            if (this.State == WidgetStates.Pressed)
            {
                this.Clicked?.Invoke(this, new WidgetClickedEventArgs(MouseButtons.Left));

                this.State = WidgetStates.Hover;

                this.ButtonHover_Sound?.Play();
            }
            else
            {
                this.Mouse_Hover?.Invoke(this, new EventArgs());
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!this.Visible) return;

            if (!this.Contains(Mouse.GetState().Position))
                this.State = WidgetStates.Idle;
            else if (this.State != WidgetStates.Pressed)
                this.State = WidgetStates.Hover;
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (!this.Visible) return;

            switch (this.State)
            {
                case WidgetStates.Idle:
                    if (this.IdleTexture != null)
                        spriteBatch.Draw(this.IdleTexture, this.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);
                    break;

                case WidgetStates.Hover:
                    if (this.HoverSprite != null)
                        spriteBatch.Draw(this.HoverSprite, this.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);
                    else
                        goto case WidgetStates.Idle;
                    break;

                case WidgetStates.Pressed:
                    if (this.MouseDownTexture != null)
                        spriteBatch.Draw(this.MouseDownTexture, this.Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (float)this.ZOrder / widgetCount);
                    else
                        goto case WidgetStates.Idle;
                    break;
            }

            spriteBatch.DrawString(_font, this.Text, _textPosition, this.ForeColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, Math.Min(((float)this.ZOrder / widgetCount) + 0.01f, 1));
        }

        public void BindTo(IWidget widget)
        {
            throw new NotImplementedException();
        }

        public void OnRightMouseDown(MouseState mouseState)
        {
        }

        public bool Selected { get; set; }
    }
}