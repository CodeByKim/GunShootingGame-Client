using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum PROTOCOL
{
    KOREAN_STRING_TEST = 0,

    REGISTER_USER,
    LOGIN,

    ENTER_LOBBY,
    LEAVE_LOBBY,
    ENTER_LOBBY_OTHER_PLAYER,
    LEAVE_LOBBY_OTHER_PLAYER,

    CREATE_ROOM,
    LEAVE_ROOM,
    CREATE_ROOM_OTHER_PLAYER,
    LEAVE_ROOM_OTHER_PLAYER,

    REMOVE_ROOM,

    JOIN_ROOM,
    JOIN_ROOM_OTHER_PLAYER,

    READY,
    READY_OTHER_PLAYER,

    CHANGE_ROOM_LEADER,
    CHANGE_ROOM_LEADER_OTHER_PLAYER,

    ROOM_START,

    LOBBY_CHATTING,
    ROOM_CHATTING,

    GAME_START,
    GAME_END,

    OTHER_GAME_START,
    OTHER_GAME_END,

    GAME_PLAYER_MOVE,
    OTHER_GAME_PLAYER_MOVE,

    GAME_PLAYER_ATTACK,
    OTHER_GAME_PLAYER_ATTACK,

    ENEMY_SPAWN,
    ENEMY_MOVE,
    ENEMY_DAMAGED,
    ENEMY_ATTACK,
    ENEMY_DIE,

    GAME_PLAYER_DIE,
    GAME_PLAYER_DAMAGED,

    POTION_SPAWN,
    POTION_PICKUP,

    HEART_BEAT
}

public enum EXTRA
{
    REQUEST = 0,
    RESPONSE,
    SUCCESS,
    FAIL,

    NONE,
    FOR_LOGOUT,
    FOR_LOGIN,
    FOR_JOIN_ROOM,
    FOR_LEAVE_ROOM,
    OVERLAPED_LOGIN,

    CLIENT_TO_SERVER,
    HOST_TO_SERVER
};

public enum PLAYER_STATE
{
    NONE,
    LOGIN,
	LOBBY,
	IN_ROOM,
	IN_GAME
};

public enum COMMAND_TYPE
{
    REQUEST,
    RESPONSE
}

public enum PLAYER_TYPE
{
    MINE,
    OTHER
}

public static class Defines
{
    public static int MAX_BUFFER_LENGTH = 4096;
    public static int MAX_PACKET_DATA_LENGTH = 4096;
    public static int PACKET_HEADER_LENGTH = 12;
    public static int SERVER_PORT = 33333;
}