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
using Lunar.Core.Utilities.Logic;
using Lunar.Core.World;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities;
using Lunar.Server.World.Structure;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;

namespace Lunar.Server.World.Actors
{
    public sealed class NPC : IActor<NPCDefinition>
    {
        private Map _map;
        private List<Vector> _targetPath;
        private long _nextAttackTime;
        private Random _random;
        private long _nextMoveTime;
        private NPCDefinition _definition;

        private int _health;
        public SpriteInfo Sprite { get; set; }

        public NPCDefinition Descriptor => _definition;


        public Layer Layer { get; set; }

        public long UniqueID { get; }


        public Direction Direction { get; set; }

        public bool Aggrevated { get; set; }

        public ActorStates State { get; set; }

        public Map Map => _map;

        public IActor<IActorDescriptor> Target { get; set; }

        public bool Attackable => true;

        public bool Alive => this.Descriptor.Stats.Health > 0;

        public ActorBehaviorDefinition BehaviorDefinition => this.Descriptor.BehaviorDefinition;

        public event EventHandler<SubjectEventArgs> EventOccured;

        public GameTimerManager GameTimerManager { get; }


        public NPC(NPCDefinition definition, Map map)
        {
            if (definition == null)
            {
                Logger.LogEvent($"Null npc spawned on map {map.Descriptor.Name}!", LogTypes.ERROR, Environment.StackTrace);
                definition = new NPCDefinition(NPCDescriptor.Create());
            }

            this.GameTimerManager = new GameTimerManager();

            _map = map;
            _definition = definition;

            this.Sprite = new SpriteInfo(this.Descriptor.TexturePath);
            this.Layer = map.Layers.ElementAt(0);
            
            _random = new Random();

            this.UniqueID = Guid.NewGuid().GetHashCode();
            _targetPath = new List<Vector>();

            _map.AddActor(this);


            var npcDataPacket = new Packet(PacketType.NPC_DATA, ChannelType.UNASSIGNED);
            npcDataPacket.Message.Write(this.Pack());
            _map.SendPacket(npcDataPacket, NetDeliveryMethod.ReliableOrdered);

            try
            {
                this.Descriptor.BehaviorDefinition?.OnCreated(this);
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Error handling OnCreated: " + ex.Message, LogTypes.ERROR, ex.StackTrace);
            }
            
        }

        

        public void OnAttacked(IActor<IActorDescriptor> attacker, int damageDelt)
        {
            try
            {
                this.Descriptor.BehaviorDefinition?.Attacked(this, attacker, damageDelt);
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Error handling OnAttacked: " + ex.Message, LogTypes.ERROR, ex.StackTrace);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (this.Target != null && _targetPath.Count == 0)
            {
                this.GoTo(this.Target);
            }

            this.ProcessMovement(gameTime);
            this.ProcessCombat(gameTime);

            try
            {
                this.Descriptor.BehaviorDefinition?.Update(this, gameTime);
            }
            catch (Exception ex)
            {
                Logger.LogEvent("Error handling Update: " + ex.Message, LogTypes.ERROR, ex.StackTrace);
            }
        }

        private void ProcessCombat(GameTime gameTime)
        {
            if (_nextAttackTime > gameTime.TotalElapsedTime || this.Target == null || !this.Aggrevated || !this.Target.Attackable) return;

            Vector posDiff = this.Descriptor.Position - this.Target.Descriptor.Position;

            posDiff = new Vector(Math.Abs(posDiff.X), Math.Abs(posDiff.Y));

            if (posDiff.X.IsWithin(0, this.Descriptor.AttackRange)
                && posDiff.Y.IsWithin(0, this.Descriptor.AttackRange))
            {

                if (this.Target is Player player)
                {
                    if (player.InLoadingScreen)
                        return;
                }

                int damageDelt = 0;
                try
                {
                    damageDelt = (int)this.Descriptor.BehaviorDefinition?.Attack(this, this.Target);
                }
                catch (Exception ex)
                {
                    Logger.LogEvent("Error handling Attack: " + ex.Message, LogTypes.ERROR, ex.StackTrace);
                }

                try
                {
                    this.Target.OnAttacked(this, Convert.ToInt32(damageDelt));
                }
                catch (Exception ex)
                {
                    Logger.LogEvent("Error handling OnAttacked: " + ex.Message, LogTypes.ERROR, ex.StackTrace);
                }

                _nextAttackTime = gameTime.TotalElapsedTime + 1000;
            }
        }

        public bool HasTarget()
        {
            return (this.Target != null);
        }

        public void GoTo(Vector targetDest)
        {
            _targetPath = this.Map.GetPathfinder(this.Layer).FindPath(this.Descriptor.Position, targetDest);
            _targetPath.Reverse();

            this.State = ActorStates.Moving;

            if (_targetPath.Count > 0)
            {
                this.SendMovementPacket(_targetPath);
            }
        }

        public void GoTo(IActor<IActorDescriptor> target)
        {
            float targetX = target.Descriptor.Position.X;
            float targetY = target.Descriptor.Position.Y;

            if (targetX < this.Descriptor.Position.X)
            {
                targetX -= this.Descriptor.CollisionBounds.Width;
            }
            else
            {
                targetX += this.Descriptor.CollisionBounds.Width;
            }

            if (targetY < this.Descriptor.Position.Y)
            {
                targetY -= this.Descriptor.CollisionBounds.Height;
            }
            else
            {
                targetY += this.Descriptor.CollisionBounds.Height;
            }

            // Don't mother moving if we're not within range of the target
            if (!this.WithinRangeOf(target))
                this.GoTo(new Vector(targetX, targetY));
        }

        /// <summary>
        /// Returns True if the player is within range of the specified actor.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool WithinRangeOf(IActor<IActorDescriptor> actor)
        {
            // We consider within range to be either 1 tile to the right or left of the player. This of course will correspond to whatever tile size is set within the engine.
            Rect collisionBoundsRight = new Rect(this.Descriptor.Position.X + this.Descriptor.CollisionBounds.Left + Settings.TileSize, this.Descriptor.Position.Y + this.Descriptor.CollisionBounds.Top,
                this.Descriptor.CollisionBounds.Width, this.Descriptor.CollisionBounds.Height);

            Rect collisionBoundsLeft = new Rect(this.Descriptor.Position.X + this.Descriptor.CollisionBounds.Left - Settings.TileSize, this.Descriptor.Position.Y + this.Descriptor.CollisionBounds.Top,
                this.Descriptor.CollisionBounds.Width, this.Descriptor.CollisionBounds.Height);

            return (collisionBoundsRight.Intersects(actor.Descriptor.CollisionBounds) || collisionBoundsLeft.Intersects(actor.Descriptor.CollisionBounds));
        }

        private void ProcessMovement(GameTime gameTime)
        {
            if (_targetPath.Count > 0)
            {
                var targetDest = _targetPath[_targetPath.Count - 1];

                if (targetDest.X < this.Descriptor.Position.X)
                {
                    this.UpdateMovement(Direction.Left, gameTime);

                    if (targetDest.X >= this.Descriptor.Position.X)
                    {

                        this.Descriptor.Position = new Vector(targetDest.X, this.Descriptor.Position.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);

                    }
                }
                else if (targetDest.X > this.Descriptor.Position.X)
                {
                    this.UpdateMovement(Direction.Right, gameTime);

                    if (targetDest.X <= this.Descriptor.Position.X)
                    {
                        this.Descriptor.Position = new Vector(targetDest.X, this.Descriptor.Position.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);

                    }
                }
                else if (targetDest.Y < this.Descriptor.Position.Y)
                {
                    this.UpdateMovement(Direction.Up, gameTime);

                    if (targetDest.Y >= this.Descriptor.Position.Y)
                    {
                        this.Descriptor.Position = new Vector(this.Descriptor.Position.X, targetDest.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);

                    }
                }
                else if (targetDest.Y > this.Descriptor.Position.Y)
                {
                    this.UpdateMovement(Direction.Down, gameTime);

                    if (targetDest.Y <= this.Descriptor.Position.X)
                    {
                        this.Descriptor.Position = new Vector(this.Descriptor.Position.X, targetDest.Y);

                        // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                        // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                        // if it encounters a blocked tile in its way.
                        if (_targetPath.Count > 0)
                            _targetPath.RemoveAt(_targetPath.Count - 1);

                    }
                }
                else
                {
                    // Check to make sure that our target path wasn't cleared due to a blocked path whilst moving.
                    // The reason that we have to check for this AGAIN is due to the fact that the method Move can clear the target path
                    // if it encounters a blocked tile in its way.
                    if (_targetPath.Count > 0)
                        _targetPath.RemoveAt(_targetPath.Count - 1);
                }

                if (_targetPath.Count == 0 && _nextMoveTime <= gameTime.TotalElapsedTime)
                {
                    _nextMoveTime = gameTime.TotalElapsedTime + Settings.NPCRestPeriod;
                    this.State = ActorStates.Idle;
                    this.SendMovementPacket();
                }
            }
        }

        private void UpdateMovement(Direction direction, GameTime gameTime)
        {
            this.Direction = direction;
            
            float dX = 0, dY = 0;

            switch (this.Direction)
            {
                case Direction.Right:
                    dX = this.Descriptor.Speed * gameTime.UpdateTimeInMilliseconds;
                    break;

                case Direction.Left:
                    dX = -this.Descriptor.Speed * gameTime.UpdateTimeInMilliseconds;
                    break;

                case Direction.Up:
                    dY = -this.Descriptor.Speed * gameTime.UpdateTimeInMilliseconds;
                    break;

                case Direction.Down:
                    dY = this.Descriptor.Speed * gameTime.UpdateTimeInMilliseconds;
                    break;
            }

            this.Descriptor.Position = new Vector(this.Descriptor.Position.X + dX, this.Descriptor.Position.Y + dY);
        }

        public T FindTarget<T>() where T : IActor<IActorDescriptor>
        {
            foreach (var actor in _map.GetActors<T>())
            {
                if (!actor.Attackable)
                {
                    continue;
                }

                if (actor.Descriptor.Position.X >= this.Descriptor.Position.X - this.Descriptor.AggresiveRange * Settings.TileSize && actor.Descriptor.Position.X <= this.Descriptor.Position.X + this.Descriptor.AggresiveRange * Settings.TileSize &&
                    actor.Descriptor.Position.Y >= this.Descriptor.Position.Y - this.Descriptor.AggresiveRange * Settings.TileSize && actor.Descriptor.Position.Y <= this.Descriptor.Position.Y + this.Descriptor.AggresiveRange * Settings.TileSize)
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
            packet.Message.Write(this.Descriptor.Position);

            _map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void WarpTo(Vector position)
        {
            this.Descriptor.Position = position;
            var npcDataPacket = new Packet(PacketType.NPC_DATA, ChannelType.UNASSIGNED);
            npcDataPacket.Message.Write(this.Pack());
            _map.SendPacket(npcDataPacket, NetDeliveryMethod.ReliableOrdered);

            this.EventOccured?.Invoke(this, new SubjectEventArgs("moved", null));
        }

        public NetBuffer Pack()
        {
            var netBuffer = new NetBuffer();

            netBuffer.Write(this.UniqueID);
            netBuffer.Write(this.Descriptor.Name);
            netBuffer.Write(this.Sprite.TextureName);
            netBuffer.Write(this.Descriptor.Speed);
            netBuffer.Write(this.Descriptor.Stats.Health);
            netBuffer.Write(this.Descriptor.Stats.MaximumHealth);
            netBuffer.Write(this.Descriptor.Level);
            netBuffer.Write(this.Descriptor.Position.X);
            netBuffer.Write(this.Descriptor.Position.Y);
            netBuffer.Write(this.Descriptor.FrameSize);
            netBuffer.Write(this.Descriptor.CollisionBounds);
            netBuffer.Write(this.Layer.Descriptor.Name);

            return netBuffer;
        }

        public event EventHandler Died;
    }
}
