using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public abstract class RawSocket
{
    private Socket socket;
    private IPEndPoint endPoint;

    //private int readDataLength;
    private byte[] readDataBuffer;

    protected SocketAsyncEventArgs recvArgs;
    protected SocketAsyncEventArgs sendArgs;

    public abstract void Receive();
    public abstract void Send(byte[] data, int length);

    public Socket Socket
    {
        get { return this.socket; }
        set { this.socket = value; }
    }

    public IPEndPoint EndPoint
    {
        get { return this.endPoint; }
        set { this.endPoint = value; }
    }

    public byte[] ReadDataBuffer
    {
        get { return this.readDataBuffer; }    
    }

    public RawSocket()
    {
        this.socket = null;
        this.endPoint = null;
        //this.readDataLength = 0;
        this.readDataBuffer = new byte[Defines.MAX_BUFFER_LENGTH];

        recvArgs = new SocketAsyncEventArgs();
        sendArgs = new SocketAsyncEventArgs();

        recvArgs.Completed += OnIoComplated;
        recvArgs.SetBuffer(this.readDataBuffer, 0, Defines.MAX_BUFFER_LENGTH);
        sendArgs.Completed += OnIoComplated;
    }

    protected void OnIoComplated(object sender, SocketAsyncEventArgs e)
    {
        switch(e.LastOperation)
        {
            case SocketAsyncOperation.Connect:
                if (e.SocketError == SocketError.Success)
                {
                    UnityEngine.Debug.Log("Connect to Server...");
                    Network.Instance.Connector.SetConnected();
                }
                else
                {
                    UnityEngine.Debug.Log("Fail");
                    return;
                }
                break;
            case SocketAsyncOperation.Receive:
                Network.Instance.Connector.OnReceive(e.BytesTransferred);
                break;
            case SocketAsyncOperation.Send:
                break;
            case SocketAsyncOperation.Disconnect:
                break;
        }
    }

    public void Bind()
    {
        // Client´Â Bind ¾ÈÇÔ
    }

    public void Close()
    {
        this.socket.Close();
    }
}
