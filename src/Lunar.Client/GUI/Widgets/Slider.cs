using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lunar.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lunar.Client.GUI.Widgets
{
    internal class Slider : IWidget
    {
        private Vector2 _controlPosition;
        private Vector2 _controlScale;
        private Rectangle _controlRect;
        private Rectangle _containerRect;

        private int _value;
        private int _maximumValue;
        private bool _active;
        private bool _dragging;
        private Vector2 _dragRelPosition;
        private Vector2 _position;
        private Vector2 _padding;
        private float _rotation;
        private Orientation _orientation;
        private readonly Orientation _initalOrientation;

        public int ZOrder { get; set; }
        public bool Visible { get; set; }

        public float Scale { get; set; }

        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                this.Activated?.Invoke(this, new EventArgs());
            }
        }

        public bool Selectable { get; set; }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;

                this.UpdateControlCalculations();
            }
        }

        public Vector2 Origin { get; set; }
        public string Name { get; set; }
        public object Tag { get; set; }

        public Vector2 Padding
        {
            get => _padding;
            set
            {
                _padding = value;

                this.UpdateControlCalculations();
            }
        }

        public Texture2D ContainerTexture { get; set; }

        public Texture2D ControlTexture { get; set; }

        public Orientation Orientation
        {
            get => _orientation;
            set
            {
                _orientation = value;
                this.UpdateControlCalculations();
            }
        }

        public int MaximumValue
        {
            get => _maximumValue;
            set
            {
                _maximumValue = value;

                this.UpdateControlCalculations();
            }
        }

        public int Value
        {
            get => _value;
            set
            {
                if (value <= this.MaximumValue)
                {
                    _value = value;

                    this.UpdateControlCalculations();

                    this.ValueChanged?.Invoke(this, new EventArgs());
                }
            }
        }

        public event EventHandler<WidgetClickedEventArgs> Clicked;

        public event EventHandler Mouse_Hover;

        public event EventHandler Activated;

        public event EventHandler<WidgetNameChangedEventArgs> NameChanged;

        public event EventHandler ValueChanged;

        public Slider(Texture2D containerTexture, Texture2D controlTexture, Orientation initialOrientation)
        {
            _initalOrientation = initialOrientation;

            this.ContainerTexture = containerTexture;
            this.ControlTexture = controlTexture;

            this.Orientation = Orientation.Horizontal;
            this.Visible = true;
            this.Scale = 1f;
            this.MaximumValue = 1;

            this.UpdateControlCalculations();
        }

        private void UpdateControlCalculations()
        {
            if (this.ControlTexture != null && this.ContainerTexture != null)
            {
                if (this.Orientation == _initalOrientation)
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        _controlScale = new Vector2((this.ContainerTexture.Width / (this.MaximumValue + 1)) / this.ControlTexture.Width, 1f);

                        if (this.Value == this.MaximumValue)
                        {
                            _controlPosition = new Vector2(this.Position.X + this.ContainerTexture.Width - (this.ControlTexture.Width * _controlScale.X),
                                this.Position.Y + this.Padding.Y);
                        }
                        else
                        {
                            _controlPosition = new Vector2(this.UnitToPosition(this.Value) + this.Position.X + this.Padding.X,
                                this.Position.Y + this.Padding.Y);
                        }

                        _controlRect = new Rectangle((int)_controlPosition.X, (int)_controlPosition.Y, (int)(this.ControlTexture.Width * _controlScale.X), (int)(this.ControlTexture.Height * _controlScale.Y));
                        _containerRect = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.ContainerTexture.Width, this.ContainerTexture.Height);
                        _rotation = 0f;
                    }
                    else
                    {
                        _controlPosition = new Vector2(this.Position.X + this.Padding.X,
                            Math.Min(this.UnitToPosition(this.Value) + this.Position.Y + this.Padding.Y,
                            this.Position.Y + this.ContainerTexture.Height - (this.ControlTexture.Height * _controlScale.Y)));

                        _controlScale = new Vector2(1f, (this.ContainerTexture.Width / (this.MaximumValue + 1)) / this.ControlTexture.Width);
                        _controlRect = new Rectangle((int)_controlPosition.X, (int)_controlPosition.Y, (int)(this.ControlTexture.Width * _controlScale.X), (int)(this.ControlTexture.Height * _controlScale.Y));
                        _containerRect = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.ContainerTexture.Width, this.ContainerTexture.Height);
                        _rotation = 0f;
                    }
                }
                else
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        _controlScale = new Vector2(1f, (this.ContainerTexture.Width / (this.MaximumValue + 1)) / this.ControlTexture.Width);

                        _containerRect = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.ContainerTexture.Width, this.ContainerTexture.Height);

                        _controlPosition = new Vector2(Math.Min(this.UnitToPosition(this.Value) + this.Position.X + this.Padding.X,
                             this.Position.X + this.ContainerTexture.Width - (this.ControlTexture.Width * _controlScale.X)),
                             this.Position.Y + this.Padding.Y);

                        _controlRect = new Rectangle((int)_controlPosition.X, (int)_controlPosition.Y, (int)(this.ControlTexture.Width * _controlScale.X), (int)(this.ControlTexture.Height * _controlScale.Y));

                        _rotation = (float)Math.PI / 2.0f;
                        _controlRect = _controlRect.Rotate(_rotation);
                        _containerRect = _containerRect.Rotate(_rotation);

                        // Recaluate with rotation
                        _controlPosition = new Vector2(Math.Min(this.UnitToPosition(this.Value) + this.Position.X + this.Padding.X,
                            this.Position.X + this.ContainerTexture.Width - (this.ControlTexture.Width * _controlScale.X)),
                            this.Position.Y + this.Padding.Y);
                    }
                    else
                    {
                        _controlScale = new Vector2(1f, (this.ContainerTexture.Width / (this.MaximumValue + 1)) / this.ControlTexture.Width);

                        _containerRect = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.ContainerTexture.Width, this.ContainerTexture.Height);

                        if (this.Value == this.MaximumValue)
                        {
                            _controlPosition = new Vector2(_containerRect.X + this.Padding.X,
                                _containerRect.Y + this.Padding.Y + this.ContainerTexture.Width - this.ControlTexture.Width);
                        }
                        else
                        {
                            _controlPosition = new Vector2(_containerRect.X + this.Padding.X,
                                this.UnitToPosition(this.Value) + _containerRect.Y + this.Padding.Y);
                        }

                        _controlRect = new Rectangle((int)_controlPosition.X, (int)_controlPosition.Y, (int)(this.ControlTexture.Width * _controlScale.X), (int)(this.ControlTexture.Height * _controlScale.Y));

                        _rotation = (float)Math.PI / 2.0f;
                        _controlRect = _controlRect.Rotate(_rotation);
                        _containerRect = _containerRect.Rotate(_rotation);
                    }
                }
            }
        }

        public void BindTo(IWidget widget)
        {
            throw new NotImplementedException();
        }

        public bool Contains(Point point)
        {
            return _containerRect.Contains(point);
        }

        private bool ControlContains(Point point)
        {
            return _controlRect.Contains(point);
        }

        private float UnitToPosition(int unit)
        {
            if (unit == 0)
                return 0f;

            if (_initalOrientation == this.Orientation)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    return (unit * ((this.ContainerTexture.Width * this.Scale) / (this.MaximumValue + 1)));
                }
                else
                {
                    return (unit * ((this.ContainerTexture.Height * this.Scale) / (this.MaximumValue + 1)));
                }
            }
            else
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    return (unit * ((this.ContainerTexture.Height * this.Scale) / (this.MaximumValue + 1)));
                }
                else
                {
                    return (unit * ((this.ContainerTexture.Width * this.Scale) / (this.MaximumValue + 1)));
                }
            }
        }

        private int PositionToUnit(float position)
        {
            if (position < 0)
                return 0;

            if (_initalOrientation == this.Orientation)
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    return (int)Math.Round(position / ((this.ContainerTexture.Width * this.Scale) / (this.MaximumValue + 1)));
                }
                else
                {
                    return (int)Math.Round(position / ((this.ContainerTexture.Height * this.Scale) / (this.MaximumValue + 1)));
                }
            }
            else
            {
                if (this.Orientation == Orientation.Horizontal)
                {
                    return (int)Math.Round(position / ((_containerRect.Width) / (this.MaximumValue + 1)), MidpointRounding.AwayFromZero);
                }
                else
                {
                    return (int)Math.Round(position / ((_containerRect.Height) / (this.MaximumValue + 1)), MidpointRounding.AwayFromZero);
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, int widgetCount)
        {
            if (!this.Visible)
                return;

            spriteBatch.Draw(this.ContainerTexture, this.Position, null, Color.White,
            _rotation, Vector2.Zero, this.Scale, SpriteEffects.None, (float)this.ZOrder / widgetCount);

            spriteBatch.Draw(this.ControlTexture, _controlPosition, null,
                Color.White, _rotation, Vector2.Zero, _controlScale, SpriteEffects.None, (float)this.ZOrder / widgetCount + .01f);
        }

        public void OnLeftMouseDown(MouseState mouseState)
        {
            if (this.ControlContains(mouseState.Position))
            {
                _dragging = true;
                _dragRelPosition = new Vector2(mouseState.Position.X - _controlRect.X,
                    mouseState.Position.Y - _controlRect.Y);
            }
        }

        public void OnMouseHover(MouseState mouseState)
        {
        }

        public void OnRightMouseDown(MouseState mouseState)
        {
        }

        public void Update(GameTime gameTime)
        {
            var mousePos = Mouse.GetState().Position;

            if (Mouse.GetState().LeftButton == ButtonState.Released && _dragging)
            {
                // Move slider left or right
                if (this.Orientation == Orientation.Horizontal)
                {
                    this.Value = this.PositionToUnit((mousePos.X - (this.Position.X + this.Padding.X)) - _dragRelPosition.X);
                }
                else // Move slider up or down
                {
                    this.Value = this.PositionToUnit((mousePos.Y - (this.Position.Y + this.Padding.Y)) - _dragRelPosition.Y);
                }

                _dragging = false;
                _dragRelPosition = Vector2.Zero;
                return;
            }

            if (_dragging)
            {
                if (_initalOrientation == this.Orientation)
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        _controlPosition = new Vector2(Math.Max(Math.Min(mousePos.X - _dragRelPosition.X,
                            this.Position.X + this.ContainerTexture.Width - (this.ControlTexture.Width * _controlScale.X)),
                            this.Position.X + this.Padding.X), _controlPosition.Y);
                    }
                    else
                    {
                        _controlPosition = new Vector2(_controlPosition.X, Math.Max(Math.Min(mousePos.Y - _dragRelPosition.Y,
                            this.Position.Y + this.ContainerTexture.Height - (this.ControlTexture.Height * _controlScale.Y)),
                            this.Position.Y + this.Padding.Y));
                    }
                }
                else
                {
                    if (this.Orientation == Orientation.Horizontal)
                    {
                        _controlPosition = new Vector2(Math.Max(Math.Min(mousePos.X - _dragRelPosition.X,
                            this.Position.X + _containerRect.Width - (_controlRect.Width)),
                            this.Position.X + this.Padding.X), _controlPosition.Y);
                    }
                    else
                    {
                        _controlPosition = new Vector2(_controlPosition.X, Math.Max(Math.Min(mousePos.Y - _dragRelPosition.Y,
                             this.Position.Y + _containerRect.Height - (_controlRect.Height)),
                             this.Position.Y + this.Padding.Y));
                    }
                }
            }
        }
    }
}