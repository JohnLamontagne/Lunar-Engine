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
using Lunar.Server.World.Conversation;

namespace Lunar.Server.World.Actors
{
    public class Player : PlayerDescriptor, IActor
    {
        private readonly PlayerDescriptor _descriptor;
        private readonly PlayerConnection _connection;
        private readonly Inventory _inventory;
        private readonly Equipment _equipment;
        private readonly PlayerPacketHandler _packetHandler;
        private readonly PlayerNetworkComponent _networkComponent;
        private readonly ActionProcessor<Player> _actionProcessor;

        private IActor _lastAttacker;

        private Map _map;

        private Script _script;

        private Dictionary<string, List<Action<EventArgs>>> _eventHandlers;

        public event EventHandler<EventArgs> LeftGame;

        public event EventHandler<SubjectEventArgs> EventOccured;

        public PlayerDescriptor Descriptor => _descriptor;

        public PlayerNetworkComponent NetworkComponent => _networkComponent;

        public ActionProcessor<Player> ActionProcessor => _actionProcessor;

        public Map Map => _map;

        public string MapID => _map != null ? _map.Name : _descriptor.MapID;

        public string UniqueID => _connection.UniqueIdentifier.ToString();

        public Layer Layer { get; set; }

        public bool MapLoaded { get; set; }

        public Inventory Inventory => _inventory;

        public Equipment Equipment => _equipment;

        public IActor Target { get; set; }

        /// <summary>
        /// Last actor to deal damage to this player.
        /// </summary>
        public IActor LastAttacker => _lastAttacker;

        /// <summary>
        /// Returns whether the client is currently still in the loading screen
        /// </summary>
        public bool InLoadingScreen => !this.MapLoaded;

        public bool Alive => (this.Descriptor.Stats.CurrentHealth + this.Descriptor.StatBoosts.CurrentHealth) >= 0;

        public bool Attackable
        {
            get
            {
                return (this.Alive && !this.InLoadingScreen);
            }
        }

        /// <summary>
        /// Currently engaged dialogue. Setting to null flags that the player is currently not engaged in any dialogue.
        /// </summary>
        public Dialogue EngagedDialogue { get; set; }

        public ActorStates State { get; set; }

        public Direction Direction { get; set; }

        public ActorBehaviorDefinition Behavior { get; }

        public CollisionBody CollisionBody { get; }

        public Player(PlayerDescriptor descriptor, PlayerConnection connection)
        {
            _descriptor = descriptor;
            _connection = connection;
            _connection.Player = this;
            this.State = ActorStates.Idle;

            this.Descriptor.CollisionBounds = new Rect(16, 52, 16, 20);

            this.CollisionBody = new CollisionBody(this);

            _inventory = new Inventory(this);
            _equipment = new Equipment(this);
            _networkComponent = new PlayerNetworkComponent(this, connection);
            _packetHandler = new PlayerPacketHandler(this);
            _actionProcessor = new ActionProcessor<Player>(this);

            _eventHandlers = new Dictionary<string, List<Action<EventArgs>>>();

            Script script = Engine.Services.Get<ScriptManager>().CreateScript(Constants.FILEPATH_SCRIPTS + "player.py");
            _script = script;

            try
            {
                this.Behavior = script.GetVariable<ActorBehaviorDefinition>("BehaviorDefinition");
            }
            catch { }

            if (this.Behavior == null)
            {
                Engine.Services.Get<Logger>().LogEvent("Error hooking player behavior definition.", LogTypes.ERROR, new Exception("Error hooking player behavior definition."));
            }
            else
            {
                this.Behavior.OnCreated(this);
                this.Behavior.EventOccured += this.BehaviorDescriptor_EventOccured;
            }

            this.Descriptor.Stats.Changed += (o, args) =>
            {
                this.NetworkComponent.SendPlayerStats();
            };
        }

        private void OnDeath()
        {
            if (_lastAttacker != null)
            {
                _lastAttacker.Target = null;
            }

            try
            {
                this.Behavior.OnDeath(this);
            }
            catch (Exception ex)
            {
                Engine.Services.Get<Logger>().LogEvent("Error in OnDeath handling: " + ex.Message, LogTypes.ERROR, ex);
            }
        }

        public void SendChatMessage(string message, ChatMessageType messageType)
        {
            this.NetworkComponent.SendChatMessage(message, messageType);
        }

        public void InflictDamage(int amount)
        {
            this.Descriptor.Stats.CurrentHealth -= amount;

            if (this.Descriptor.Stats.CurrentHealth <= 0)
            {
                this.OnDeath();
            }
        }

        public void OnAttacked(IActor attacker, int damageDelt)
        {
            _lastAttacker = attacker;

            this.Behavior?.Attacked(this, attacker, damageDelt);
        }

        private void BehaviorDescriptor_EventOccured(object sender, SubjectEventArgs e)
        {
            // Chain events that occur within the behaviordescriptor up
            this.OnEvent(e.EventName, e.Args);
        }

        public void CalculateBoostedStats()
        {
            this.Descriptor.StatBoosts.Strength = 0;
            this.Descriptor.StatBoosts.Intelligence = 0;
            this.Descriptor.StatBoosts.Defense = 0;
            this.Descriptor.StatBoosts.Dexterity = 0;

            foreach (Item item in this.Equipment.Items)
            {
                // Ignore empty equipment slots
                if (item == null)
                    continue;

                this.Descriptor.StatBoosts.Strength += item.Descriptor.Strength;
                this.Descriptor.StatBoosts.Intelligence += item.Descriptor.Intelligence;
                this.Descriptor.StatBoosts.Defense += item.Descriptor.Defence;
                this.Descriptor.StatBoosts.Dexterity += item.Descriptor.Dexterity;
            }

            this.NetworkComponent.SendPlayerStats();
        }

        private void OnEvent(string eventName, params object[] args)
        {
            this.EventOccured?.Invoke(this, new SubjectEventArgs(eventName, args));
        }

        public void AddEventHandler(string eventName, Action<EventArgs> handler)
        {
            if (!_eventHandlers.ContainsKey(eventName))
                _eventHandlers.Add(eventName, new List<Action<EventArgs>>());

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

        public bool CanMove(float dX, float dY)
        {
            return !(this.Layer.CheckCollision(this.CollisionBody.CollisionArea.Move(dX, dY)));
        }

        public bool CanMove(Direction direction, float magnitude)
        {
            int orient = (this.Direction == Direction.Right || this.Direction == Direction.Down) ? 1 : -1;
            float delta = this.Descriptor.Speed * orient * magnitude;
            float dX = delta * ((this.Direction == Direction.Right || this.Direction == Direction.Left) ? 1 : 0);
            float dY = delta * ((this.Direction == Direction.Up || this.Direction == Direction.Down) ? 1 : 0);

            return this.CanMove(dX, dY);
        }

        private void ProcessMovement(GameTime gameTime)
        {
            int orient = (this.Direction == Direction.Right || this.Direction == Direction.Down) ? 1 : -1;
            float delta = this.Descriptor.Speed * orient * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            float dX = delta * ((this.Direction == Direction.Right || this.Direction == Direction.Left) ? 1 : 0);
            float dY = delta * ((this.Direction == Direction.Up || this.Direction == Direction.Down) ? 1 : 0);

            if (!this.CanMove(dX, dY))
            {
                this.OnEvent("stopped");

                this.State = ActorStates.Idle;

                this.NetworkComponent.SendMovementPacket();
            }
            else
            {
                this.Descriptor.Position = this.Descriptor.Position.Move(dX, dY);
                this.Layer.OnPlayerMoved(this);
                this.OnEvent("moved");
            }
        }

        public void Update(GameTime gameTime)
        {
            if (!this.InLoadingScreen)
            {
                _actionProcessor.Update(gameTime);

                if (this.Descriptor.Stats.CurrentHealth <= 0)
                {
                    this.OnDeath();
                    return;
                }

                if (this.State == ActorStates.Moving)
                {
                    this.ProcessMovement(gameTime);
                }
            }
        }

        public IActor FindTarget()
        {
            return this.FindTarget<IActor>();
        }

        public T FindTarget<T>() where T : IActor
        {
            foreach (var actor in _map.GetActors<T>())
            {
                if (actor.Equals(this))
                {
                    continue;
                }

                if (actor.CollisionBody.Collides(this.CollisionBody, this.Descriptor.Reach))
                {
                    return actor;
                }
            }

            return default(T);
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

            buffer.Write(this.Descriptor.Stats.CurrentHealth);
            buffer.Write(this.Descriptor.Stats.Health);
            buffer.Write(this.Descriptor.Stats.Strength);
            buffer.Write(this.Descriptor.Stats.Intelligence);
            buffer.Write(this.Descriptor.Stats.Dexterity);
            buffer.Write(this.Descriptor.Stats.Defense);
            buffer.Write(this.Descriptor.Position);
            buffer.Write(this.Descriptor.SpriteSheet.Pack());
            buffer.Write(this.Descriptor.CollisionBounds);
            buffer.Write(this.Layer.Name);

            return buffer;
        }
    }
}