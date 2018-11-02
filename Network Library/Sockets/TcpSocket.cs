using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class TcpSocket : RawSocket
{
    private SocketAsyncEventArgs connectArgs;

    public TcpSocket()
    {
        this.Socket = new Socket(AddressFamily.InterNetwork, 
            SocketType.Stream, ProtocolType.Tcp);
        this.connectArgs = new SocketAsyncEventArgs();
        this.connectArgs.Completed += OnIoComplated;
    }

    public void Listen(int backlog)
    {
        this.Socket.Listen(backlog);
    }

    public void Accept()
    {
        // Client는 Accept 안함
    }

    public void SetEndPoint(string ip, int port)
    {
        this.EndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        this.connectArgs.RemoteEndPoint = this.EndPoint;
    }

    public void Connect()
    {
        bool pending = this.Socket.ConnectAsync(this.connectArgs);
        if (!pending)
            OnIoComplated(this.Socket, this.connectArgs);
    }

    public override void Receive()
    {        
        Array.Clear(this.ReadDataBuffer, 0, Defines.MAX_BUFFER_LENGTH);
        bool pending = this.Socket.ReceiveAsync(this.recvArgs);
        if(!pending)
            OnIoComplated(this.Socket, this.recvArgs);        
    }

    public override void Send(byte[] data, int length)
    {        
        this.sendArgs.SetBuffer(data, 0, length);

        bool pending = this.Socket.SendAsync(this.sendArgs);
        if (!pending)
            OnIoComplated(this.Socket, this.sendArgs);
    }
}
