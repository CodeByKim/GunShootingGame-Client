using System;

public class PacketFactory
{
    private static PacketFactory instance;

    public static PacketFactory Instance
    {
        get
        {
            if (instance == null)
                instance = new PacketFactory();

            return instance;
        }
    }

    public void Initialize()
    {
        
    }

    public void Release()
    {
        
    }

    public bool IsMakePacket(StreamBuffer streamBuffer)
    {
        if (streamBuffer.RemainLength >= Defines.PACKET_HEADER_LENGTH)
        {
            TcpPacketHeader header = DeserializeHeader(streamBuffer);

            if (header.length > Defines.MAX_BUFFER_LENGTH || header.length <= 0)
            {
                // 패킷의 길이가 이상하면 잘못된 패킷
                streamBuffer.RemainLength = 0;
                return false;
            }

            if (streamBuffer.RemainLength >= header.length)
                return true;                
        }

        return false;
    }

    public TcpPacketHeader DeserializeHeader(StreamBuffer streamBuffer)
    {
        TcpPacketHeader header = new TcpPacketHeader();
        header.length = BitConverter.ToInt32(streamBuffer.Buffer, 0);
        header.protocol = BitConverter.ToInt32(streamBuffer.Buffer, 4);
        header.extra = BitConverter.ToInt32(streamBuffer.Buffer, 8);

        return header;
    }

    public TcpPacket DeserializePacket(StreamBuffer streamBuffer)
    {        
        TcpPacketHeader packetHeader = DeserializeHeader(streamBuffer);
        int packetDataLength = packetHeader.length - Defines.PACKET_HEADER_LENGTH;

        TcpPacket packet = new TcpPacket();

        byte[] dataBuffer = new byte[packetDataLength];
        Array.Copy(streamBuffer.Buffer, Defines.PACKET_HEADER_LENGTH, dataBuffer, 0, packetDataLength);

        packet.Initialize(dataBuffer, packetDataLength);
        packet.SetHeader(packetHeader);

        if (streamBuffer.RemainLength - packetHeader.length > 0)
        {
            Array.Copy(streamBuffer.Buffer, packetHeader.length, 
                       streamBuffer.Buffer, 0, streamBuffer.RemainLength - packetHeader.length);
        }

        int remainLength = streamBuffer.RemainLength;
        remainLength -= packetHeader.length;
        streamBuffer.RemainLength = remainLength;

        if (streamBuffer.RemainLength <= 0)
        {
            remainLength = 0;
            Array.Clear(streamBuffer.Buffer, 0, streamBuffer.Buffer.Length);
        }

        return packet;
    }

    public byte[] SerializeHeader(TcpPacket packet)
    {        
        byte[] data = new byte[Defines.PACKET_HEADER_LENGTH];

        TcpPacketHeader header = packet.header;
        Array.Copy(BitConverter.GetBytes(header.length), 0, data, 0, 4);
        Array.Copy(BitConverter.GetBytes(header.protocol), 0, data, 4, 4);
        Array.Copy(BitConverter.GetBytes(header.extra), 0, data, 8, 4);

        return data;
    }

    public byte[] SerializePacket(TcpPacket packet)
    {
        int length = packet.header.length;
        int packetDataLength = length - Defines.PACKET_HEADER_LENGTH;

        byte[] serializedHeader = SerializeHeader(packet);
        byte[] serializedPacket = new byte[packet.header.length];
        Array.Copy(serializedHeader, 0, serializedPacket, 0, Defines.PACKET_HEADER_LENGTH);

        if(packetDataLength != 0)
            Array.Copy(packet.stream.Buffer, 0, serializedPacket, Defines.PACKET_HEADER_LENGTH, packetDataLength);

        return serializedPacket;
    }
}