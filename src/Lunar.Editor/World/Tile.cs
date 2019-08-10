using Lunar.Core.Content.Graphics;
using Lunar.Core.Utilities.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.World.Structure;
using Lunar.Graphics;

namespace Lunar.Editor.World
{
    public class Tile : BaseTile<Sprite>
    {
        private long _nextAnimationTime;
        private Sprite _sprite;

        public float ZIndex
        {
            get => this.Sprite.Transform.LayerDepth;
            set
            {
                this.Sprite.Transform.LayerDepth = value;
            }
        }

        public override Sprite Sprite
        {
            get { return _sprite; }
            set
            {
                _sprite = value;
                _sprite.TextureName = this.Sprite.Texture.Tag.ToString();

                this.Position = new Vector(this.Sprite.Transform.Position.X, this.Sprite.Transform.Position.Y);
            }
        }

        public Tile(BaseTile<SpriteInfo> descriptor)
        {
            this.Animated = descriptor.Animated;
            this.Attribute = descriptor.Attribute;
            this.Blocked = descriptor.Blocked;
            this.FrameCount = descriptor.FrameCount;
            this.LightColor = descriptor.LightColor;
            this.LightRadius = descriptor.LightRadius;
            this.LightSource = descriptor.LightSource;
            this.Position = descriptor.Position;
            this.Teleporter = descriptor.Teleporter;
        }

        public Tile(Texture2D texture, Microsoft.Xna.Framework.Rectangle sourceRectangle, Vector2 position)
            : this()
        {
            this.Sprite = new Sprite(texture);
            this.Sprite.Transform.Rect = sourceRectangle;
            this.Sprite.Transform.Position = position;
        }

        public Tile()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (this.Sprite != null)
                spriteBatch.Draw(this.Sprite);
        }

        public void Update(GameTime gameTime)
        {
            if (gameTime.TotalGameTime.TotalMilliseconds >= _nextAnimationTime && this.Animated)
            {
                _nextAnimationTime = (long)gameTime.TotalGameTime.TotalMilliseconds + 300;
            }
        }
    }
}