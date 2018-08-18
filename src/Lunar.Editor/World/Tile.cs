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
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Lunar.Core.World.Structure;
using Lunar.Graphics;

namespace Lunar.Editor.World
{
    public class Tile
    {
        private long _nextAnimationTime;
        private Sprite _sprite;

        public bool Animated { get; set; }

        public bool LightSource { get; set; }

        public bool Teleporter { get; set; }

        public int PlayerDamage { get; set; }

        public int FrameCount { get; set; }

        public TileAttributes Attribute { get; set; }

        public AttributeData AttributeData { get; set; }

        public Color Color { get; set; }

        public float ZIndex
        {
            get => _sprite.LayerDepth;
            set => _sprite.LayerDepth = value;
        }

        public Sprite Sprite => _sprite;

        public Tile(Texture2D texture, Rectangle sourceRectangle, Vector2 position)
            : this()
        {
            _sprite = new Sprite(texture)
            {
                SourceRectangle = sourceRectangle,
                Position = position
            };

        }

        public Tile()
        {
            this.Attribute = TileAttributes.None;
            this.AttributeData = new AttributeData();
            this.Animated = false;
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

        public void Save(BinaryWriter bW)
        {
            bW.Write((byte)this.Attribute);
           
            byte[] attributeDataBytes = this.AttributeData.Serialize();
            bW.Write(attributeDataBytes.Length);
            bW.Write(attributeDataBytes);

            // Tell the client whether it's a blank tile
            bW.Write(_sprite != null);
            // Is this a blank tile (determined based on the existence of a Sprite)
            if (_sprite != null)
            {
                bW.Write(this.Animated);
                bW.Write(this.LightSource);
                bW.Write(_sprite.Texture.Tag.ToString());
                bW.Write(this.ZIndex);
                bW.Write(this.Color.R);
                bW.Write(this.Color.G);
                bW.Write(this.Color.B);
                bW.Write(this.Color.A);
                bW.Write(this.Sprite.SourceRectangle.X);
                bW.Write(this.Sprite.SourceRectangle.Y);
                bW.Write(this.Sprite.SourceRectangle.Width);
                bW.Write(this.Sprite.SourceRectangle.Height);
                bW.Write(this.FrameCount);
            }
        }

        public void Load(BinaryReader bR, Dictionary<string, Texture2D> tilesets, Vector2 tilePosition)
        {
            this.Attribute = (TileAttributes)bR.ReadByte();

            int attributeDataLength = bR.ReadInt32();
            byte[] attributeData = bR.ReadBytes(attributeDataLength);
            this.AttributeData = AttributeData.Deserialize(attributeData);

            if (bR.ReadBoolean())
            {
                this.Animated = bR.ReadBoolean();
                this.LightSource = bR.ReadBoolean();

                string spriteName = bR.ReadString();
                float zIndex = bR.ReadSingle();

                if (tilesets.ContainsKey(Path.GetFileName(spriteName)))
                {
                    _sprite = new Sprite(tilesets[Path.GetFileName(spriteName)])
                    {
                        LayerDepth = zIndex,
                        Position = tilePosition
                    };
                }

                this.Color = new Color(bR.ReadByte(), bR.ReadByte(), bR.ReadByte(), bR.ReadByte());

                var rectangle = new Rectangle(bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32(), bR.ReadInt32());

                if (_sprite != null)
                    _sprite.SourceRectangle = rectangle;

                this.FrameCount = bR.ReadInt32();
            }
        }
    }
}