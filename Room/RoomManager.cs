using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager
{
    private static RoomManager _instance;
    private List<RoomButton> roomButtons;
    private Room enteredRoom;
    private RoomPlayer myPlayer;

    public static RoomManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new RoomManager();
            return _instance;
        }
    }

    public Room EnteredRoom
    {
        get { return this.enteredRoom; }
        set { this.enteredRoom = value; }
    }

    public RoomPlayer MyPlayer
    {
        get { return this.myPlayer; }
        set { this.myPlayer = value; }
    }

    public List<RoomButton> RoomButtons
    {
        get { return this.roomButtons; }
    }

    public RoomManager()
    {
        this.roomButtons = new List<RoomButton>();
    }

    public RoomButton GetRoomButton(string roomKey)
    {
        RoomButton roomButton = this.roomButtons.Find((room) => room.RoomKey == roomKey);
        return roomButton;
    }

    public RoomPlayer CreateRoomPlayer(Player player)
    {
        RoomPlayer roomPlayer = new RoomPlayer();
        roomPlayer.player = player;
        roomPlayer.readyState = false;

        return roomPlayer;
    }

    public Room CreateRoom(string roomName, string roomKey, int joinedPlayerCount)
    {
        Room room = new Room();
        room.RoomName = roomName;
        room.RoomKey = roomKey;
        room.JoinedPlayerCount = joinedPlayerCount;
        room.IsStartGame = false;

        return room;
    }

    public RoomButton CreateRoomButton(string roomKey, string roomName, bool isStart, int joinedPlayerCount)
    {        
        GameObject room = Resources.Load("Prefabs/Room Button 2") as GameObject;
        GameObject createdRoom = UnityEngine.Object.Instantiate(room);
        createdRoom.GetComponent<RoomButton>().Initialize(roomKey, roomName, isStart, joinedPlayerCount);
        createdRoom.SetActive(false);

        this.roomButtons.Add(createdRoom.GetComponent<RoomButton>());
        return createdRoom.GetComponent<RoomButton>();
    }

    public void RemoveRoomButton(string roomKey)
    {
        RoomButton removeRoom = this.roomButtons.Find((room) => room.RoomKey == roomKey);

        if(removeRoom != null)
        {
            this.roomButtons.Remove(removeRoom);

            LobbyGUI.Instance.RemoveRoomButton(removeRoom);
            LobbyGUI.Instance.LayoutPositionOfRoom(roomButtons);
        }
    }

    public void ClearRoomButtons()
    {
        for (int i = 0; i < roomButtons.Count; i++)
            GameObject.Destroy(roomButtons[i].gameObject);

        this.roomButtons.Clear();
    }
}
