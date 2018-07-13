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