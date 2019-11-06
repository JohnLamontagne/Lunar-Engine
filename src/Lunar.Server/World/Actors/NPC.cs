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

using System;
using Lunar.Server.Net;
using Lidgren.Network;
using System.Collections.Generic;
using System.Linq;
using Lunar.Core.Content.Graphics;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;
using Lunar.Server.World.Structure;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using Lunar.Core;
using Lunar.Server.World.Conversation;

namespace Lunar.Server.World.Actors
{
    public sealed class NPC : NPCModel, IActor
    {
        private Map _map;
        private Stack<Vector> _targetPath;
        private Random _random;
        private long _nextMoveTime;
        private List<Script> _scripts;

        private float _avgMoveSpeedX = 0;
        private float _avgMoveSpeedY = 0;

        public SpriteInfo Sprite { get; set; }

        public Layer Layer { get; set; }

        public Direction Direction { get; set; }

        public bool Moving { get; private set; }

        public Map Map => _map;

        public IActor Target { get; set; }

        public bool Attackable { get; set; }

        public bool Alive => this.Stats.Vitality > 0;

        public ActorBehaviorDefinition Behavior { get; set; }

        public ActorStateMachine<NPC> StateMachine { get; }

        public event EventHandler<SubjectEventArgs> EventOccured;

        public GameTimerManager GameTimers { get; }

        public CollisionBody CollisionBody { get; }

        public new Dialogue Dialogue { get; }

        protected NPC(NPCModel descriptor)
        {
            _scripts = new List<Script>();

            this.Name = descriptor.Name;
            this.Level = descriptor.Level;
            this.MaxRoam = descriptor.MaxRoam;
            this.Position = descriptor.Position;
            this.FrameSize = descriptor.FrameSize;
            this.UniqueID = descriptor.UniqueID;
            this.AggresiveRange = descriptor.AggresiveRange;
            this.Reach = descriptor.Reach;
            this.Speed = descriptor.Speed;
            this.TexturePath = descriptor.TexturePath;
            this.Stats.Defense = descriptor.Stats.Defense;
            this.Stats.Dexterity = descriptor.Stats.Dexterity;
            this.Stats.Vitality = descriptor.Stats.Vitality;
            this.Stats.Vitality = descriptor.Stats.Vitality;
            this.Stats.Strength = descriptor.Stats.Strength;
            this.Stats.Intelligence = descriptor.Stats.Intelligence;
            this.CollisionBounds = descriptor.CollisionBounds;
            this.DialogueBranch = descriptor.DialogueBranch;

            this.InitalizeScripts(descriptor.Scripts);

            if (!string.IsNullOrEmpty(descriptor.Dialogue))
            {
                this.Dialogue = Engine.Services.Get<DialogueManager>().Get(descriptor.Dialogue);
                this.DialogueBranch = descriptor.DialogueBranch;
            }
        }

        public NPC(NPCModel descriptor, Map map)
            : this(descriptor)
        {
            if (descriptor == null)
            {
                Engine.Services.Get<Logger>().LogEvent($"Null npc spawned on map {map.Name}!", LogTypes.ERROR, new Exception($"Null npc spawned on map {map.Name}!"));
            }

            this.GameTimers = new GameTimerManager();
            this.StateMachine = new ActorStateMachine<NPC>(this);

            this.CollisionBody = new CollisionBody(this);

            _map = map;

            this.Sprite = new SpriteInfo(this.TexturePath);
            this.Layer = map.Layers.ElementAt(0);

            _random = new Random();

            _targetPath = new Stack<Vector>();

            _map.AddActor(this);

            var npcDataPacket = new Packet(PacketType.NPC_DATA, ChannelType.UNASSIGNED);
            npcDataPacket.Message.Write(this.Pack());
            _map.SendPacket(npcDataPacket, NetDeliveryMethod.ReliableOrdered);

            try
            {
                this.Behavior?.OnCreated(this);
            }
            catch (Exception ex)
            {
                Engine.Services.Get<Logger>().LogEvent("Error handling OnCreated: " + ex.Message, LogTypes.ERROR, ex);
            }
        }

        private void InitalizeScripts(IEnumerable<string> scriptPaths)
        {
            foreach (var scriptPath in scriptPaths)
            {
                Script script = Engine.Services.Get<ScriptManager>().CreateScript(Constants.FILEPATH_DATA + scriptPath);
                ActorBehaviorDefinition behaviorDefinition = script?.GetVariable<ActorBehaviorDefinition>("BehaviorDefinition");

                if (behaviorDefinition != null)
                {
                    this.Behavior = behaviorDefinition;
                }
            }
        }

        public void OnAttacked(IActor attacker, int damageDelt)
        {
            try
            {
                this.Behavior?.Attacked(this, attacker, damageDelt);
            }
            catch (Exception ex)
            {
                Engine.Services.Get<Logger>().LogEvent("Error handling OnAttacked: " + ex.Message, LogTypes.ERROR, ex);
            }
        }

        public void Update(GameTime gameTime)
        {
            this.GameTimers.Update(gameTime);

            this.ProcessMovement(gameTime);

            try
            {
                this.Behavior?.Update(this, gameTime);
            }
            catch (Exception ex)
            {
                Engine.Services.Get<Logger>().LogEvent("Error handling Update: " + ex.Message, LogTypes.ERROR, ex);
            }

            this.StateMachine.Update(gameTime);
        }

        public bool HasTarget()
        {
            return (this.Target != null);
        }

        public bool GoTo(Vector targetDest)
        {
            var rawPath = this.Map.GetPathfinder(this.Layer).FindPath(this.Position, targetDest, this.CollisionBody);
            _targetPath = new Stack<Vector>(rawPath);

            if (_targetPath.Count > 0)
            {
                this.Moving = true;
                Console.WriteLine(rawPath[0]);
                rawPath.Reverse(); // Reverse for the client which uses a queue
                this.SendMovementPacket(rawPath);
                return true;
            }

            // Return false if we were not able to find a valid path to the target.
            return false;
        }

        public bool GoTo(IActor target)
        {
            float targetX = target.Position.X;
            float targetY = target.Position.Y;

            // Don't mother moving if we're within range of the target
            if (!this.WithinAttackingRangeOf(target))
            {
                var foundPath = this.GoTo(new Vector(targetX, targetY));

                if (!foundPath)
                {
                    this.Target = null;
                }

                return foundPath;
            }
            else if (!this.WithinRangeOf(target, this.AggresiveRange)) // Clear the target if it is no longer within range.
                this.Target = null;

            return false;
        }

        /// <summary>
        /// Returns True if the player is within range of the specified actor.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool WithinRangeOf(IActor actor, int range)
        {
            Rect collisionBoundsRight = new Rect(this.CollisionBody.CollisionArea.X, this.CollisionBody.CollisionArea.Y,
                this.CollisionBody.CollisionArea.Width + range, this.CollisionBody.CollisionArea.Height + range);

            Rect collisionBoundsLeft = new Rect(this.CollisionBody.CollisionArea.X - range, this.CollisionBody.CollisionArea.Y - range,
                this.CollisionBody.CollisionArea.Width, this.CollisionBody.CollisionArea.Height);

            return (actor.CollisionBody.Collides(collisionBoundsRight) || actor.CollisionBody.Collides(collisionBoundsLeft));
        }

        public bool WithinAttackingRangeOf(IActor actor)
        {
            return this.WithinRangeOf(actor, this.AggresiveRange);
        }

        private void ProcessMovement(GameTime gameTime)
        {
            if (_targetPath.Count > 0)
            {
                var targetDest = _targetPath.Peek();

                if (targetDest.X < this.Position.X)
                {
                    this.UpdateMovement(Direction.Left, gameTime);

                    if (targetDest.X >= this.Position.X)
                    {
                        this.Position = new Vector(targetDest.X, this.Position.Y);

                        _targetPath.Pop();
                    }
                }
                else if (targetDest.X > this.Position.X)
                {
                    this.UpdateMovement(Direction.Right, gameTime);

                    if (targetDest.X <= this.Position.X)
                    {
                        this.Position = new Vector(targetDest.X, this.Position.Y);

                        _targetPath.Pop();
                    }
                }
                else if (targetDest.Y < this.Position.Y)
                {
                    this.UpdateMovement(Direction.Up, gameTime);

                    if (targetDest.Y >= this.Position.Y)
                    {
                        this.Position = new Vector(this.Position.X, targetDest.Y);

                        _targetPath.Pop();
                    }
                }
                else if (targetDest.Y > this.Position.Y)
                {
                    this.UpdateMovement(Direction.Down, gameTime);

                    if (targetDest.Y <= this.Position.X)
                    {
                        this.Position = new Vector(this.Position.X, targetDest.Y);

                        _targetPath.Pop();
                    }
                }
                else
                {
                    _targetPath.Pop();
                }
            }
            else
            {
                // If we no longer have a path and were previously moving, switch moving to false and update the clients.
                if (this.Moving)
                {
                    this.Moving = false;
                    Console.WriteLine("Finished moving. Now at " + this.Position.ToString());
                    Console.WriteLine("Avg Update: " + new Vector(_avgMoveSpeedX, _avgMoveSpeedY).ToString());
                    _nextMoveTime = (long)gameTime.TotalGameTime.TotalMilliseconds + Settings.NPCRestPeriod;
                    this.Moving = false;
                    this.SendMovementPacket();

                    _avgMoveSpeedX = 0;
                    _avgMoveSpeedY = 0;
                }
            }
        }

        private bool CanMove(float delta)
        {
            Rect destCollisionArea;

            switch (this.Direction)
            {
                case Direction.Right:
                    destCollisionArea = new Rect(this.CollisionBody.CollisionArea.X + delta, this.CollisionBody.CollisionArea.Y, this.CollisionBounds.Width, this.CollisionBounds.Height);
                    break;

                case Direction.Left:
                    destCollisionArea = new Rect(this.CollisionBody.CollisionArea.X - delta, this.CollisionBody.CollisionArea.Y, this.CollisionBounds.Width, this.CollisionBounds.Height);
                    break;

                case Direction.Up:
                    destCollisionArea = new Rect(this.CollisionBody.CollisionArea.X, this.CollisionBody.CollisionArea.Y - delta, this.CollisionBounds.Width, this.CollisionBounds.Height);
                    break;

                case Direction.Down:
                    destCollisionArea = new Rect(this.CollisionBody.CollisionArea.X, this.CollisionBody.CollisionArea.Y + delta, this.CollisionBounds.Width, this.CollisionBounds.Height);
                    break;
            }

            return !(this.Layer.CheckCollision(destCollisionArea));
        }

        private void UpdateMovement(Direction direction, GameTime gameTime)
        {
            this.Direction = direction;

            float dX = 0, dY = 0;
            float delta = this.Speed * (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (this.CanMove(delta))
            {
                switch (this.Direction)
                {
                    case Direction.Right:
                        dX = delta;
                        break;

                    case Direction.Left:
                        dX = -delta;
                        break;

                    case Direction.Up:
                        dY = -delta;
                        break;

                    case Direction.Down:
                        dY = delta;
                        break;
                }

                if (_avgMoveSpeedX == 0)
                    _avgMoveSpeedX = dX;
                else
                    _avgMoveSpeedX = (_avgMoveSpeedX + dX) / 2;

                if (_avgMoveSpeedY == 0)
                    _avgMoveSpeedY = dY;
                else
                    _avgMoveSpeedY = (_avgMoveSpeedY + dY) / 2;

                this.Position = new Vector(this.Position.X + dX, this.Position.Y + dY);
            }
            else
            {
                this.Position = _targetPath.Peek();
            }
        }

        public T FindTarget<T>() where T : IActor
        {
            foreach (var actor in _map.GetActors<T>())
            {
                if (!actor.Attackable)
                {
                    continue;
                }

                if (actor.Position.X >= this.Position.X - this.AggresiveRange && actor.Position.X <= this.Position.X + this.AggresiveRange &&
                    actor.Position.Y >= this.Position.Y - this.AggresiveRange && actor.Position.Y <= this.Position.Y + this.AggresiveRange)
                {
                    return actor;
                }
            }

            return default(T);
        }

        public Player FindPlayerTarget()
        {
            return this.FindTarget<Player>();
        }

        public NPC FindNPCTarget()
        {
            return this.FindTarget<NPC>();
        }

        private void SendMovementPacket(List<Vector> targetPath)
        {
            var packet = new Packet(PacketType.NPC_MOVING, ChannelType.UNASSIGNED);
            packet.Message.Write(this.UniqueID);
            packet.Message.Write(true);
            packet.Message.Write((int)this.Direction);

            packet.Message.Write(targetPath.Count);
            foreach (var pos in targetPath)
            {
                packet.Message.Write(pos);
            }

            _map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        /// <summary>
        /// Sends a packet telling clients that the NPC is no longer moving.
        /// </summary>
        private void SendMovementPacket()
        {
            var packet = new Packet(PacketType.NPC_MOVING, ChannelType.UNASSIGNED);
            packet.Message.Write(this.UniqueID);
            packet.Message.Write(false);
            packet.Message.Write((int)this.Direction);
            packet.Message.Write(this.Position);

            _map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void WarpTo(Vector position)
        {
            this.Position = position;
            var npcDataPacket = new Packet(PacketType.NPC_DATA, ChannelType.UNASSIGNED);
            npcDataPacket.Message.Write(this.Pack());
            _map.SendPacket(npcDataPacket, NetDeliveryMethod.ReliableOrdered);

            this.EventOccured?.Invoke(this, new SubjectEventArgs("moved", null));
        }

        public NetBuffer Pack()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.UniqueID);
            netBuffer.Write(this.Name);
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write(this.Speed);
            netBuffer.Write(this.Stats.Vitality);
            netBuffer.Write(this.Stats.Vitality);
            netBuffer.Write(this.Level);
            netBuffer.Write(this.Position.X);
            netBuffer.Write(this.Position.Y);
            netBuffer.Write(this.FrameSize);
            netBuffer.Write(this.CollisionBounds);
            netBuffer.Write(this.Layer.Name);

            return netBuffer;
        }

        public event EventHandler Died;
    }
}