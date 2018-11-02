using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#region Heart Beat
public class CLIENTtoSERVER_HeartBeatPacketData : BasePacketData        //게임씬의 로딩이 다 끝나면 서버로 전송
{
    public override void Serialize(TcpPacket packet)
    {
        
    }
}

public class SERVERtoCLIENT_HeartBeatPacketData : BasePacketData        //모든 플레이어의 로딩이 끝나면 게임 시작
{
    public override void Deserialize(TcpPacket packet)
    {

    }
}
#endregion

#region Game Start
public class CLIENTtoSERVER_GameStartPacketData : BasePacketData        //게임씬의 로딩이 다 끝나면 서버로 전송
{
    public string playerKey;
    public string gameKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
    }
}

public class SERVERtoCLIENT_GameStartPacketData : BasePacketData        //모든 플레이어의 로딩이 끝나면 게임 시작
{
    public override void Deserialize(TcpPacket packet)
    {
        
    }
}
#endregion

#region Game End
public class CLIENTtoSERVER_GameEndPacketData : BasePacketData        
{
    public string playerKey;
    public string gameKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
    }
}

public class SERVERtoCLIENT_GameEndPacketData : BasePacketData       
{
    public string roomName;
    public string roomKey;
    public int joinedPlayerCount;
    public string[] joinedPlayers;
    public bool[] joinedPlayerReadyState;
    public bool[] isLeaders;

    public override void Deserialize(TcpPacket packet)
    {
        this.roomName = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();

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

#region Game Player Move
public class CLIENTtoSERVER_GamePlayerMovePacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;

    public float posX;
    public float posY;
    public float posZ;

    public float angularX;
    public float angularY;
    public float angularZ;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);

        packet.stream.WriteFloat(this.posX);
        packet.stream.WriteFloat(this.posY);
        packet.stream.WriteFloat(this.posZ);

        packet.stream.WriteFloat(this.angularX);
        packet.stream.WriteFloat(this.angularY);
        packet.stream.WriteFloat(this.angularZ);
    }
}
#endregion

#region Other Game Player Move
public class SERVERtoCLIENT_OtherGamePlayerMovePacketData : BasePacketData
{
    public string playerID;

    public float posX;
    public float posY;
    public float posZ;

    public float angularX;
    public float angularY;
    public float angularZ;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();

        this.posX = packet.stream.ReadFloat();
        this.posY = packet.stream.ReadFloat();
        this.posZ = packet.stream.ReadFloat();

        this.angularX = packet.stream.ReadFloat();
        this.angularY = packet.stream.ReadFloat();
        this.angularZ = packet.stream.ReadFloat();
    }
}

public class SERVERtoHOST_GamePlayerMovePacketData : BasePacketData
{
    public string playerKey;

    public float posX;
    public float posY;
    public float posZ;

    public float angularX;
    public float angularY;
    public float angularZ;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerKey = packet.stream.ReadString();

        this.posX = packet.stream.ReadFloat();
        this.posY = packet.stream.ReadFloat();
        this.posZ = packet.stream.ReadFloat();

        this.angularX = packet.stream.ReadFloat();
        this.angularY = packet.stream.ReadFloat();
        this.angularZ = packet.stream.ReadFloat();
    }
}
#endregion

#region Game Player Attack
public class CLIENTtoSERVER_GamePlayerAttackPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
    }
}

public class HOSTtoSERVER_GamePlayerAttackPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);            
    }
}
#endregion

#region Other Game Player Attack
public class SERVERtoCLIENT_OtherGamePlayerAttackPacketData : BasePacketData
{
    public string playerID;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();            
    }
}

public class SERVERtoHOST_GamePlayerAttackPacketData : BasePacketData
{
    public string playerKey;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerKey = packet.stream.ReadString();           
    }
}
#endregion

#region Game Player Damaged
public class CLIENTtoSERVER_GamePlayerDamagedPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string playerID;
    public int hp;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.playerID);
        packet.stream.WriteInt(this.hp);
    }
}

public class SERVERtoCLIENT_GamePlayerDamagedPacketData : BasePacketData
{
    public string playerID;
    public int hp;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();
        this.hp = packet.stream.ReadInt();
    }
}
#endregion

#region Game Player Die
public class CLIENTtoSERVER_GamePlayerDiePacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string playerID;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.playerID);
    }
}

public class SERVERtoCLIENT_GamePlayerDiePacketData : BasePacketData
{
    public string playerID;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();
    }
}
#endregion

#region Enemy Spawn
public class CLIENTtoSERVER_EnemySpawnPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string enemyID;

    public float posX;
    public float posY;
    public float posZ;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.enemyID);

        packet.stream.WriteFloat(this.posX);
        packet.stream.WriteFloat(this.posY);
        packet.stream.WriteFloat(this.posZ);
    }
}

public class SERVERtoCLIENT_EnemySpawnPacketData : BasePacketData
{
    public string enemyID;

    public float posX;
    public float posY;
    public float posZ;

    public override void Deserialize(TcpPacket packet)
    {
        this.enemyID = packet.stream.ReadString();

        this.posX = packet.stream.ReadFloat();
        this.posY = packet.stream.ReadFloat();
        this.posZ = packet.stream.ReadFloat();
    }
}
#endregion

#region Enemy Move
public class CLIENTtoSERVER_EnemyMovePacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string enemyID;

    public float posX;
    public float posY;
    public float posZ;

    public float angularX;
    public float angularY;
    public float angularZ;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.enemyID);

        packet.stream.WriteFloat(this.posX);
        packet.stream.WriteFloat(this.posY);
        packet.stream.WriteFloat(this.posZ);

        packet.stream.WriteFloat(this.angularX);
        packet.stream.WriteFloat(this.angularY);
        packet.stream.WriteFloat(this.angularZ);
    }
}

public class SERVERtoCLIENT_EnemyMovePacketData : BasePacketData
{
    public string enemyID;

    public float posX;
    public float posY;
    public float posZ;

    public float angularX;
    public float angularY;
    public float angularZ;

    public override void Deserialize(TcpPacket packet)
    {
        this.enemyID = packet.stream.ReadString();

        this.posX = packet.stream.ReadFloat();
        this.posY = packet.stream.ReadFloat();
        this.posZ = packet.stream.ReadFloat();

        this.angularX = packet.stream.ReadFloat();
        this.angularY = packet.stream.ReadFloat();
        this.angularZ = packet.stream.ReadFloat();
    }
}
#endregion

#region Enemy Damaged
public class CLIENTtoSERVER_EnemyDamagedPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string enemyID;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.enemyID);
    }
}

public class SERVERtoCLIENT_EnemyDamagedPacketData : BasePacketData
{
    public string enemyID;

    public override void Deserialize(TcpPacket packet)
    {
        this.enemyID = packet.stream.ReadString();
    }
}
#endregion

#region Enemy Attack
public class CLIENTtoSERVER_EnemyAttackPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string enemyID;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.enemyID);    
    }
}

public class SERVERtoCLIENT_EnemyAttackPacketData : BasePacketData
{
    public string enemyID;

    public override void Deserialize(TcpPacket packet)
    {
        this.enemyID = packet.stream.ReadString();
    }
}
#endregion

#region Enemy Die
public class CLIENTtoSERVER_EnemyDiePacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string enemyID;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.enemyID);
    }
}

public class SERVERtoCLIENT_EnemyDiePacketData : BasePacketData
{
    public string enemyID;

    public override void Deserialize(TcpPacket packet)
    {
        this.enemyID = packet.stream.ReadString();
    }
}
#endregion

#region Potion Spawn
public class CLIENTtoSERVER_PotionSpawnPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string potionID;

    public float positionX;
    public float positionY;
    public float positionZ;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.potionID);

        packet.stream.WriteFloat(this.positionX);
        packet.stream.WriteFloat(this.positionY);
        packet.stream.WriteFloat(this.positionZ);
    }
}

public class SERVERtoCLIENT_PotionSpawnPacketData : BasePacketData
{
    public string potionID;

    public float positionX;
    public float positionY;
    public float positionZ;

    public override void Deserialize(TcpPacket packet)
    {
        this.potionID = packet.stream.ReadString();

        this.positionX = packet.stream.ReadFloat();
        this.positionY = packet.stream.ReadFloat();
        this.positionZ = packet.stream.ReadFloat();
    }

}
#endregion

#region Potion Pickup
public class CLIENTtoSERVER_PotionPickupPacketData : BasePacketData
{
    public string playerKey;
    public string gameKey;
    public string playerID;
    public string potionID;
    public int hp;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.playerKey);
        packet.stream.WriteString(this.gameKey);
        packet.stream.WriteString(this.playerID);
        packet.stream.WriteString(this.potionID);
        packet.stream.WriteInt(this.hp);
    }
}

public class SERVERtoCLIENT_PotionPickupPacketData : BasePacketData
{
    public string playerID;
    public string potionID;
    public int hp;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerID = packet.stream.ReadString();
        this.potionID = packet.stream.ReadString();
        this.hp = packet.stream.ReadInt();
    }
}
#endregion

#region Other Game Start
public class SERVERtoCLIENT_OtherGameStartPacketData : BasePacketData        //모든 플레이어의 로딩이 끝나면 게임 시작
{
    public string roomName;
    public string roomKey;

    public override void Deserialize(TcpPacket packet)
    {
        this.roomName = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();
    }
}
#endregion

#region Other Game End
public class SERVERtoCLIENT_OtherGameEndPacketData : BasePacketData        //모든 플레이어의 로딩이 끝나면 게임 시작
{
    public string roomName;
    public string roomKey;

    public override void Deserialize(TcpPacket packet)
    {
        this.roomName = packet.stream.ReadString();
        this.roomKey = packet.stream.ReadString();
    }
}
#endregion