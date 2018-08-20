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
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    public class WidgetContainer : GUIManager, IWidget
    {
        private Rectangle _area;
        private Texture2D _backSprite;
        private Vector2 _position;
        private int _relativeDragX;
        private int _relativeDragY;
        private bool _dragStarted;
        private Vector2 _size;

        public bool Visible { get; set; }

        public bool Active { get; set; }

        public bool Draggable { get; set; }

        public bool Selectable { get; set; }

        public string Tag { get; set; }

        public int ZOrder { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Position {
            get => _position;
            set
            {
                // Update the child elements
                foreach (var widget in _widgets.Values)
                {
                    var relX = widget.Position.X - this.Position.X;
                    var relY = widget.Position.Y - this.Position.Y;

                    widget.Position = new Vector2(value.X + relX, value.Y + relY);
                }

                _position = value;
                _area = new Rectangle((int)this.Position.X, (int)this.Position.Y, _area.Width, _area.Height);
            }
        }

        public Texture2D BackSprite
        {
            get => _backSprite;
            set
            {
                _backSprite = value;
                this.Area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Size.X, (int)this.Size.Y);
            }
        }

        public Rectangle Area
        {
            get => _area;
            set => _area = value;
        }


        public Vector2 Size
        {
            get => _size;
            set
            {
                _size = value;

                this.Area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Size.X, (int)this.Size.Y);
            }
        }

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public WidgetContainer(Texture2D backSprite)
        {
            _backSprite = backSprite;

            this.Size = new Vector2(_backSprite.Width, _backSprite.Height);
            this.Selectable = true;

            this.Area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Size.X, (int)this.Size.Y);
            this.Visible = true;
            this.Origin = Vector2.Zero;
        }

        public WidgetContainer(Vector2 size)
        {
            this.Size = size;
            this.Selectable = true;

            this.Area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)this.Size.X, (int)this.Size.Y);
            this.Visible = true;
            this.Origin = Vector2.Zero;
        }

        public void OnMouseHover(MouseState mouseState)
        {
        }


        public void OnLeftMouseDown(MouseState mouseState)
        {
        }

        public override void Update(GameTime gameTime)
        {
            if (!this.Visible)
                return;

            if (this.Contains(Mouse.GetState().Position))
            {
                if (Mouse.GetState().LeftButton == ButtonState.Pressed)
                {
                    if (this.Draggable && !_dragStarted && this.Contains(Mouse.GetState().Position) && this.Active)
                    {
                        // Get the starting mouse position.
                        _relativeDragX = (int)this.Position.X - Mouse.GetState().Position.X;
                        _relativeDragY = (int)this.Position.Y - Mouse.GetState().Position.Y;
                        _dragStarted = true;
                    }
                }
                else
                {
                    _dragStarted = false;
                }
            }
            else
            {
                if (!_dragStarted)
                {
                    this.Active = false;
                }
            }

            if (this.Draggable)
            {
                if (_dragStarted)
                {
                    var newX = Mouse.GetState().Position.X + _relativeDragX;
                    var newY = Mouse.GetState().Position.Y + _relativeDragY;

                    this.Position = new Vector2(newX, newY);
                }
            }

            base.Update(gameTime);
        }

        public bool Contains(Point point)
        {
            return this.Area.Contains(point) || _widgets.Values.Any(widget => widget.Contains(point));
        }

        public void BindTo(IWidget widget)
        {
            throw new System.NotImplementedException();
        }

        public void OnRightMouseDown(MouseState mouseState)
        {
            
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (!this.Visible) return;

            this.Begin(spriteBatch);

            if (this.BackSprite != null)
                spriteBatch.Draw(this.BackSprite, this.Position - this.Origin, null, Color.White, 0f, Vector2.Zero, new Vector2(_size.X / this.BackSprite.Width, _size.Y / this.BackSprite.Height), SpriteEffects.None, (float)this.ZOrder / widgetCount);

            this.Draw(spriteBatch);

            this.End(spriteBatch);
        }

        protected override Vector2 ParsePosition(string posX, string posY)
        {
            float x = 0;
            float y = 0;

            if (posX == null)
            {
                x = 0;
            }
            else if (posX.Contains("%"))
            {
                float.TryParse(posX.Replace("%", ""), out float pX);
                x = this.Size.Y * (pX / 100f);
            }
            else
            {
                float.TryParse(posX, out x);
            }

            if (posY == null)
            {
                y = 0;
            }
            else if (posY.Contains("%"))
            {
                float.TryParse(posY.Replace("%", ""), out float pY);
                y = this.Size.Y * (pY / 100f);
            }
            else
            {
                float.TryParse(posY, out y);
            }

            return new Vector2(x + this.Position.X, y + this.Position.Y);
        }

        public bool Selected { get; set; }
    }
}