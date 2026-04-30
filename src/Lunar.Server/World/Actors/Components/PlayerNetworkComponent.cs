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
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Server.Net;
using Lunar.Server.Utilities.Commands;

namespace Lunar.Server.World.Actors.Components
{
    public class PlayerNetworkComponent
    {
        private readonly Player _player;

        public PlayerConnection Connection { get; }

        public PlayerNetworkComponent(Player player, PlayerConnection connection)
        {
            _player = player;
            this.Connection = connection;
        }

        public void SendAvailableCommands()
        {
            var packet = new Packet();
            packet.Write(Engine.Services.Get<CommandHandler>().Pack());
            this.SendPacket(PacketType.AVAILABLE_COMMANDS, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendPositionUpdate()
        {
            var packet = new Packet();
            packet.Write(_player.UniqueID);
            packet.Write(_player.Layer.Name);
            packet.Write(_player.Descriptor.Position);
            _player.Map.SendPacket(PacketType.POSITION_UPDATE, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerData()
        {
            var packet = new Packet();
            packet.Write(_player.Pack());
            this.SendPacket(PacketType.PLAYER_DATA, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendChatMessage(string message, ChatMessageType type)
        {
            var packet = new Packet();
            packet.Write((byte)type);
            packet.Write(message);
            this.SendPacket(PacketType.PLAYER_MSG, packet, DeliveryMethod.Unreliable);
        }

        public void SendPacket(PacketType packetType, Packet packet, DeliveryMethod deliveryMethod)
        {
            this.Connection.SendPacket(packetType, packet, deliveryMethod);
        }

        public void SendPlayerStats()
        {
            var packet = new Packet();
            packet.Write(_player.UniqueID);
            packet.Write(_player.Descriptor.Speed);
            packet.Write(_player.Descriptor.Level);
            packet.Write(_player.Descriptor.Stats.Vitality);
            packet.Write(_player.Descriptor.Stats.Vitality);
            packet.Write(_player.Descriptor.Stats.Strength + _player.Descriptor.StatBoosts.Strength);
            packet.Write(_player.Descriptor.Stats.Intelligence + _player.Descriptor.StatBoosts.Intelligence);
            packet.Write(_player.Descriptor.Stats.Dexterity + _player.Descriptor.StatBoosts.Dexterity);
            packet.Write(_player.Descriptor.Stats.Defense + _player.Descriptor.StatBoosts.Defense);
            _player.Map.SendPacket(PacketType.PLAYER_STATS, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendInventoryUpdate()
        {
            var packet = new Packet();

            for (int i = 0; i < Settings.MaxInventoryItems; i++)
            {
                if (_player.Inventory.GetSlot(i) != null)
                {
                    packet.Write(true);
                    packet.Write(_player.Inventory.GetSlot(i).Item.PackData());
                    packet.Write(_player.Inventory.GetSlot(i).Amount);
                }
                else
                {
                    packet.Write(false);
                }
            }

            this.SendPacket(PacketType.INVENTORY_UPDATE, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendMovementPacket()
        {
            var packet = new Packet();
            packet.Write(_player.UniqueID);
            packet.Write((byte)_player.Direction);
            packet.Write((byte)_player.State);
            packet.Write(_player.Descriptor.Position);
            _player.Map.SendPacket(PacketType.PLAYER_MOVING, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendLoadingScreen(bool active = true)
        {
            var packet = new Packet();
            packet.Write(active);
            this.SendPacket(PacketType.LOADING_SCREEN, packet, DeliveryMethod.ReliableOrdered);
        }

        public void SendEquipmentUpdate()
        {
            var packet = new Packet();

            for (int i = 0; i < Enum.GetNames(typeof(EquipmentSlots)).Length; i++)
            {
                if (_player.Equipment.GetSlot(i) == null)
                {
                    packet.Write(false);
                    continue;
                }

                packet.Write(true);
                packet.Write(_player.Equipment.GetSlot(i).PackData());
            }

            this.SendPacket(PacketType.EQUIPMENT_UPDATE, packet, DeliveryMethod.ReliableOrdered);
        }
    }
}
