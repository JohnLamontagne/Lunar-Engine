using System;
using Lidgren.Network;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Server.Net;

namespace Lunar.Server.World.Actors
{
    public class PlayerNetworkComponent
    {
        private readonly Player _player;

        public PlayerNetworkComponent(Player player)
        {
            _player = player;
        }

        public void SendPlayerData()
        {
            var packet = new Packet(PacketType.PLAYER_DATA, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.Pack());
            _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendPlayerStats()
        {
            var packet = new Packet(PacketType.PLAYER_STATS, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.UniqueID);
            packet.Message.Write(_player.Descriptor.Speed);
            packet.Message.Write(_player.Descriptor.Level);
            packet.Message.Write(_player.Descriptor.Stats.Health);
            packet.Message.Write(_player.Descriptor.Stats.MaximumHealth);
            packet.Message.Write(_player.Descriptor.Stats.Strength + _player.Descriptor.StatBoosts.Strength);
            packet.Message.Write(_player.Descriptor.Stats.Intelligence + _player.Descriptor.StatBoosts.Intelligence);
            packet.Message.Write(_player.Descriptor.Stats.Dexterity + _player.Descriptor.StatBoosts.Dexterity);
            packet.Message.Write(_player.Descriptor.Stats.Defense + _player.Descriptor.StatBoosts.Defense);
            _player.Map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }



        public void SendInventoryUpdate()
        {
            var packet = new Packet(PacketType.INVENTORY_UPDATE, ChannelType.UNASSIGNED);

            for (int i = 0; i < Settings.MaxInventoryItems; i++)
            {
                if (_player.Inventory.GetSlot(i) != null)
                {
                    packet.Message.Write(true); // there is an item in this slot

                    packet.Message.Write(_player.Inventory.GetSlot(i).Item.PackData());
                    packet.Message.Write(_player.Inventory.GetSlot(i).Amount);
                }
                else
                {
                    packet.Message.Write(false);
                }
            }

            _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendMovementPacket()
        {
            var packet = new Packet(PacketType.PLAYER_MOVING, ChannelType.UNASSIGNED);
            packet.Message.Write(_player.UniqueID);
            packet.Message.Write((byte)_player.Direction);
            packet.Message.Write((byte)_player.State); // true if moving, false if not
            packet.Message.Write(_player.Descriptor.Position);

            _player.Map.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendLoadingScreen(bool active = true)
        {
            var packet = new Packet(PacketType.LOADING_SCREEN, ChannelType.UNASSIGNED);
            packet.Message.Write(active);
            _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }

        public void SendEquipmentUpdate()
        {
            var packet = new Packet(PacketType.EQUIPMENT_UPDATE, ChannelType.UNASSIGNED);

            for (int i = 0; i < Enum.GetNames(typeof(EquipmentSlots)).Length; i++)
            {
                if (_player.Equipment.GetSlot(i) == null)
                {
                    // There's nothing in this slot.
                    packet.Message.Write(false);
                    continue;
                }

                packet.Message.Write(true);
                packet.Message.Write(_player.Equipment.GetSlot(i).PackData());
            }

            _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
        }
    }
}
