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

using System;
using System.Linq;
using Lidgren.Network;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Core.World.Actor;
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.World.Structure;

namespace Lunar.Server.World.Actors.PacketHandlers
{
    public class PlayerPacketHandler
    {
        private readonly Player _player;

        public PlayerPacketHandler(Player player)
        {
            _player = player;

            player.Connection.AddPacketHandler(PacketType.PLAYER_MOVING, this.Handle_PlayerMoving);
            player.Connection.AddPacketHandler(PacketType.DROP_ITEM, this.Handle_DropItem);
            player.Connection.AddPacketHandler(PacketType.MAP_LOADED, this.Handle_MapLoaded);
            player.Connection.AddPacketHandler(PacketType.REQ_USE_ITEM, this.Handle_UseItem);
            player.Connection.AddPacketHandler(PacketType.REQ_UNEQUIP_ITEM, this.Handle_UnequipItem);
            player.Connection.AddPacketHandler(PacketType.REQ_TARGET, this.Handle_ReqTarget);
            player.Connection.AddPacketHandler(PacketType.DESELECT_TARGET, this.Handle_DeselectTarget);
            player.Connection.AddPacketHandler(PacketType.PICKUP_ITEM, this.Handle_PickupItem);
            player.Connection.AddPacketHandler(PacketType.PLAYER_INTERACT, this.Handle_PlayerInteract);
        }

        private void Handle_DeselectTarget(PacketReceivedEventArgs obj)
        {
            _player.Target = null;
        }

        private void Handle_PlayerInteract(PacketReceivedEventArgs args)
        {
            foreach (var mapObject in _player.Layer.GetCollidingMapObjects(_player.Position, _player.CollisionBounds))
            {
                mapObject.OnInteract(_player);
            }

            // Try to attack the target
            if (_player.Target != null)
            {
                if (_player.Target.Attackable)
                {
                    _player.Target.OnAttacked(_player, (int)(_player.Strength * new Random().NextDouble()));
                }
            }
        }

        private void Handle_ReqTarget(PacketReceivedEventArgs args)
        {
            var targetUniqueID = args.Message.ReadInt64();

            // Make sure we don't target ourselves
            if (targetUniqueID == _player.UniqueID)
                return;

            IActor target = _player.Map.GetActor(targetUniqueID);

            if (target != null)
            {
                _player.Target = target;

                var packet = new Packet(PacketType.TARGET_ACQ, ChannelType.UNASSIGNED);
                packet.Message.Write(target.UniqueID);
                _player.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
            }
            else
            {
                _player.SendChatMessage("Invalid target!", ChatMessageType.Alert);
            }
        }

        private void Handle_MapLoaded(PacketReceivedEventArgs args)
        {
            _player.MapLoaded = true;
        }

        private void Handle_DropItem(PacketReceivedEventArgs args)
        {
            int slotNum = args.Message.ReadInt32();

            if (_player.Inventory.GetSlot(slotNum) != null)
            {
                _player.Map.SpawnItem(_player.Inventory.GetSlot(slotNum).Item, _player.Position, _player.Layer);
                _player.Inventory.RemoveItem(slotNum, 1);

            }
        }

        private void Handle_UseItem(PacketReceivedEventArgs args)
        {
            int slotNum = args.Message.ReadInt32();

            // Sanity check: is there actually an item in this slot?
            if (_player.Inventory.GetSlot(slotNum) == null)
            {
                // Log it!
                Logger.LogEvent($"Player attempted to equip bad item! User: {_player.Name} SlotNum: {slotNum}.", LogTypes.GAME, Environment.StackTrace);

                return;
            }

            Item item = _player.Inventory.GetSlot(slotNum).Item;

            if (item.ItemType == ItemTypes.Equipment)
            {
                _player.Equipment.Equip(item);
                item.OnEquip(_player);
                _player.Inventory.RemoveItem(slotNum, 1);
            }
            else if (item.ItemType == ItemTypes.Usable)
            {
                item.OnUse(_player);
            }
        }

        private void Handle_PlayerMoving(PacketReceivedEventArgs args)
        {
            if (!_player.MapLoaded)
                return;

            _player.Direction = (Direction)args.Message.ReadByte();
            var wantsToMove = args.Message.ReadBoolean();

            if (wantsToMove)
            {
                _player.State = ActorStates.Moving;
            }
            else
            {
                _player.State = ActorStates.Idle;
            }

            if (wantsToMove)
            {
                // Can the player actually move based on the minimum update time from the tick rate
                if (_player.CanMove(_player.Speed * (1000f / Settings.TileSize)))
                {
                    _player.SendMovementPacket();
                }
                else
                {
                    _player.State = ActorStates.Idle;

                    _player.SendMovementPacket();
                }
            }
            else // The player wishes to cease moving.
            {
                _player.SendMovementPacket();
            }
        }


        private void Handle_PickupItem(PacketReceivedEventArgs obj)
        {
            MapItem mapItem = _player.Map.GetMapItems().FirstOrDefault(mItem => mItem.WithinReachOf(_player));

            if (mapItem != null)
            {
                _player.Inventory.Add(mapItem.Item, mapItem.Amount);
                _player.Map.RemoveItem(mapItem.Item);
            }
        }

        private void Handle_UnequipItem(PacketReceivedEventArgs args)
        {
            int slotNum = args.Message.ReadInt32();

            // Sanity check: is there actually an item in this slot?
            if (_player.Equipment.GetSlot(slotNum) == null)
            {
                // Log it!
                Logger.LogEvent($"Player attempted to unequip bad item! User: {_player.Name} SlotNum: {slotNum}.", LogTypes.GAME, Environment.StackTrace);

                return;
            }

            var item = _player.Equipment.GetSlot(slotNum);

            if (item.ItemType != ItemTypes.Equipment || item.SlotType == EquipmentSlots.NE)
            {
                // Log it!
                Logger.LogEvent($"Player attempted to unequip unequippable item! User: {_player.Name} SlotNum: {slotNum}.", LogTypes.GAME, Environment.StackTrace);

                return;
            }

            _player.Equipment.SetSlot(slotNum, null);
            _player.Inventory.Add(item, 1);
            _player.SendEquipmentUpdate();
            _player.CalculateBoostedStats();
        }


    }
}
