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

using System.IO;
using Lidgren.Network;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Net;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World.Structure;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors;
using Lunar.Server.World.Structure.Attribute;

namespace Lunar.Server.World.Structure
{
    public class Tile : TileDescriptor<SpriteInfo>
    {
        private Rect _collisionArea;

        public Layer Layer { get; }

        protected Tile(TileDescriptor<SpriteInfo> tileData)
        {
            this.Animated = tileData.Animated;
            this.Attribute = tileData.Attribute;
            this.Blocked = tileData.Blocked;
            this.FrameCount = tileData.FrameCount;
            this.LightColor = tileData.LightColor;
            this.LightRadius = tileData.LightRadius;
            this.LightSource = tileData.LightSource;
            this.Position = tileData.Position;
            this.Sprite = tileData.Sprite;
            this.Teleporter = tileData.Teleporter;
        }

        public Tile(Layer layer, TileDescriptor<SpriteInfo> tileData)
            : this(tileData)
        {
            this.Layer = layer;

            if (this.Attribute != null)
                this.Attribute.ActionHandler = TileAttributeActionHandlerFactory.Create(this.Attribute);

            if (this.Sprite != null)
                _collisionArea = new Rect(this.Sprite.Transform.Position.X, this.Sprite.Transform.Position.Y, Settings.TileSize, Settings.TileSize);
            else
                _collisionArea = new Rect(this.Position.X, this.Position.Y, Settings.TileSize, Settings.TileSize);

            this.Attribute?.ActionHandler?.OnInitalize(new TileAttributeArgs(this.Attribute, this));
        }

        public Tile(Vector position)
        {
            _collisionArea = new Rect(position.X, position.Y, Settings.TileSize, Settings.TileSize);
        }

        public void Update(GameTime gameTime)
        {
            this.Attribute?.ActionHandler?.OnUpdate(new TileAttributeUpdateArgs(this.Attribute, gameTime, this));
        }

        public bool CheckCollision(Rect collisionArea)
        {
            return (_collisionArea.Intersects(collisionArea));
        }

        public void OnPlayerEntered(Player player)
        {
            this.Attribute?.ActionHandler?.OnPlayerEntered(new TileAttributePlayerArgs(this.Attribute, this, player));
        }

        public void OnPlayerLeft(Player player)
        {
            this.Attribute?.ActionHandler?.OnPlayerLeft(new TileAttributePlayerArgs(this.Attribute, this, player));
        }

        public NetBuffer PackData()
        {
            var netBuffer = new NetBuffer();

            // Tell the client whether it's a blank tile
            netBuffer.Write(this.Sprite == null);

            // Is this a blank tile (determined based on the existence of a Sprite)
            if (this.Sprite != null)
            {
                netBuffer.Write(this.LightSource);
                netBuffer.Write(this.LightRadius);
                netBuffer.Write(this.LightColor);
                netBuffer.Write(this.Teleporter);
                netBuffer.Write(this.Sprite.TextureName);
                netBuffer.Write(this.Sprite.Transform.Color);
                netBuffer.Write(this.Sprite.Transform.Rect);
                netBuffer.Write(this.Sprite.Transform.Position);
                netBuffer.Write(this.Animated);
                netBuffer.Write(this.FrameCount);
            }

            return netBuffer;
        }

        public void Load(BinaryReader bR, Vector tilePosition)
        {
        }
    }
}