using System;
using System.Collections.Generic;
using System.Threading;

public class Network
{
    private static Network instance;

    private TcpConnector connector;
    private Queue<Package> packageQueue;
    private Locker locker;
    private bool isConnected;

    private Queue<TcpPacket> sendQueue;
    private Locker sendLocker;

    private Thread sendThread;

    public static Network Instance
    {
        get
        {
            if (instance == null)
                instance = new Network();
            return instance;
        }
    }

    public TcpConnector Connector
    {
        get { return this.connector; }    
    }

    public bool IsConnected
    {
        get
        {
            if (connector == null)
                return false;

            return connector.IsConnected;
        }
    }

    public void Initialize()
    {
        this.connector = new TcpConnector();
        this.packageQueue = new Queue<Package>();
        this.locker = new Locker();
        this.locker.Initialize();

        //this.sendQueue = new Queue<TcpPacket>();
        //this.sendLocker = new Locker();
        //this.sendLocker.Initialize();
        //this.sendThread = new Thread(SendingThread);
        //this.sendThread.Start();
    }

    public void Connect(string ip, int port)
    {
        this.connector.SetConnectInfo(ip, port);
        this.connector.Connect();
    }

    public void Disconnect()
    {
        this.connector.Close();    
    }

    public bool IsGetPacket()
    {
        if (this.packageQueue.Count == 0)
            return false;
        else
            return true;
    }

    public void Execute()
    {
        this.locker.Lock();
        int queueSize = this.packageQueue.Count;

        for (int i = 0; i < queueSize; i++)
        {
            Package package = this.packageQueue.Dequeue();
            int protocol = package.packet.header.protocol;
            ProtocolManager.Instance.ExecuteRecvCommand(protocol, package.packet);
        }
        this.locker.Unlock();
    }

    public void PutPackage(Package package)
    {
        this.locker.Lock();
        this.packageQueue.Enqueue(package);
        this.locker.Unlock();
    }

    public void SendCommand(int protocol, int extra, BasePacketData data)
    {
        TcpPacket sendPacket = new TcpPacket();
        sendPacket.Initialize();
        data.Serialize(sendPacket);
        sendPacket.SetHeader(protocol, extra);

        #region 수정 전
        this.Connector.SendPacket(sendPacket);
        #endregion

        #region 쓰레드로 따로 빼서 전송
        //this.sendLocker.Lock();
        //this.sendQueue.Enqueue(sendPacket);
        //this.sendLocker.Unlock();
        #endregion
    }

    private void SendingThread()
    {
        while(true)
        {
            this.sendLocker.Lock();
            int size = this.sendQueue.Count;

            for (int i = 0; i < size; i++)
            {
                TcpPacket sendPacket = this.sendQueue.Dequeue();
                this.connector.SendPacket(sendPacket);
                UnityEngine.Debug.Log("send");
            }
            this.sendLocker.Unlock();
            System.Threading.Thread.Sleep(10);
        }
    }

    public void Release()
    {
        
    }
}
