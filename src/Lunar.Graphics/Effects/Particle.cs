using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lunar.Graphics.Effects
{
    public class Particle
    {
        private long _timeToLive;
        private Texture2D _texture;
        private Rectangle _srcRectangle;
        private Vector2 _origin;

        public Texture2D Texture
        {
            get => _texture;
            private set
            {
                _texture = value;
                _srcRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);
                _origin = new Vector2(this.Texture.Width / 2f, this.Texture.Height / 2f);
            }
        }

        public Vector2 Position { get; set; }             
        public Vector2 Velocity { get; set; }        
        public float Angle { get; set; }           
        public float AngularVelocity { get; set; }    
        public Color Color { get; set; }            
        public float Size { get; set; }

        public bool IsAlive => _timeToLive >= 0;

        public Particle(Texture2D texture, Vector2 position, Vector2 velocity, float angle, float angularVelocity, Color color, float size, int lifeTime)
        {
            this.Texture = texture;
            this.Position = position;
            this.Velocity = velocity;
            this.Angle = angle;
            this.AngularVelocity = angularVelocity;
            this.Color = color;
            this.Size = size;
            _timeToLive = lifeTime;
        }

        public void Update(GameTime gameTime)
        {
            _timeToLive--;
            this.Position += this.Velocity;
            this.Angle += this.AngularVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, _srcRectangle, Color,
                Angle, _origin, Size, SpriteEffects.None, 0f);
        }
    }
}
