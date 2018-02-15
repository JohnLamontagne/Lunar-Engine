using System;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Penumbra;
using Lunar.Client.Net;
using Lunar.Client.Utilities;
using Lunar.Client.Utilities.Services;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.World;
using Lunar.Core.World.Actor;
using Lunar.Graphics;
using Lunar.Graphics.Effects;

namespace Lunar.Client.World.Actors
{
    public class Player : IActor, ISubject
    {
        private readonly long _uniqueID;
        private readonly Camera _camera;

        private string _name;
        private float _speed;
        private int _level;
        private int _health;
        private int _maximumHealth;
        private int _strength;
        private int _intelligence;
        private int _dexterity;
        private int _defence;
        private Vector2 _position;
        private Sprite _sprite;
        private ActorStates _state;
        private bool _requestMoving;
        private Direction _direction;
        private long _nextUpdateSpritesheetTime;
        private readonly bool _mainPlayer;
        private Layer _layer;
        private Rectangle _collisionBounds;

        private PointLight _light;

        public event EventHandler<SubjectEventArgs> EventOccured;

        public long UniqueID => _uniqueID;

        public string Name => _name;

        public float Speed => _speed;

        public int Level => _level;

        public int Health => _health;

        public int MaximumHealth => _maximumHealth;

        public int Strength => _strength;

        public int Intelligence => _intelligence;

        public int Dexterity => _dexterity;

        public int Defence => _defence;

        public ActorStates State { get { return _state; } set { _state = value; } }

        public Light Light => _light;

        public Vector2 Position
        {
            get => _position;
            set
            {
                _position = value;


                if (_sprite != null)
                {
                    _sprite.Position = value;

                    _requestMoving = false;

                    if (_mainPlayer)
                        _camera.Position = new Vector2(this.Position.X + (_sprite.SourceRectangle.Width / 2f) - (Settings.ResolutionX / 2f), 
                            this.Position.Y + (_sprite.SourceRectangle.Height / 2f) - (Settings.ResolutionY / 2f));
                }
            }
        }

        public Sprite Sprite => _sprite;

        public Layer Layer
        {
            get
            {
                return _layer;
            }
            set
            {
                _layer = value;
                _sprite.LayerDepth = _layer.ZIndex + .01f;   // We add .01f to ensure the player is always drawn slightly above the layer on which they exist.
            }
        }

        public bool InChat { get; set; }

        public Direction Direction
        {
            get => _direction;
            set => _direction = value;
        }

        public Rectangle CollisionBounds => _collisionBounds;

        public Emitter Emitter { get; set; }

        public Player(Camera camera, long uniqueID)
        {
            _camera = camera;
            _uniqueID = uniqueID;
            _mainPlayer = true;

            _light = new PointLight
            {
                Color = Color.LightYellow,
                Scale = new Vector2(500),
                Intensity = .7f
            };
          

            this.InitalizePacketHandlers();
        }

        public Player(long uniqueID)
        {
            _uniqueID = uniqueID;
            _mainPlayer = false;

            _light = new PointLight();
            _light.Color = Color.LightYellow;
            _light.Scale = new Vector2(500);
            _light.Intensity = .7f;

            this.InitalizePacketHandlers();
        }

        private void InitalizePacketHandlers()
        {
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.PLAYER_MOVING, this.Handle_PlayerMoving);
            Client.ServiceLocator.GetService<NetHandler>().AddPacketHandler(PacketType.PLAYER_STATS, this.Handle_PlayerStats);
        }

        private void Handle_PlayerStats(PacketReceivedEventArgs args)
        {
            long uniqueID = args.Message.ReadInt64();

            if (this.UniqueID != uniqueID)
                return;

            _speed = args.Message.ReadFloat();
            _level = args.Message.ReadInt32();
            _health = args.Message.ReadInt32();
            _maximumHealth = args.Message.ReadInt32();
            _strength = args.Message.ReadInt32();
            _intelligence = args.Message.ReadInt32();
            _dexterity = args.Message.ReadInt32();
            _defence = args.Message.ReadInt32();

            this.EventOccured?.Invoke(this, new SubjectEventArgs("playerUpdated"));
        }

        public void Handle_PlayerMoving(PacketReceivedEventArgs args)
        {
            long uniqueID = args.Message.ReadInt64();

            if (this.UniqueID != uniqueID)
                return;

            _requestMoving = false;

            this.Direction = (Direction)args.Message.ReadByte();
            this.State = (ActorStates)args.Message.ReadByte();
            this.Position = new Vector2(args.Message.ReadFloat(), args.Message.ReadFloat());
            this.Sprite.SourceRectangle = new Rectangle(52, (int)this.Direction * 72, 52, 72);
        }

        public bool CanMove(float delta, Direction direction)
        {
            var canMove = true;

            switch (direction)
            {
                case Direction.Right:
                    if (this.Layer.CheckCollision(new Vector2(this.Position.X + delta, this.Position.Y), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Left:
                    if (this.Layer.CheckCollision(new Vector2(this.Position.X - delta, this.Position.Y), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Up:
                    if (this.Layer.CheckCollision(new Vector2(this.Position.X, this.Position.Y - delta), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Down:
                    if (this.Layer.CheckCollision(new Vector2(this.Position.X, this.Position.Y + delta), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;
            }

            return canMove;
        }

        private void CheckInput(GameTime gameTime)
        {
            // Don't spam the server with movement requests
            if (!_requestMoving && !this.InChat)
            {
                KeyboardState keyboardState = Keyboard.GetState();

                if (keyboardState.IsKeyDown(Keys.A))
                {
                    if (_direction != Direction.Left || this.State != ActorStates.Moving)
                    {
                        if (this.CanMove(gameTime.ElapsedGameTime.Milliseconds, Direction.Left))
                        {
                            var packet = new Packet(PacketType.PLAYER_MOVING);
                            packet.Message.Write((byte)Direction.Left);
                            packet.Message.Write(true);
                            Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                            _requestMoving = true;
                        }
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.D))
                {
                    if (_direction != Direction.Right || this.State != ActorStates.Moving)
                    {
                        if (this.CanMove(gameTime.ElapsedGameTime.Milliseconds, Direction.Right))
                        {
                            var packet = new Packet(PacketType.PLAYER_MOVING);
                            packet.Message.Write((byte)Direction.Right);
                            packet.Message.Write(true);
                            Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                            _requestMoving = true;
                        }

                    }
                }
                else if (keyboardState.IsKeyDown(Keys.W))
                {
                    if (_direction != Direction.Up || this.State != ActorStates.Moving)
                    {
                        if (this.CanMove(gameTime.ElapsedGameTime.Milliseconds, Direction.Up))
                        {
                            var packet = new Packet(PacketType.PLAYER_MOVING);
                            packet.Message.Write((byte)Direction.Up);
                            packet.Message.Write(true);
                            Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                            _requestMoving = true;
                        }
                    }
                }
                else if (keyboardState.IsKeyDown(Keys.S))
                {
                    if (_direction != Direction.Down || this.State != ActorStates.Moving)
                    {
                        if (this.CanMove(gameTime.ElapsedGameTime.Milliseconds, Direction.Down))
                        {
                            var packet = new Packet(PacketType.PLAYER_MOVING);
                            packet.Message.Write((byte)Direction.Down);
                            packet.Message.Write(true);
                            Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                            _requestMoving = true;
                        }
                    }
                }
                else
                {
                    if (this.State == ActorStates.Moving)
                    {
                        var packet = new Packet(PacketType.PLAYER_MOVING);
                        packet.Message.Write((byte)_direction);
                        packet.Message.Write(false);
                        Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);

                        _requestMoving = true;
                    }
                }

                if (keyboardState.IsKeyDown(Keys.LeftControl))
                {
                    var packet = new Packet(PacketType.PLAYER_INTERACT);
                    Client.ServiceLocator.GetService<NetHandler>().SendMessage(packet.Message, NetDeliveryMethod.ReliableOrdered, ChannelType.UNASSIGNED);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (_mainPlayer)
            {
                this.CheckInput(gameTime);
            }

            if (this.State == ActorStates.Moving)
            {
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
                    this.Sprite.SourceRectangle = this.Sprite.SourceRectangle.Left + 52 < _sprite.Texture.Width ? new Rectangle(this.Sprite.SourceRectangle.Left + 52, (int)this.Direction * 72, 52, 72)
                        : new Rectangle(0, (int)this.Direction * 72, 52, 72);

                    _nextUpdateSpritesheetTime = (long)gameTime.TotalGameTime.TotalMilliseconds + (long)((72 / this.Speed) / (_sprite.Texture.Width / 52f));
                }

                this.Position = new Vector2(this.Position.X + dX, this.Position.Y + dY);
            }

            
            _light.Position = new Vector2(this.Position.X + (_sprite.SourceRectangle.Width/2f) - _light.Radius/2f, this.Position.Y + (_sprite.SourceRectangle.Height / 2f) - _light.Radius / 2f);

            this.Emitter?.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(this.Sprite);

            this.Emitter?.Draw(spriteBatch);
        }

        public void Unpack(NetBuffer buffer, ContentManager contentManager)
        {
            _name = buffer.ReadString();
            _speed = buffer.ReadFloat();
            _level = buffer.ReadInt32();
            _health = buffer.ReadInt32();
            _maximumHealth = buffer.ReadInt32();
            _strength = buffer.ReadInt32();
            _intelligence = buffer.ReadInt32();
            _dexterity = buffer.ReadInt32();
            _defence = buffer.ReadInt32();
            this.Position = new Vector2(buffer.ReadFloat(), buffer.ReadFloat());

            if (_sprite == null)
            {
                _sprite = new Sprite(
                    contentManager.Load<Texture2D>(Constants.FILEPATH_GFX + "Characters/" + buffer.ReadString()));
            }
            else
            {
                _sprite.Texture =
                    contentManager.Load<Texture2D>(Constants.FILEPATH_GFX + "Characters/" + buffer.ReadString());
            }

            _sprite.Position = this.Position;
            _sprite.SourceRectangle = new Rectangle(52, (int)this.Direction * 72, 52, 72);
            _collisionBounds = new Rectangle(buffer.ReadInt32(), buffer.ReadInt32(), buffer.ReadInt32(), buffer.ReadInt32());

            var layerName = buffer.ReadString();
            this.Layer = Client.ServiceLocator.GetService<WorldManager>().Map.GetLayer(layerName);

            _requestMoving = false;

            if (_mainPlayer)
                _camera.Position = new Vector2(this.Position.X + (_sprite.SourceRectangle.Width / 2f) - (Settings.ResolutionX / 2f), this.Position.Y + (_sprite.SourceRectangle.Height / 2f) - (Settings.ResolutionY / 2f));
        }
    }
}