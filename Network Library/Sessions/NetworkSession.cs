using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NetworkSession
{
    protected RawSocket socket;
    protected string id;

    public RawSocket Socket
    {
        get { return this.socket; }
        set { this.socket = value; }
    }

    public string ID
    {
        get { return this.id; }
        set { this.id = value; }
    }

    public NetworkSession()
    {

    }

    public abstract void SendPacket(TcpPacket packet);
    public abstract void OnReceive(int readDataLength);

    public void Bind()
    {

    }

    public void Close()
    {
        this.socket.Close();   
    }
}
