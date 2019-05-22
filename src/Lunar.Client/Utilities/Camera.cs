/** Copyright 2018 John Lamontagne https://www.rpgorigin.com

	Licensed under the Apache License, Version 2.0 (the "License");
	you may not use this file except in compliance with the License.
	You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0

	Unless required by applicable law or agreed to in writing, software
	distributed under the License is distributed on an "AS IS" BASIS,
	WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	See the License for the specific language governing permissions and
	limitations under the License.
*/
using Lunar.Client.World.Actors;
using Microsoft.Xna.Framework;
using System;

namespace Lunar.Client.Utilities
{
    public class Camera
    {
        private float _zoom;
        private Vector2 _position;
        private float _rotation;
        private Vector2 _minView;
        private Vector2 _maxView;
        private Rectangle _bounds;

        public float Speed { get; set; }

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

        public IActor Subject { get; set; }

        public Vector2 Position
        {
            get => _position;
            set => _position = new Vector2((int)Math.Floor(MathHelper.Clamp(value.X, _minView.X, _maxView.X - (float)Settings.ResolutionX)), (int)Math.Floor(MathHelper.Clamp(value.Y, _minView.Y, _maxView.Y - (float)Settings.ResolutionY)));
        }

        public Camera(Rectangle bounds)
        {
            _zoom = 1f;
            _rotation = 0f;
            _position = Vector2.Zero;
            this.Bounds = bounds;
            this.Speed = 0.2f;
            //this.Rotate(30);
        }



        public void Rotate(float amount)
        {
            _rotation += amount;

            if (_rotation > 360f) _rotation -= 360f;
        }

        public Matrix GetTransformation()
        {
            return Matrix.CreateTranslation(new Vector3(-this.Position.X * this.Zoom, -this.Position.Y * this.Zoom, 0f)) *
                Matrix.CreateRotationZ(this.Rotation) *
                Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 1f));
        }

        public void Unload()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (this.Subject != null)
            {
                float x = MathHelper.Lerp(this.Position.X, (float)Math.Floor(this.Subject.Position.X) + (this.Subject.SpriteSheet.Sprite.SourceRectangle.Width / 2) - (Settings.ResolutionX / 2), this.Speed);
                float y = MathHelper.Lerp(this.Position.Y, (float)Math.Floor(this.Subject.Position.Y) + (this.Subject.SpriteSheet.Sprite.SourceRectangle.Height / 2) - (Settings.ResolutionY / 2), this.Speed);
                this.Position = new Vector2(x, y);
            }
        }

        public Vector2 ScreenToWorldCoords(Vector2 screenPos)
        {
            return Vector2.Transform(screenPos, Matrix.Invert(this.GetTransformation()));
        }
    }
}