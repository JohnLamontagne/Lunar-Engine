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