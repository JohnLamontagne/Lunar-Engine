using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using Lunar.Graphics;
using Lunar.Graphics.Effects;

namespace Lunar.Client.World.Actors
{
    public interface IActor
    {
        long UniqueID { get; }

        string Name { get; }

        float Speed { get; }

        int Level { get; }

        int Health { get; }

        int MaximumHealth { get; }

        Emitter Emitter { get; set; }

        Vector2 Position { get; }

        Layer Layer { get; }

        Rectangle CollisionBounds { get; }

        SpriteSheet SpriteSheet { get; }

        Light Light { get; }

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}