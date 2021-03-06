﻿using Microsoft.Xna.Framework;

namespace Lunar.Editor.Utilities
{
    public class Camera
    {
        private float _zoom;
        private Vector2 _position;
        private float _rotation;
        private Vector2 _minView;
        private Vector2 _maxView;
        private Rectangle _bounds;

        public int ResolutionX { get; set; }

        public int ResolutionY { get; set; }

        public float Zoom
        {
            get { return _zoom; }
            set
            {
                _zoom = _zoom < 0.1f ? 0.1f : value;
                _bounds = new Rectangle(_bounds.X, _bounds.Y, (int)(_bounds.Width * this.Zoom), (int)(_bounds.Height * this.Zoom));
                _minView = new Vector2(this.Bounds.X, this.Bounds.Y);
                _maxView = new Vector2(this.Bounds.Width, this.Bounds.Height);
            }
        }

        public Rectangle Bounds
        {
            get
            {
                return _bounds;
            }
            set
            {
                _bounds = new Rectangle(value.X, value.Y, (int)(value.Width * this.Zoom), (int)(value.Height * this.Zoom));
                _minView = new Vector2(this.Bounds.X, this.Bounds.Y);
                _maxView = new Vector2(this.Bounds.Width, this.Bounds.Height);
            }
        }

        public float Rotation { get { return _rotation; } set { _rotation = value; } }

        public Vector2 Position
        {
            get => _position;
            set => _position = new Vector2(MathHelper.Clamp(value.X, _minView.X, _maxView.X - this.ResolutionX), MathHelper.Clamp(value.Y, _minView.Y, _maxView.Y - this.ResolutionY));
        }

        public Camera(Rectangle bounds)
        {
            _zoom = 1f;
            _rotation = 0f;
            _position = Vector2.Zero;
            this.Bounds = bounds;
        }

        public void Move(Vector2 distance)
        {
            _position += distance;
            Vector2.Clamp(this.Position, _minView, _maxView);
        }

        public void Rotate(float amount)
        {
            _rotation += amount;

            if (_rotation > 360) _rotation -= 360;
        }

        public Matrix GetTransformation()
        {
            return Matrix.CreateTranslation(new Vector3(-this.Position.X * this.Zoom, -this.Position.Y * this.Zoom, 0)) *
                Matrix.CreateRotationZ(this.Rotation) *
                Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 1));
        }

        public void Unload()
        {
        }

        public Vector2 ScreenToWorldCoords(Vector2 screenPos)
        {
            return Vector2.Transform(screenPos, Matrix.Invert(this.GetTransformation()));
        }
    }
}