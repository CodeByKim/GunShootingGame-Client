using System;
using System.Collections;
using System.Collections.Generic;

#region Create Room
public class CLIENTtoSERVER_CreateRoomPacketData : BasePacketData
{
    public string playerKey;
    public string roomName;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.roomName);
    }
}

public class SERVERtoCLIENT_CreateRoomPacketData : BasePacketData
{
    public string roomName;
    public string roomKey;
    //public int joinedRoomPlayerCount;       //사실 해당 필드는 필요없다. 방을 생성했으면 당연히 플레이어는 한명일테니

    public override void Deserialize(TcpPacket packet)
    {
        this.roomName = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();
        //this.joinedRoomPlayerCount = packet.stream.ReadInt();
    }
}
#endregion

#region Create Room Other Player
public class SERVERtoCLIENT_CreateRoomOtherPlayerPacketData : BasePacketData
{
    public string roomName;
    public string roomKey;
    //public int joinedRoomPlayerCount;   //사실 해당 필드는 필요없다. 방을 생성했으면 당연히 플레이어는 한명일테니
    public string playerName;       //로비에서 해당 플레이어를 제거하기 위해 필요

    public override void Deserialize(TcpPacket packet)
    {
        this.roomName = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();
        //this.joinedRoomPlayerCount = packet.stream.ReadInt();
        this.playerName = packet.stream.ReadString();
    }
}
#endregion

#region Leave Room
public class CLIENTtoSERVER_LeaveRoomPacketData : BasePacketData
{
    public string playerKey;
    public string roomKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.roomKey);
    }
}

public class SERVERtoCLIENT_LeaveRoomPacketData : BasePacketData
{
    public int roomCount;
    public string[] roomKeys;
    public string[] roomNames;
    public bool[] isStart;
    public int[] joinedRoomPlayerCount;

    public int playerCount;
    public string[] playerNames;

    public override void Deserialize(TcpPacket packet)
    {
        this.roomCount = packet.stream.ReadInt();

        this.roomKeys = new string[this.roomCount];
        this.roomNames = new string[this.roomCount];
        this.isStart = new bool[this.roomCount];
        this.joinedRoomPlayerCount = new int[this.roomCount];

        for (int i = 0; i < this.roomCount; i++)
            this.roomKeys[i] = packet.stream.ReadString();

        for (int i = 0; i < this.roomCount; i++)
            this.roomNames[i] = packet.stream.ReadString();

        for (int i = 0; i < this.roomCount; i++)
            this.joinedRoomPlayerCount[i] = packet.stream.ReadInt();

        for (int i = 0; i < this.roomCount; i++)
            this.isStart[i] = packet.stream.ReadBool();

        this.playerCount = packet.stream.ReadInt();
        this.playerNames = new string[this.playerCount];

        for (int i = 0; i < this.playerCount; i++)
            this.playerNames[i] = packet.stream.ReadString();
    }
}
#endregion

#region Leave Room Other Player
public class SERVERtoCLIENT_LeaveRoomOtherPlayerPacketData : BasePacketData
{
    public string playerID;
    public string roomName;
    public string roomKey;
    public int joinedRoomPlayerCount;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();
        this.roomName = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();
        this.joinedRoomPlayerCount = packet.stream.ReadInt();
    }
}
#endregion

#region Join Room
public class CLIENTtoSERVER_JoinRoomPacketData :  BasePacketData
{
    public string playerKey;
    public string roomKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.roomKey);
    }
}

public class SERVERtoCLIENT_JoinRoomPacketData : BasePacketData
{
    public string roomKey;
    public string roomName;             
	public int joinedPlayerCount;       
    public string[] joinedPlayers;
    public bool[] joinedPlayerReadyState;
    public bool[] isLeaders;

    public override void Deserialize(TcpPacket packet)
    {
        this.roomKey = packet.stream.ReadString();
        this.roomName = packet.stream.ReadString();

        this.joinedPlayerCount = packet.stream.ReadInt();

        this.joinedPlayers = new string[this.joinedPlayerCount];
        this.joinedPlayerReadyState = new bool[this.joinedPlayerCount];
        this.isLeaders = new bool[this.joinedPlayerCount];

        for (int i = 0; i < this.joinedPlayerCount; i++)
            this.joinedPlayers[i] = packet.stream.ReadString();

        for (int i = 0; i < this.joinedPlayerCount; i++)
            this.joinedPlayerReadyState[i] = packet.stream.ReadBool();

        for (int i = 0; i < this.joinedPlayerCount; i++)
            this.isLeaders[i] = packet.stream.ReadBool();       
    }
}
#endregion

#region Join Room Other Player
public class SERVERtoCLIENT_JoinRoomOtherPlayerPacketData : BasePacketData      //2가지 경우가 있음 1. 내가 로비에 있을때 누군가 방에 입장할때, 2. 내가 방에 있을때 누군가 방으로 들어올때
{
    public string playerID;
    public string roomKey;
    //public int joinedRoomPlayerCount;       //사실 이 필드는 필요가 없다. 누가 방에 입장하면 당연히 사람이 한명 더 증가하는것일뿐.

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();
        //this.joinedRoomPlayerCount = packet.stream.ReadInt();
    }
}
#endregion

#region Change Room Leader
public class SERVERtoCLIENT_ChangeRoomLeaderPacketData : BasePacketData
{
	public string playerKey;
    public string playerID;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerKey = packet.stream.ReadString();
        this.playerID = packet.stream.ReadString();
    }
}
#endregion

#region Remove Room
public class SERVERtoCLIENT_RemoveRoomPacketData : BasePacketData
{
	public string roomKey;

    public override void Deserialize(TcpPacket packet)
    {
        this.roomKey = packet.stream.ReadString();
    }
}
#endregion

#region Room Chatting
public class CLIENTtoSERVER_RoomChattingPacketData : BasePacketData
{
    public string playerKey;
    public string roomKey;
    public string chatString;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.roomKey);
        packet.stream.WriteString(this.chatString);
    }
}

public class SERVERtoCLIENT_RoomChattingPacketData : BasePacketData
{
    public string playerName;
    public string chatString;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerName = packet.stream.ReadString();
        this.chatString = packet.stream.ReadString();
    }
}
#endregion

#region Request Ready
public class CLIENTtoSERVER_ReadyPacketData : BasePacketData
{
	public string playerKey;
    public string roomKey;
    //public bool ready;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.roomKey);
        //packet.stream.WriteBool(this.ready);
    }
}

public class SERVERtoCLIENT_ReadyPacketData : BasePacketData
{
    public bool ready;

	public override void Deserialize(TcpPacket packet)
    {
        this.ready = packet.stream.ReadBool();
    }
}
#endregion

#region Ready Other Player
public class SERVERtoCLIENT_ReadyOtherPlayerPacketData : BasePacketData
{
	public string playerID;
    public bool ready;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();
        this.ready = packet.stream.ReadBool();
    }
}
#endregion

#region Request Room Start
public class CLIENTtoSERVER_RoomStartPacketData : BasePacketData
{
    public string playerKey;
    public string roomKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.roomKey);
    }
}

public class SERVERtoCLIENT_RoomStartPacketData : BasePacketData
{
    public string gameKey;

    public override void Deserialize(TcpPacket packet)
    {
        this.gameKey = packet.stream.ReadString();
    }
}
#endregion