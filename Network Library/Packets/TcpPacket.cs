using System;
using System.Collections;
using System.Collections.Generic;

public class TcpPacketHeader
{
    public int length;
    public int protocol;
    public int extra;
}

public class TcpPacket
{
    public TcpPacketHeader header;
    public MemoryStream stream;

    public void Initialize()
    {
        byte[] buffer = new byte[Defines.MAX_PACKET_DATA_LENGTH];
        this.stream = new MemoryStream(buffer, Defines.MAX_PACKET_DATA_LENGTH);
    }

    public void Initialize(byte[] buffer, int length)
    {
        if (length != 0)
            this.stream = new MemoryStream(buffer, length);
        else
            this.stream = null;
    }

    public void SetHeader(int protocol, int extra)
    {
        TcpPacketHeader header = new TcpPacketHeader();
        header.length = Defines.PACKET_HEADER_LENGTH + this.stream.Offset;
        header.protocol = protocol;
        header.extra = extra;
        this.header = header;
    }

    public void SetHeader(TcpPacketHeader header)
    {
        this.header = header;
    }
}

public class BasePacketData
{
    public virtual void Serialize(TcpPacket packet) { }
    public virtual void Deserialize(TcpPacket packet) { }
}

