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
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.Utilities.Scripting;
using Lunar.Server.World.BehaviorDefinition;
using Lunar.Server.World.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.Utilities;
using Lunar.Core.Utilities.Data;
using Lunar.Core.World;
using Lunar.Core.World.Actor;
using Lunar.Core.World.Actor.Descriptors;
using Lunar.Server.Utilities.Commands;
using Lunar.Server.World.Actors.Components;
using Lunar.Server.World.Actors.PacketHandlers;

namespace Lunar.Server.World.Actors
{
    public class Player : IActor<PlayerDescriptor>
    {
        private readonly PlayerDescriptor _descriptor;
        private readonly PlayerConnection _connection;
        private readonly Inventory _inventory;
        private readonly Equipment _equipment;
        private readonly PlayerPacketHandler _packetHandler;
        private readonly PlayerNetworkComponent _networkComponent;
        private readonly ActionProcessor<Player> _actionProcessor;

        private IActor<IActorDescriptor> _lastAttacker;

        private Rect _collisionBounds;

        private Map _map;

        // Boosted stats
        private int _speedBoost;
        private int _strengthBoost;
        private int _intelBoost;
        private int _dexBoost;
        private int _defBoost;

        private Dictionary<string, List<ScriptAction>> _eventHandlers;

        public event EventHandler<EventArgs> LeftGame;

        public event EventHandler<SubjectEventArgs> EventOccured;

        public PlayerDescriptor Descriptor => _descriptor;

        public PlayerNetworkComponent NetworkComponent => _networkComponent;

        public ActionProcessor<Player> ActionProcessor => _actionProcessor;

        public Map Map => _map;

        public string MapID => _map != null ? _map.Descriptor.Name : _descriptor.MapID;

        public long UniqueID => _connection.UniqueIdentifier;

        public Layer Layer { get; set; }

        public bool MapLoaded { get; set; }

        public Inventory Inventory => _inventory;

        public Equipment Equipment => _equipment;

        public PlayerConnection Connection => _connection;

        public IActor<IActorDescriptor> Target { get; set; }

        /// <summary>
        /// Last actor to deal damage to this player.
        /// </summary>
        public IActor<IActorDescriptor> LastAttacker => _lastAttacker;

        /// <summary>
        /// Returns whether the client is currently still in the loading screen
        /// </summary>
        public bool InLoadingScreen => !this.MapLoaded;

        public bool Attackable => this.MapLoaded;

        public ActorStates State { get; set; }


        public Rect CollisionBounds
        {
            get => _collisionBounds;
            set => _collisionBounds = value;
        }

        public Direction Direction { get; set; }

        public ActorBehaviorDefinition BehaviorDefinition { get; }

        public Player(PlayerDescriptor descriptor, PlayerConnection connection)
        {
            _descriptor = descriptor;
            _connection = connection;
            this.State = ActorStates.Idle;

            this.CollisionBounds = new Rect(16, 52, 16, 20);

            _inventory = new Inventory(this);
            _equipment = new Equipment(this);
            _packetHandler = new PlayerPacketHandler(this);
            _networkComponent = new PlayerNetworkComponent(this);
            _actionProcessor = new ActionProcessor<Player>(this);

            _eventHandlers = new Dictionary<string, List<ScriptAction>>();
            _eventHandlers = new Dictionary<string, List<ScriptAction>>();

            if (this.BehaviorDefinition == null)
            {
                Logger.LogEvent("Error hooking player behavior definition!", LogTypes.ERROR, Environment.StackTrace);
            }
            else
            {
                this.BehaviorDefinition.OnCreated?.Invoke(new ScriptActionArgs(this));
                this.BehaviorDefinition.EventOccured += this.BehaviorDescriptor_EventOccured;
            }
        }

        private void OnDeath()
        {
            if (_lastAttacker != null)
            {
                _lastAttacker.Target = null;
            }

            this.BehaviorDefinition.OnDeath?.Invoke(new ScriptActionArgs(this));
        }

        public void InflictDamage(int amount)
        {
            this.Descriptor.Stats.Health -= amount;

            if (this.Descriptor.Stats.Health <= 0)
            {
                this.OnDeath();
            }
        }

        public void OnAttacked(IActor<IActorDescriptor> attacker, int damageDelt)
        {
            _lastAttacker = attacker;

            this.BehaviorDefinition?.Attacked?.Invoke(new ScriptActionArgs(this, attacker, damageDelt));
        }


        private void BehaviorDescriptor_EventOccured(object sender, SubjectEventArgs e)
        {
            // Chain events that occur within the behaviordescriptor up
            this.OnEvent(e.EventName, e.Args);
        }

        public void CalculateBoostedStats()
        {
            _strengthBoost = 0;
            _intelBoost = 0;
            _defBoost = 0;
            _dexBoost = 0;

            foreach (Item item in this.Equipment.Items)
            {
                // Ignore empty equipment slots
                if (item == null)
                    continue;

                _strengthBoost += item.Descriptor.Strength;
                _intelBoost += item.Descriptor.Intelligence;
                _defBoost += item.Descriptor.Defence;
                _dexBoost += item.Descriptor.Dexterity;
            }

            this.NetworkComponent.SendPlayerStats();
        }

     



        private void OnEvent(string eventName, params object[] args)
        {
            if (_eventHandlers.ContainsKey(eventName))
                _eventHandlers[eventName].ForEach(a => a.Invoke(new ScriptActionArgs(this, args)));

            this.EventOccured?.Invoke(this, new SubjectEventArgs(eventName, args));
        }

        public void AddEventHandler(string eventName, ScriptAction handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
                _eventHandlers.Add(eventName, new List<ScriptAction>());

            _eventHandlers[eventName].Add(handler);
        }

       
       
        public void WarpTo(Vector position)
        {
            this.Descriptor.Position = position;

            this.Layer.OnPlayerWarped(this);
            
            this.NetworkComponent.SendPositionUpdate();
           
            this.OnEvent("moved");
        }

      
        public void JoinMap(Map map)
        {
            this.State = ActorStates.Idle;

            this.Layer = map.Layers.ElementAt(0);

            this.NetworkComponent.SendLoadingScreen();
            
            this.MapLoaded = false;
           
            this.Map?.OnPlayerQuit(this);
            _map = map;

            this.Map?.OnPlayerJoined(this);

            this.OnEvent("joinedMap");
        }

        public void JoinGame(Map map)
        {
            this.JoinMap(map);

            this.NetworkComponent.SendAvailableCommands();

            this.NetworkComponent.SendChatMessage(Settings.WelcomeMessage, ChatMessageType.Announcement);

            this.OnEvent("joinedGame");
        }

        public void LeaveGame()
        {
            _map.OnPlayerQuit(this);

            this.LeftGame?.Invoke(this, new EventArgs());
        }

        public bool CanMove(float delta)
        {
            var canMove = true;
         
            switch (this.Direction)
            {
                case Direction.Right:
                    if (this.Layer.CheckCollision(new Vector(this.Descriptor.Position.X + delta, this.Descriptor.Position.Y), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Left:
                    if (this.Layer.CheckCollision(new Vector(this.Descriptor.Position.X - delta, this.Descriptor.Position.Y), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Up:
                    if (this.Layer.CheckCollision(new Vector(this.Descriptor.Position.X, this.Descriptor.Position.Y - delta), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Down:
                    if (this.Layer.CheckCollision(new Vector(this.Descriptor.Position.X, this.Descriptor.Position.Y + delta), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;
            }

            return canMove;
        }

        public void Update(GameTime gameTime)
        {
            if (!this.InLoadingScreen)
            {
                _actionProcessor.Update(gameTime);

                if (this.Descriptor.Stats.Health <= 0)
                {
                    this.OnDeath();
                    return;
                }

                if (this.State == ActorStates.Moving)
                {
                    if (!this.CanMove(this.Descriptor.Speed * gameTime.UpdateTimeInMilliseconds))
                    {
                        this.State = ActorStates.Idle;

                        this.NetworkComponent.SendMovementPacket();
                    }
                    else
                    {
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
                        this.Layer.OnPlayerMoved(this);
                        this.OnEvent("moved");
                    }
                }
            }
        }


        public NetBuffer Pack()
        {
            var buffer = new NetBuffer();

            buffer.Write(this.UniqueID);
            buffer.Write(this.Descriptor.Name);
            buffer.Write(this.Descriptor.Speed);
            buffer.Write(this.Descriptor.Level);
            buffer.Write(this.Descriptor.Experience);

            buffer.Write(Settings.ExperienceThreshhold.Length > this.Descriptor.Level + 1
                ? Settings.ExperienceThreshhold[this.Descriptor.Level + 1]
                : 0);

            buffer.Write(this.Descriptor.Stats.Health);
            buffer.Write(this.Descriptor.Stats.MaximumHealth);
            buffer.Write(this.Descriptor.Stats.Strength);
            buffer.Write(this.Descriptor.Stats.Intelligence);
            buffer.Write(this.Descriptor.Stats.Dexterity);
            buffer.Write(this.Descriptor.Stats.Defense);
            buffer.Write(this.Descriptor.Position);
            buffer.Write(this.Descriptor.SpriteSheet.Pack());
            buffer.Write(this.CollisionBounds);
            buffer.Write(this.Layer.Descriptor.Name);

            return buffer;
        }
    }
}