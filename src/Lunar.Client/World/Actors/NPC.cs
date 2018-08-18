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
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Penumbra;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Graphics;
using Lunar.Graphics.Effects;

namespace Lunar.Client.World.Actors
{
    public class NPC : IActor
    {
        private string _name;
        private float _speed;
        private int _level;
        private int _health;
        private int _maximumHealth;
        private Vector2 _position;
        private SpriteSheet _spriteSheet;
        private long _uniqueID;
        private List<Vector2> _targetPath;
        private bool _moving;
        private Direction _direction;
        private double _nextUpdateSpritesheetTime;
        private Layer _layer;
        private Vector2 _frameSize;
        private Rectangle _collisionBounds;

        private bool _resetTmr;

        public string Name => _name;

        public Light Light { get; set; }

        public SpriteSheet SpriteSheet
        {
            get => _spriteSheet;
            set => _spriteSheet = value;
        }

        public float Speed
        {
            get => _speed;
            private set => _speed = value;
        }

        public int Level
        {
            get => _level;
            private set => _level = value;
        }

        public int Health
        {
            get => _health;
            private set => _health = value;
        }

        public int MaximumHealth
        {
            get => _maximumHealth;
            private set => _maximumHealth = value;
        }

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;
                this.SpriteSheet.Position = value;
            }
        }

        public long UniqueID => _uniqueID;

        public Layer Layer => _layer;

        public Rectangle CollisionBounds => _collisionBounds;

        public Direction Direction
        {
            get => _direction;
            private set
            {
                _direction = value;

                if (this.SpriteSheet != null)
                    this.SpriteSheet.Sprite.SourceRectangle = new Rectangle(this.SpriteSheet.Sprite.SourceRectangle.Left, (int)_direction * (int)_frameSize.Y, (int)_frameSize.X, (int)_frameSize.Y);
            }
        }

        public Emitter Emitter { get; set; }

        public NPC(long uniqueID)
        {
            _uniqueID = uniqueID;

            this.Light = new PointLight();

            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.NPC_MOVING, this.Handle_NPCMoving);
        }

        private void Handle_NPCMoving(PacketReceivedEventArgs args)
        {
            long uniqueID = args.Message.ReadInt64();

            if (_uniqueID != uniqueID)
                return;

            _moving = args.Message.ReadBoolean();

            if (!_moving)
            {
                _targetPath?.Clear();

                // Update NPC to final position dictated by server.
                this.Direction = (Direction)args.Message.ReadInt32();
                this.Position = new Vector2(args.Message.ReadFloat(), args.Message.ReadFloat());

                this.SpriteSheet.HorizontalFrameIndex = (int)this.Direction;

                return;
            }


            this.Direction = (Direction)args.Message.ReadInt32();

            int pathCount = args.Message.ReadInt32();

            _targetPath = new List<Vector2>();

            for(int i = 0; i < pathCount; i++)
            {
                _targetPath.Add(new Vector2(args.Message.ReadFloat(), args.Message.ReadFloat()));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            this.SpriteSheet.Draw(spriteBatch);
        }

        public void Update(GameTime gameTime)
        {
            this.ProcessMovement(gameTime);
        }

        private void ProcessMovement(GameTime gameTime)
        {
            if (_targetPath != null && _targetPath.Count > 0)
            {
                if (_resetTmr)
                {
                }

                var targetDest = _targetPath[_targetPath.Count - 1];

                if (targetDest.X < this.Position.X)
                {
                    this.Move(Direction.Left, gameTime);

                    if (targetDest.X >= this.Position.X)
                    {
                        this.Position = new Vector2(targetDest.X, this.Position.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);
                        
                    }
                }
                else if (_targetPath[_targetPath.Count - 1].X > this.Position.X)
                {
                    this.Move(Direction.Right, gameTime);

                    if (targetDest.X <= this.Position.X)
                    {
                        this.Position = new Vector2(targetDest.X, this.Position.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);
                        
                    }
                }
                else if (_targetPath[_targetPath.Count - 1].Y < this.Position.Y)
                {
                    this.Move(Direction.Up, gameTime);

                    if (targetDest.Y >= this.Position.Y)
                    {
                        this.Position = new Vector2(this.Position.X, targetDest.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);
                        
                    }
                }
                else if (_targetPath[_targetPath.Count - 1].Y > this.Position.Y)
                {
                    this.Move(Direction.Down, gameTime);

                    if (targetDest.Y <= this.Position.Y)
                    {
                        this.Position = new Vector2(this.Position.X, targetDest.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);
                        
                    }
                }
            }
        }

        private void Move(Direction direction, GameTime gameTime)
        {
            this.Direction = direction;

            float dX = 0, dY = 0;

            switch (this.Direction)
            {
                case Direction.Right:
                    dX = this.Speed * gameTime.ElapsedGameTime.Milliseconds;
                    break;

                case Direction.Left:
                    dX = -this.Speed * gameTime.ElapsedGameTime.Milliseconds;
                    break;

                case Direction.Up:
                    dY = -this.Speed * gameTime.ElapsedGameTime.Milliseconds;
                    break;

                case Direction.Down:
                    dY = this.Speed * gameTime.ElapsedGameTime.Milliseconds;
                    break;
            }

            if (gameTime.TotalGameTime.TotalMilliseconds > _nextUpdateSpritesheetTime)
            {
                this.SpriteSheet.HorizontalFrameIndex += 1;
                this.SpriteSheet.VerticalFrameIndex = (int)this.Direction;

                _nextUpdateSpritesheetTime = (long)gameTime.TotalGameTime.TotalMilliseconds + (long)((_frameSize.Y / this.Speed) / (this.SpriteSheet.Sprite.Texture.Width / _frameSize.X));
            }

            this.Position = new Vector2(this.Position.X + dX, this.Position.Y + dY);
        }

        public void Unpack(NetBuffer buffer, ContentManager contentManager)
        {
            _name = buffer.ReadString();
            string texturePath = buffer.ReadString();

            var sprite = new Sprite(
                contentManager.LoadTexture2D(Constants.FILEPATH_DATA + texturePath));
            this.Speed = buffer.ReadFloat();
            
           
            this.Health = buffer.ReadInt32();
            this.MaximumHealth = buffer.ReadInt32();
            this.Level = buffer.ReadInt32();        
            Vector2 position = new Vector2(buffer.ReadFloat(), buffer.ReadFloat());
            _frameSize = new Vector2(buffer.ReadFloat(), buffer.ReadFloat());
            _collisionBounds = new Rectangle(buffer.ReadInt32(), buffer.ReadInt32(), buffer.ReadInt32(), buffer.ReadInt32());

            this.SpriteSheet = new SpriteSheet(sprite, (int)(sprite.Texture.Width / _frameSize.X), (int)(sprite.Texture.Height / _frameSize.Y), (int)_frameSize.X, (int)_frameSize.Y);

            this.Position = position;

            var layerName = buffer.ReadString();
            _layer = Client.ServiceLocator.GetService<WorldManager>().Map.GetLayer(layerName);

            this.SpriteSheet.Sprite.LayerDepth = _layer.ZIndex + (EngineConstants.PARTS_PER_LAYER / 2);
            this.SpriteSheet.HorizontalFrameIndex = 1;
            this.SpriteSheet.VerticalFrameIndex = (int) this.Direction;

        }
    }
}
