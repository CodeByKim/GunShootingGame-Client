using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private List<GamePlayer> _gamePlayers;
    private bool _isHost;
    private string _gameKey;
    private bool _isGameStart;

    public static GameManager Instance;
    public GameObject gamePlayerPrefab;

    public List<GamePlayer> GamePlayers
    {
        get { return _gamePlayers; }
    }

    public bool IsHost
    {
        get { return _isHost; }
        set { _isHost = value; }
    }

    public string GameKey
    {
        get { return _gameKey; }
        set { _gameKey = value; }
    }

    public bool IsGameStart
    {
        get { return _isGameStart; }
        set { _isGameStart = value; }
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start ()
    {
        IsHost = GameFramework.Instance.GameInfo.isHost;
        GameKey = GameFramework.Instance.GameInfo.gameKey;

        _gamePlayers = new List<GamePlayer>();

        CLIENTtoSERVER_GameStartPacketData data = new CLIENTtoSERVER_GameStartPacketData();
        data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
        data.gameKey = GameKey;

        for (int i = 0; i < GameFramework.Instance.GameInfo.gamePlayers.Count; i++)
        {
            //GamePlayer player = Instantiate(gamePlayerPrefab, new Vector3(i * 3, 1, 0), Quaternion.identity).GetComponent<GamePlayer>();
            // 플레이어 생성 위치 수정
            GamePlayer player = Instantiate(gamePlayerPrefab, new Vector3(i*2-3, 1, 0), Quaternion.identity).GetComponent<GamePlayer>();
            player.Initialize(GameFramework.Instance.GameInfo.gamePlayers[i], PlayerDeath);
            _gamePlayers.Add(player);
        }

        NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_START, (int)EXTRA.REQUEST, data);        
    }
	
    public void Initialize()
    {
        IsGameStart = true;
    }

    public void PlayerDeath(GamePlayer gamePlayer)
    {        
        GamePlayers.Remove(gamePlayer);
        Destroy(gamePlayer.gameObject);

        if(IsHost)
        {
            if (GamePlayers.Count == 0)
            {
                // 게임 종료
                IsGameStart = false;
                StartCoroutine(GameEnd());
            }
        }
    }

    private IEnumerator GameEnd()
    {
        GameGUI.Instance.ShowGameOver();
        IsGameStart = false;

        foreach (var player in GamePlayers)
            Debug.Log(player.Player.PlayerID + " : " + player.KillCount);

        yield return new WaitForSeconds(2);

        CLIENTtoSERVER_GameEndPacketData data = new CLIENTtoSERVER_GameEndPacketData();
        data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
        data.gameKey = GameManager.Instance.GameKey;
        NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_END, (int)EXTRA.REQUEST, data);
    }

	void Update ()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            //StartCoroutine(GameEnd());
        }
	}
}
