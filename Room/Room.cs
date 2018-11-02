using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    private string roomName;
    private string roomKey;
    private int joinedPlayerCount;
    private List<RoomPlayer> roomPlayers;
    private RoomPlayer leader;
    private bool isStartGame;

    public Room()
    {
        this.roomPlayers = new List<RoomPlayer>();
    }

    public string RoomName
    {
        get { return this.roomName; }
        set { this.roomName = value; }
    }

    public string RoomKey
    {
        get { return this.roomKey; }
        set { this.roomKey = value; }
    }

    public int JoinedPlayerCount
    {
        get { return this.joinedPlayerCount; }
        set { this.joinedPlayerCount = value; }
    }

    public bool IsStartGame
    {
        get { return this.isStartGame; }
        set { this.isStartGame = value; }
    }

    public List<RoomPlayer> RoomPlayer
    {
        get { return this.roomPlayers; }
    }

    public void LeaveOtherPlayer(string playerID)
    {
        for (int i = 0; i < roomPlayers.Count; i++)
        {
            if (roomPlayers[i].player.PlayerID == playerID)
            {
                roomPlayers.RemoveAt(i);
                RoomGUI.Instance.UpdateRoomPlayerUIElement(this);
            }
        }
    }

    public void PushRoomPlayer(RoomPlayer roomPlayer)
    {
        this.roomPlayers.Add(roomPlayer);

        if (roomPlayer.player.IsMine())
            RoomManager.Instance.MyPlayer = roomPlayer;

        if (roomPlayer.isLeader)
            this.leader = roomPlayer;

        RoomGUI.Instance.UpdateRoomPlayerUIElement(this);
    }

    public void PushRoomPlayer(RoomPlayer[] roomPlayers)
    {        
        this.roomPlayers.Clear();

        for (int i = 0; i < roomPlayers.Length; i++)
        {
            this.roomPlayers.Add(roomPlayers[i]);

            if (roomPlayers[i].player.IsMine())
                RoomManager.Instance.MyPlayer = roomPlayers[i];

            if (roomPlayers[i].isLeader)
                this.leader = roomPlayers[i];
        }

        RoomGUI.Instance.UpdateRoomPlayerUIElement(this);
    }

    public void Enter()
    {
        RoomManager.Instance.EnteredRoom = this;
        GameFramework.Instance.MyPlayer.State = PLAYER_STATE.IN_ROOM;
    }

    public void ChangeLeader(string playerID)
    {
        // 1. 기존 방장 리셋
        // 2. 새로운 방장 임명

        for (int i = 0; i < roomPlayers.Count; i++)
        {
            if(roomPlayers[i].isLeader)
            {
                roomPlayers[i].isLeader = false;
                break;
            }
        }

        for (int i = 0; i < roomPlayers.Count; i++)
        {
            if (roomPlayers[i].player.PlayerID == playerID)
            {
                if(roomPlayers[i].player.IsMine())
                    RoomGUI.Instance.ChangeReadyToStartButton();

                roomPlayers[i].isLeader = true;
                this.leader = roomPlayers[i];
                SetReady(roomPlayers[i].player.PlayerID, false);
            }
        }
        RoomGUI.Instance.UpdateRoomPlayerUIElement(this);
    }

    public void SetReady(string playerID, bool readyState)
    {
        for(int i = 0; i < roomPlayers.Count; i++)
        {
            if(roomPlayers[i].player.PlayerID == playerID)
            {
                roomPlayers[i].readyState = readyState;

                if (readyState)
                    RoomGUI.Instance.ReadyPlayer(i);
                else
                    RoomGUI.Instance.CancleReadyPlayer(i);
            }
        }
    }

    public void StartGame()
    {
        this.isStartGame = true;

    }

    public void EndGame()
    {
        this.isStartGame = false;
    }

    public void Release()
    {
        roomName = "";
        roomKey = "";
        joinedPlayerCount = 0;
        roomPlayers.Clear();
        leader = null;
        isStartGame = false;
        
        RoomGUI.Instance.Release();
    }

    public void Leave()
    {
        roomName = "";
        roomKey = "";
        joinedPlayerCount = 0;
        roomPlayers.Clear();
        leader = null;
        isStartGame = false;

        RoomGUI.Instance.Release();
    }
}
