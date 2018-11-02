using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TcpConnector : TcpSession
{
    public TcpConnector()
    {

    }

    public void SetConnectInfo(string ip, int port)
    {
        TcpSocket socket = this.socket as TcpSocket;
        socket.SetEndPoint(ip, port);
    }

    public void Connect()
    {
        TcpSocket socket = this.socket as TcpSocket;
        socket.Connect();
    }
}
