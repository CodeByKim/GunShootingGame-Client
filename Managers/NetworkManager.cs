using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum NETWORK_STATE
{
    LOCAL_HOST,
    VIRTUAL_MACHINE
}

public class NetworkManager : MonoBehaviour 
{
    private string serverIP;

    public NETWORK_STATE state;

    public LoginProtocol loginProtocol;
    public LobbyProtocol lobbyProtocol;
    public RoomProtocol roomProtocol;
    public TestProtocol testProtocol;
    public GameProtocol gameProtocol;

    public static NetworkManager Instance;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start () 
    {        
        if(state == NETWORK_STATE.LOCAL_HOST)
            this.serverIP = "127.0.0.1";
        else
            this.serverIP = "10.211.55.3";

        if (!Network.Instance.IsConnected)
        {
            Network.Instance.Initialize();
            ProtocolManager.Instance.Initialize();
            Network.Instance.Connect(this.serverIP, Defines.SERVER_PORT);
        }

        this.loginProtocol = transform.Find("Login Protocol").GetComponent<LoginProtocol>();
        this.lobbyProtocol = transform.Find("Lobby Protocol").GetComponent<LobbyProtocol>();
        this.roomProtocol = transform.Find("Room Protocol").GetComponent<RoomProtocol>();
        this.testProtocol = transform.Find("Test Protocol").GetComponent<TestProtocol>();
        this.gameProtocol = transform.Find("Game Protocol").GetComponent<GameProtocol>();

        this.loginProtocol.InitializeProtocol();
        this.lobbyProtocol.InitializeProtocol();
        this.roomProtocol.InitializeProtocol();
        this.testProtocol.InitializeProtocol();
        this.gameProtocol.InitializeProtocol();
        
        //Network.Instance.Connect(this.serverIP, Defines.SERVER_PORT);
    }

    private void Update () 
    {
        if(Network.Instance.IsConnected)
        {
            ExecutePacket();
        }
	}

    private void ExecutePacket()
    {
        if(Network.Instance.IsGetPacket())
        {            
            Network.Instance.Execute();
        }
    }

    public void SendCommand(int protocol, int extra, BasePacketData data)
    {
        Network.Instance.SendCommand(protocol, extra, data);
    }

    private void OnApplicationQuit()
    {
        Network.Instance.Disconnect();
        Debug.Log("Disconnect...");
    }
}
