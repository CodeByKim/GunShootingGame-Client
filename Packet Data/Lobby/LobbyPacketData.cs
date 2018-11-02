using System;
using System.Collections;
using System.Collections.Generic;

#region Enter Lobby
public class CLIENTtoSERVER_EnterLobbyPacketData : BasePacketData
{
    public string playerKey;

    public override void Serialize(TcpPacket packet)
    {        
        packet.stream.WriteString(this.playerKey);
    }
}

public class SERVERtoCLIENT_EnterLobbyPacketData : BasePacketData
{
    public int roomCount;
    public string[] roomKeys;
    public string[] roomNames;
    public int[] joinedRoomPlayerCount;
    public bool[] isStart;
    public int playerCount;
    public string[] playerIds;

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
        this.playerIds = new string[this.playerCount];

        for (int i = 0; i < this.playerCount; i++)
            this.playerIds[i] = packet.stream.ReadString();
    }
}
#endregion

#region Leave Lobby
public class CLIENTtoSERVER_LeaveLobbyPacketData : BasePacketData
{
    public string playerKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
    }
}

public class SERVERtoCLIENT_LeaveLobbyPacketData : BasePacketData
{
    //public string test;

    public override void Deserialize(TcpPacket packet)
    {
        //this.test = packet.stream.ReadString();    
    }
}
#endregion

#region Enter Lobby Other Player
public class SERVERtoCLIENT_EnterLobbyOtherPlayerPacketData : BasePacketData
{
    public string playerName;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerName = packet.stream.ReadString();
    }
}

public class SERVERtoCLIENT_LeaveLobbyOtherPlayerPacketData : BasePacketData
{   
    public string playerName;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerName = packet.stream.ReadString();
    }
}
#endregion

#region Lobby Chatting
public class CLIENTtoSERVER_LobbyChattingPacketData : BasePacketData
{
    public string playerKey;
    public string chatString;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.chatString);
    }
}

public class SERVERtoCLIENT_LobbyChattingPacketData : BasePacketData
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