using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameInfo
{
    public List<Player> gamePlayers;
    public bool isHost;
    public string gameKey;

    public GameInfo()
    {
        gamePlayers = new List<Player>();        
    }
}

public class GameFramework : MonoBehaviour
{    
    public static GameFramework Instance;

    private GameInfo _gameInfo;
    private MyPlayer _myPlayer;

    public GameInfo GameInfo
    {
        get { return _gameInfo; }
    }

    public MyPlayer MyPlayer
    {
        get { return this._myPlayer; }
        set { this._myPlayer = value; }
    }

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
    }

    void Start ()
    {
        _gameInfo = new GameInfo();
        _myPlayer = new MyPlayer();
        _myPlayer.Initialize();
        _myPlayer.State = PLAYER_STATE.LOGIN;
    }

    public void EnterGameScene(string gameKey)
    {
        MyPlayer.State = PLAYER_STATE.IN_GAME;

        for (int i = 0; i < RoomManager.Instance.EnteredRoom.RoomPlayer.Count; i++)
            GameInfo.gamePlayers.Add(RoomManager.Instance.EnteredRoom.RoomPlayer[i].player);

        GameInfo.gameKey = gameKey;
        GameInfo.isHost = RoomManager.Instance.MyPlayer.isLeader ? true : false;
        SceneManager.LoadScene("Game");
    }

    public void LeaveGameScene()
    {
        MyPlayer.State = PLAYER_STATE.IN_ROOM;
        GameInfo.gamePlayers.Clear();
        GameInfo.isHost = false;
        GameInfo.gameKey = "";
        SceneManager.LoadScene("Lobby");
    }
}
