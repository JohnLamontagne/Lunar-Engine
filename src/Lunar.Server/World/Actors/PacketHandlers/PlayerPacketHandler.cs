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
using System.Linq;
using Lidgren.Network;
using Lunar.Core;
using Lunar.Core.Net;
using Lunar.Core.World;
using Lunar.Core.World.Actor;
using Lunar.Server.Net;
using Lunar.Server.Utilities;
using Lunar.Server.World.Actors.Actions.Player;
using Lunar.Server.World.Structure;

namespace Lunar.Server.World.Actors.PacketHandlers
{
    public class PlayerPacketHandler
    {
        private readonly Player _player;

        public PlayerPacketHandler(Player player)
        {
            _player = player;

            player.NetworkComponent.Connection.AddPacketHandler(PacketType.PLAYER_MOVING, this.Handle_PlayerMoving);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.DROP_ITEM, this.Handle_DropItem);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.MAP_LOADED, this.Handle_MapLoaded);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.REQ_USE_ITEM, this.Handle_UseItem);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.REQ_UNEQUIP_ITEM, this.Handle_UnequipItem);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.REQ_TARGET, this.Handle_ReqTarget);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.DESELECT_TARGET, this.Handle_DeselectTarget);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.PICKUP_ITEM, this.Handle_PickupItem);
            player.NetworkComponent.Connection.AddPacketHandler(PacketType.PLAYER_INTERACT, this.Handle_PlayerInteract);
        }

        private void Handle_DeselectTarget(PacketReceivedEventArgs obj)
        {
            _player.Target = null;
        }

        private void Handle_PlayerInteract(PacketReceivedEventArgs args)
        {
            _player.ActionProcessor.Process(new PlayerInteractAction());
        }

        private void Handle_ReqTarget(PacketReceivedEventArgs args)
        {
            var targetUniqueID = args.Message.ReadString();

            // Make sure we don't target ourselves
            if (targetUniqueID == _player.UniqueID)
                return;

            var target = _player.Map.GetActor(targetUniqueID);

            if (target != null)
            {
                _player.Target = target;

                var packet = new Packet(PacketType.TARGET_ACQ, ChannelType.UNASSIGNED);
                packet.Message.Write(target.UniqueID);
                _player.NetworkComponent.SendPacket(packet, NetDeliveryMethod.ReliableOrdered);
            }
            else
            {
                _player.NetworkComponent.SendChatMessage("Invalid target!", ChatMessageType.Alert);
            }
        }

        private void Handle_MapLoaded(PacketReceivedEventArgs args)
        {
            _player.MapLoaded = true;
        }

        private void Handle_DropItem(PacketReceivedEventArgs args)
        {
            int slotNum = args.Message.ReadInt32();

            _player.ActionProcessor.Process(new PlayerDropItemAction(slotNum));
        }

        private void Handle_UseItem(PacketReceivedEventArgs args)
        {
            int slotNum = args.Message.ReadInt32();

            _player.ActionProcessor.Process(new PlayerUseItemAction(slotNum));
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
                // Can the player actually move based on the minimum update time from the tick rate
                if (_player.CanMove(_player.Direction, _player.Descriptor.Speed * (1000f / Settings.TickRate)))
                {
                    _player.NetworkComponent.SendMovementPacket();
                }
                else
                {
                    _player.State = ActorStates.Idle;

                    _player.NetworkComponent.SendMovementPacket();
                }
            }
            else // The player wishes to cease moving.
            {
                _player.State = ActorStates.Idle;
                _player.NetworkComponent.SendMovementPacket();
            }
        }

        private void Handle_PickupItem(PacketReceivedEventArgs obj)
        {
            _player.ActionProcessor.Process(new PlayerPickupItemAction());
        }

        private void Handle_UnequipItem(PacketReceivedEventArgs args)
        {
            int slotNum = args.Message.ReadInt32();

            _player.ActionProcessor.Process(new PlayerUnequipItemAction(slotNum));
        }
    }
}