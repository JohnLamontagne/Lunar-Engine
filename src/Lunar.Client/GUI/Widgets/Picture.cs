using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    public class Picture : IWidget
    {
        private WidgetStates _previousState;
        private MouseButtons _previousPressedButton;
        private Rectangle _area;
        private Vector2 _position;
        private Vector2 _scale;
        
        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public bool Visible { get; set; }

        public Texture2D Sprite { get; set; }

        public bool Active { get; set; }

        public string Tag { get; set; }

        public int ZOrder { get; set; }

        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                _area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Sprite.Width), (int)(this.Sprite.Height));
            }
        }

        public Vector2 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;

                _area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Sprite.Width * this.Scale.X), (int)(this.Sprite.Height * this.Scale.Y));
            }
        }

        public bool Selected { get; set; }

        public bool Selectable { get; set; }

        public Picture(Texture2D sprite)
        {
            this.Sprite = sprite;
            this.Scale = new Vector2(1f);

            _previousState = WidgetStates.Idle;

            this.Selectable = false;

            _area = new Rectangle((int)this.Position.X, (int)this.Position.Y, (int)(this.Sprite.Width), (int)(this.Sprite.Height));
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!this.Visible) return;

            if (!this.Contains(Mouse.GetState().Position))
                _previousState = WidgetStates.Idle;
            else if (_previousState != WidgetStates.Pressed)
                _previousState = WidgetStates.Hover;
        }

        public virtual void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (this.Visible)
                spriteBatch.Draw(this.Sprite, this.Position, null, Color.White, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, (float)this.ZOrder / widgetCount);

            spriteBatch.Draw(this.Sprite, this.Position, null, Color.White, 0f, Vector2.Zero, this.Scale, SpriteEffects.None, (float)this.ZOrder / widgetCount);
        }

        public bool Contains(Point point)
        {
            return _area.Contains(point);
        }

        public void BindTo(IWidget widget)
        {
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
        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
            _previousState = WidgetStates.Pressed;
            _previousPressedButton = MouseButtons.Left;
        }


        public void OnRightMouseDown(MouseState mouseState)
        {
            _previousState = WidgetStates.Pressed;
            _previousPressedButton = MouseButtons.Right;
        }
    }
}