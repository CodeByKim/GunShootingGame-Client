using System;
using System.Collections.Generic;

public class CLIENTtoSERVER_KoreanStringPacketData : BasePacketData 
{
    public string testString1;
    public string testString2;

    public override void Serialize(TcpPacket packet)
    {
        packet.stream.WriteString(this.testString1);
        packet.stream.WriteString(this.testString2);
    }
}

public class SERVERtoCLIENT_KoreanStringPacketData : BasePacketData
{
    public string testString1;
    public string testString2;

    public override void Deserialize(TcpPacket packet)
    {
        this.testString1 = packet.stream.ReadString();
        this.testString2 = packet.stream.ReadString();
    }
}