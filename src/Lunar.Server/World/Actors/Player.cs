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
using Lunar.Server.Content.Graphics;
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
using Lunar.Server.Utilities.Commands;
using Lunar.Server.World.Actors.PacketHandlers;

namespace Lunar.Server.World.Actors
{
    public class Player : IActor
    {
        private readonly PlayerDescriptor _descriptor;
        private readonly PlayerConnection _connection;
        private readonly Inventory _inventory;
        private readonly Equipment _equipment;
        private readonly PlayerPacketHandler _packetHandler;

        private IActor _lastAttacker;

        private Rect _collisionBounds;

        private Map _map;

        // Boosted stats
        private int _speedBoost;
        private int _strengthBoost;
        private int _intelBoost;
        private int _dexBoost;
        private int _defBoost;

        private Dictionary<string, List<ScriptAction>> _eventHandlers;

        public event EventHandler<SubjectEventArgs> EventOccured;

        public string Name => _descriptor.Name;

        public Map Map => _map;

        public string MapID => _map != null ? _map.Name : _descriptor.MapID;

        public long UniqueID => _connection.UniqueIdentifier;

        public Role Role => _descriptor.Role;

        public SpriteSheet SpriteSheet
        {
            get => _descriptor.SpriteSheet;
            set => _descriptor.SpriteSheet = value;
        }

        public Layer Layer { get; set; }

        public bool MapLoaded { get; set; }

        public Inventory Inventory => _inventory;

        public Equipment Equipment => _equipment;

        public PlayerConnection Connection => _connection;

        public IActor Target { get; set; }

        /// <summary>
        /// Last actor to deal damage to this player.
        /// </summary>
        public IActor LastAttacker => _lastAttacker;

        public float Speed
        {
            get => _descriptor.Speed + _speedBoost;
            set
            {
                _descriptor.Speed = value;
                this.SendPlayerStats();
            }
        }

        public int Level
        {
            get => _descriptor.Level;
            set
            {
                if (value > Settings.MaxLevel)
                    return;

                _descriptor.Level = value;
                this.SendPlayerStats();

                this.SendChatMessage($"Congratulations, {this.Name}! You are now level {this.Level}", ChatMessageType.Announcement);
            }
        }

        public int Experience
        {
            get => _descriptor.Experience;
            set
            {
                _descriptor.Experience = value;

                if (this.Level + 1 < Settings.ExperienceThreshhold.Length && _descriptor.Experience > Settings.ExperienceThreshhold[this.Level + 1])
                {
                    this.Level++;
                }
            }
        }

        public int Strength
        {
            get => _descriptor.Strength + _strengthBoost;
            set
            {
                _descriptor.Strength = value;
                this.SendPlayerStats();
            }
        }

        public int Intelligence
        {
            get => _descriptor.Intelligence + _intelBoost;
            set
            {
                _descriptor.Intelligence = value;
                this.SendPlayerStats();
            }
        }

        public int Dexterity
        {
            get => _descriptor.Dexterity + _dexBoost;
            set
            {
                _descriptor.Dexterity = value;
                this.SendPlayerStats();
            }
        }

        public int Defense
        {
            get => _descriptor.Defense + _defBoost;
            set
            {
                _descriptor.Defense = value;
                this.SendPlayerStats();
            }
        }

        /// <summary>
        /// Returns whether the client is currently still in the loading screen
        /// </summary>
        public bool InLoadingScreen => !this.MapLoaded;

        public bool Attackable => this.MapLoaded;

        public ActorStates State { get; set; }

        public int Health
        {
            get => _descriptor.Health;
            set
            {
                _descriptor.Health = value;
               
                this.SendPlayerStats();
            }
        }

        public int MaximumHealth
        {
            get => _descriptor.MaximumHealth;
            set => _descriptor.MaximumHealth = value;
        }

        public Rect CollisionBounds
        {
            get => _collisionBounds;
            set => _collisionBounds = value;
        }

        public Vector Position
        {
            get => _descriptor.Position;
            set => _descriptor.Position = value;
        }

        public Direction Direction { get; set; }

        public ActorBehaviorDefinition BehaviorDefinition => _descriptor.BehaviorDefinition;

        public Player(PlayerDescriptor descriptor, PlayerConnection connection)
        {
            _descriptor = descriptor;
            _connection = connection;
            this.State = ActorStates.Idle;

            this.CollisionBounds = new Rect(16, 52, 16, 20);

            _inventory = new Inventory(this);
            _equipment = new Equipment(this);
            _packetHandler = new PlayerPacketHandler(this);

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
            this.Health -= amount;

            if (this.Health <= 0)
            {
                this.OnDeath();
            }
        }

        public void OnAttacked(IActor attacker, int damageDelt)
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

                _strengthBoost += item.Strength;
                _intelBoost += item.Intelligence;
                _defBoost += item.Defence;
                _dexBoost += item.Dexterity;
            }

            this.SendPlayerStats();
        }

        public void SendEquipmentUpdate()
        {
            var packet = new Packet(PacketType.EQUIPMENT_UPDATE, ChannelType.UNASSIGNED);
            
            for (int i = 0; i < Enum.GetNames(typeof(EquipmentSlots)).Length; i++)
            {
                if (this.Equipment.GetSlot(i) == null)
                {
                    // There's nothing in this slot.
                    packet.Message.Write(false);
                    continue;
                }

                packet.Message.Write(true);
                packet.Message.Write(this.Equipment.GetSlot(i).PackData());
            }

            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
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

        public void SendPlayerStats()
        {
            var packet = new Packet(PacketType.PLAYER_STATS, ChannelType.UNASSIGNED);
            packet.Message.Write(this.UniqueID);
            packet.Message.Write(this.Speed);
            packet.Message.Write(this.Level);
            packet.Message.Write(this.Health);
            packet.Message.Write(this.MaximumHealth);
            packet.Message.Write(this.Strength + _strengthBoost);
            packet.Message.Write(this.Intelligence + _intelBoost);
            packet.Message.Write(this.Dexterity + _dexBoost);
            packet.Message.Write(this.Defense + _defBoost);
            this.Map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerData()
        {
            var packet = new Packet(PacketType.PLAYER_DATA, ChannelType.UNASSIGNED);
            packet.Message.Write(this.Pack());
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void WarpTo(Vector position)
        {
            this.Position = position;

            this.Layer.OnPlayerWarped(this);

            var packet = new Packet(PacketType.POSITION_UPDATE, ChannelType.UNASSIGNED);
            packet.Message.Write(this.UniqueID);
            packet.Message.Write(this.Layer.Name);
            packet.Message.Write(this.Position);
            _map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);

            this.OnEvent("moved");
        }

        public void SendInventoryUpdate()
        {
            var packet = new Packet(PacketType.INVENTORY_UPDATE, ChannelType.UNASSIGNED);
            
            for (int i = 0; i < Settings.MaxInventoryItems; i++)
            {
                if (this.Inventory.GetSlot(i) != null)
                {
                    packet.Message.Write(true); // there is an item in this slot

                    packet.Message.Write(this.Inventory.GetSlot(i).Item.PackData());
                    packet.Message.Write(this.Inventory.GetSlot(i).Amount);
                }
                else
                {
                    packet.Message.Write(false);
                }
            }

            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

      
        public void JoinMap(Map map)
        {
            this.State = ActorStates.Idle;

            this.Layer = map.Layers.ElementAt(0);

            this.SendLoadingScreen();
            
            this.MapLoaded = false;
           
            this.Map?.OnPlayerQuit(this);
            _map = map;

            this.Map?.OnPlayerJoined(this);

            this.OnEvent("joinedMap");
        }

        public void JoinGame(Map map)
        {
            this.JoinMap(map);

            var packet = new Packet(PacketType.AVAILABLE_COMMANDS, ChannelType.UNASSIGNED);
            packet.Message.Write(Server.ServiceLocator.GetService<CommandHandler>().Pack());
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);

            this.OnEvent("joinedGame");
        }

        public void SendLoadingScreen(bool active = true)
        {
            var packet = new Packet(PacketType.LOADING_SCREEN, ChannelType.UNASSIGNED);
            packet.Message.Write(active);
            this.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void LeaveGame()
        {
            this.Save();
            _map.OnPlayerQuit(this);

            this.OnEvent("quitGame");
        }

        public bool CanMove(float delta)
        {
            var canMove = true;
         
            switch (this.Direction)
            {
                case Direction.Right:
                    if (this.Layer.CheckCollision(new Vector(this.Position.X + delta, this.Position.Y), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Left:
                    if (this.Layer.CheckCollision(new Vector(this.Position.X - delta, this.Position.Y), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Up:
                    if (this.Layer.CheckCollision(new Vector(this.Position.X, this.Position.Y - delta), _collisionBounds))
                    {
                        // The player can't move anymore.
                        canMove = false;
                    }
                    break;

                case Direction.Down:
                    if (this.Layer.CheckCollision(new Vector(this.Position.X, this.Position.Y + delta), _collisionBounds))
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
                if (this.Health <= 0)
                {
                    this.OnDeath();
                    return;
                }

                if (this.State == ActorStates.Moving)
                {
                    if (!this.CanMove(this.Speed * gameTime.UpdateTimeInMilliseconds))
                    {
                        this.State = ActorStates.Idle;

                        this.SendMovementPacket();
                    }
                    else
                    {
                        float dX = 0, dY = 0;

                        switch (this.Direction)
                        {
                            case Direction.Right:
                                dX = this.Speed * gameTime.UpdateTimeInMilliseconds;
                                break;

                            case Direction.Left:
                                dX = -this.Speed * gameTime.UpdateTimeInMilliseconds;
                                break;

                            case Direction.Up:
                                dY = -this.Speed * gameTime.UpdateTimeInMilliseconds;
                                break;

                            case Direction.Down:
                                dY = this.Speed * gameTime.UpdateTimeInMilliseconds;
                                break;
                        }

                        this.Position = new Vector(this.Position.X + dX, this.Position.Y + dY);
                        this.Layer.OnPlayerMoved(this);
                        this.OnEvent("moved");
                    }
                }
            }
        }

        public void SendMovementPacket()
        {
            var packet = new Packet(PacketType.PLAYER_MOVING, ChannelType.UNASSIGNED);
            packet.Message.Write(this.UniqueID);
            packet.Message.Write((byte)this.Direction);
            packet.Message.Write((byte)this.State); // true if moving, false if not
            packet.Message.Write(this.Position);

            _map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

     
        public void SendChatMessage(string message, ChatMessageType type)
        {
            var packet = new Packet(PacketType.PLAYER_MSG, ChannelType.UNASSIGNED);

            packet.Message.Write((byte)type);
            packet.Message.Write(message);

            this.SendPacket(packet, NetDeliveryMethod.Unreliable);
        }

        public void SendPacket(Packet packet, NetDeliveryMethod method)
        {
            _connection.SendPacket(packet, method);
        }

        public void Save()
        {
            _descriptor.Save();
        }

        public NetBuffer Pack()
        {
            var buffer = new NetBuffer();

            buffer.Write(this.UniqueID);
            buffer.Write(this.Name);
            buffer.Write(this.Speed);
            buffer.Write(this.Level);
            buffer.Write(this.Experience);

            buffer.Write(Settings.ExperienceThreshhold.Length > this.Level + 1
                ? Settings.ExperienceThreshhold[this.Level + 1]
                : 0);

            buffer.Write(this.Health);
            buffer.Write(this.MaximumHealth);
            buffer.Write(this.Strength);
            buffer.Write(this.Intelligence);
            buffer.Write(this.Dexterity);
            buffer.Write(this.Defense);
            buffer.Write(this.Position);
            buffer.Write(this.SpriteSheet.Pack());
            buffer.Write(this.CollisionBounds);
            buffer.Write(this.Layer.Name);

            return buffer;
        }
    }
}