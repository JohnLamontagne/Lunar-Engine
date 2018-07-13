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
using Lidgren.Network;
using Lunar.Client.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Client.Utilities.Services;
using Lunar.Client.World.Actors;
using Lunar.Core.World;
using Lunar.Graphics;

namespace Lunar.Client.World
{
    public class MapItem
    {
        private CollisionDescriptor _collisionDescriptor;
        private string _name;
        private Sprite _sprite;
        
        public int Amount { get; set; }

        public string Name => _name;

        public Sprite Sprite => _sprite;

        public Vector2 Position => _sprite.Position;

        public MapItem()
        {
        }

        public void Unpack(NetBuffer netBuffer, Layer layer)
        {
            var position = new Vector2(netBuffer.ReadFloat(), netBuffer.ReadFloat());
            _name = netBuffer.ReadString();

            var textureName = netBuffer.ReadString();
            _sprite = new Sprite(Client.ServiceLocator.GetService<ContentManagerService>()
                .ContentManager.LoadTexture2D(Constants.FILEPATH_GFX + "/Items/" + textureName))
            {
                Position = position,
                LayerDepth = layer.ZIndex + .001f, // the .001f makes it so that the item spawns above the map layer, but just below actors
            };

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_sprite);
        }

        public bool WithinReachOf(IActor actor)
        {
            return (_collisionDescriptor.Collides(actor));
        }
    }
}
