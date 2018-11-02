using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour 
{
    public static LobbyManager Instance;
    public List<LobbyPlayer> lobbyPlayers;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        this.lobbyPlayers = new List<LobbyPlayer>();
        LobbyGUI.Instance.Initialize();

        switch (GameFramework.Instance.MyPlayer.State)
        {
            case PLAYER_STATE.LOBBY:
                CLIENTtoSERVER_EnterLobbyPacketData data = new CLIENTtoSERVER_EnterLobbyPacketData();
                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                NetworkManager.Instance.SendCommand((int)PROTOCOL.ENTER_LOBBY, (int)EXTRA.REQUEST, data);

                break;

            case PLAYER_STATE.IN_ROOM:
                LobbySceneGUI.Instance.ChangeLobbyToRoom();

                foreach (var player in RoomManager.Instance.EnteredRoom.RoomPlayer)
                    RoomManager.Instance.EnteredRoom.SetReady(player.player.PlayerID, false);

                RoomGUI.Instance.Initialize(RoomManager.Instance.EnteredRoom);

                if(RoomManager.Instance.MyPlayer.isLeader)
                    RoomGUI.Instance.ChangeReadyToStartButton();
                break;
        }
    }

    public LobbyPlayer CreateLobbyPlayer(Player player)
    {
        GameObject lobbyPlayerPrefab = Resources.Load("Prefabs/Lobby Player 2") as GameObject;
        LobbyPlayerUIElement createdLobbyPlayer = Instantiate(lobbyPlayerPrefab).GetComponent<LobbyPlayerUIElement>();
        createdLobbyPlayer.Initialize(player.PlayerID);
        RectTransform userViewPanel = GameObject.Find("User View Content").GetComponent<RectTransform>();

        createdLobbyPlayer.GetComponent<RectTransform>().SetParent(userViewPanel);
        userViewPanel.sizeDelta = new Vector2(0, userViewPanel.sizeDelta.y + 30);

        LobbyPlayer lobbyPlayer = new LobbyPlayer(player);
        lobbyPlayer.uiElement = createdLobbyPlayer;

        return lobbyPlayer;
    }

    public void AttachRoomButton(RoomButton roomButton)
    {
        LobbyGUI.Instance.MoveToRoomPanel(roomButton);
        LobbyGUI.Instance.LayoutPositionOfRoom(RoomManager.Instance.RoomButtons);
    }

    public void CreateLobbyPlayer(string playerID)
    {
        if(playerID == GameFramework.Instance.MyPlayer.PlayerID)
        {            
            this.lobbyPlayers.Add(CreateLobbyPlayer(GameFramework.Instance.MyPlayer));
        }
        else
        {
            Player otherPlayer = new OtherPlayer();
            otherPlayer.Initialize();
            otherPlayer.PlayerID = playerID;
            this.lobbyPlayers.Add(CreateLobbyPlayer(otherPlayer));
        }
    }

    public void RemoveLobbyPlayer(string playerID)
    {
        LobbyPlayer removePlayer = this.lobbyPlayers.Find((player) => player.player.PlayerID == playerID);
        Destroy(removePlayer.uiElement.gameObject);
        this.lobbyPlayers.Remove(removePlayer);
    }

    public void ClearLobbyPlayers()
    {        
        for(int i = 0; i < lobbyPlayers.Count; i++)
            Destroy(lobbyPlayers[i].uiElement.gameObject);

        this.lobbyPlayers.Clear();      
    }

    public void Release()
    {
        ClearLobbyPlayers();
    }
}
