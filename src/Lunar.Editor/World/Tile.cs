using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.World.Structure;
using Lunar.Graphics;

namespace Lunar.Editor.World
{
    public class Tile
    {
        private TileDescriptor _descriptor;

        public TileDescriptor Descriptor => _descriptor;

        private long _nextAnimationTime;
        private Sprite _sprite;

        public float ZIndex
        {
            get => this.Sprite.LayerDepth;
            set
            {
                this.Sprite.LayerDepth = value;
                _descriptor.SpriteInfo.Transform.LayerDepth = value;
            }
        }

        public Sprite Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;

                _descriptor.SpriteInfo = new SpriteInfo(this.Sprite.Texture.Tag.ToString());
                _descriptor.Position = new Vector(this.Sprite.Position.X, this.Sprite.Position.Y);
                _descriptor.SpriteInfo.Transform = new Transform()
                {
                    Rect = new Rect(this.Sprite.SourceRectangle.Left, this.Sprite.SourceRectangle.Top, this.Sprite.SourceRectangle.Width, this.Sprite.SourceRectangle.Height),
                    Color = new Core.Content.Graphics.Color(this.Sprite.Color.R, this.Sprite.Color.G, this.Sprite.Color.B, this.Sprite.Color.A),
                    LayerDepth = this.Sprite.LayerDepth,
                    Position = this.Sprite.Position
                };
            }
        }

        public Tile(TileDescriptor descriptor)
        {
            _descriptor = descriptor;
        }

        public Tile(Texture2D texture, Rectangle sourceRectangle, Vector2 position)
            : this()
        {
            this.Sprite = new Sprite(texture)
            {
                SourceRectangle = sourceRectangle,
                Position = position,
            };

        }

        public Tile()
        {
            _descriptor = new TileDescriptor(null);
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.Sprite != null)
                spriteBatch.Draw(this.Sprite);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds >= _nextAnimationTime && this.Descriptor.Animated)
            {

                _nextAnimationTime = (long)gameTime.TotalGameTime.TotalMilliseconds + 300;
            }
        }

       
    }
}