using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CLIENTtoSERVER_LoginPacketData : BasePacketData
{
    public string id;
    public string password;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.id);
        packet.stream.WriteString(this.password);
    }
}

public class SERVERtoCLIENT_LoginPacketData : BasePacketData
{
    public string playerKey;
    public string playerID;

    public override void Deserialize(TcpPacket packet)
    {
        this.playerKey = packet.stream.ReadString();
        this.playerID = packet.stream.ReadString();
    }
}

public class CLIENTtoSERVER_RegisterPacketData : BasePacketData
{
    public string id;
    public string password;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.id);
        packet.stream.WriteString(this.password);
    }
}

public class SERVERtoCLIENT_RegisterPacketData : BasePacketData
{
    public override void Deserialize(TcpPacket packet)
    {

    }
}
