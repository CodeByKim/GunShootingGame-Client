using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TcpSession : NetworkSession
{
    private bool isConnected;
    private StreamBuffer streamBuffer;

    public bool IsConnected
    {
        get { return this.isConnected; }
        set { this.isConnected = value; }
    }

    public TcpSession()
    {
        this.isConnected = false;
        this.streamBuffer = new StreamBuffer();
        this.socket = new TcpSocket();
    }

    public override void SendPacket(TcpPacket packet)
    {        
        byte[] packetData = PacketFactory.Instance.SerializePacket(packet);
        this.socket.Send(packetData, packet.header.length);
    }

    public override void OnReceive(int readDataLength)
    {
        this.streamBuffer.PutStream(this.socket.ReadDataBuffer, readDataLength);

        while(PacketFactory.Instance.IsMakePacket(this.streamBuffer))
        {
            
            TcpPacket packet = PacketFactory.Instance.DeserializePacket(this.streamBuffer);

            Package package = new Package();
            package.packet = packet;
            package.session = this;

            Network.Instance.PutPackage(package);
        }

        this.socket.Receive();
    }

    public void SetConnected()
    {
        this.isConnected = true;
        this.socket.Receive();
    }
}
