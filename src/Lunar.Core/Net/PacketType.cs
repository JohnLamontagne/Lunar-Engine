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
namespace Lunar.Core.Net
{
    public enum PacketType
    {
        LOGIN,

        LOGIN_SUCCESS,

        LOGIN_FAIL,

        REGISTER,

        REGISTER_SUCCESS,

        REGISTRATION_FAIL,

        ALERT_MSG,

        PLAYER_MSG,

        MAP_DATA,

        MAP_ITEM_SPAWN,

        MAP_ITEM_DESPAWN,

        PLAYER_DATA,

        PLAYER_STATS,

        PLAYER_JOINED,

        PLAYER_MOVING,

        PLAYER_LEFT,

        QUIT_GAME,

        PLAY_MUSIC,

        STOP_MUSIC,

        PLAY_SOUND,

        INVENTORY_UPDATE,

        DROP_ITEM,

        PICKUP_ITEM,

        NPC_DATA,

        NPC_MOVING,

        MAP_LOADED,

        REQ_USE_ITEM,

        EQUIPMENT_UPDATE,

        REQ_UNEQUIP_ITEM,

        REQ_TARGET,

        DESELECT_TARGET,

        TARGET_ACQ,

        DIALOGUE,

        DIALOGUE_END,

        DIALOGUE_RESP,

        POSITION_UPDATE,

        AVAILABLE_COMMANDS,

        CLIENT_COMMAND,

        LOADING_SCREEN,

        PLAYER_INTERACT,

        // Represents the total number of packets.
        PACKET_COUNT
    }
}