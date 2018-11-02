using System;
using System.Collections.Generic;

public class ProtocolManager
{
    private static ProtocolManager instance;
    private Dictionary<int, Action<TcpPacket>> recvProtocolFuncDict;

    public static ProtocolManager Instance
    {
        get
        {
            if (instance == null)
                instance = new ProtocolManager();
            return instance;
        }
    }

    public void Initialize()
    {
        this.recvProtocolFuncDict = new Dictionary<int, Action<TcpPacket>>();
    }

    public void RegisterResponseProtocolFunc(PROTOCOL protocol, Action<TcpPacket> protocolFunc)
    {        
        this.recvProtocolFuncDict[(int)protocol] = protocolFunc;
    }

    public void ExecuteRecvCommand(int protocol, TcpPacket packet)
    {
        this.recvProtocolFuncDict[protocol](packet);
    }
}
