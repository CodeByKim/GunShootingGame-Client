using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneGUI : MonoBehaviour
{
    public static LobbySceneGUI Instance;

    public GameObject lobbyGUI;
    public GameObject roomGUI;

    private void Awake()
    {
        Instance = this; 
    }

    public void ChangeLobbyToRoom()
    {
        RoomManager.Instance.ClearRoomButtons();
        LobbyManager.Instance.ClearLobbyPlayers();
        
        ClearGenerateUIElements();

        this.lobbyGUI.SetActive(false);
        this.roomGUI.SetActive(true);
    }

    public void ChangeRoomToLobby()
    {
        this.roomGUI.SetActive(false);
        this.lobbyGUI.SetActive(true);
             
        RoomManager.Instance.ClearRoomButtons();
        LobbyManager.Instance.ClearLobbyPlayers();
    }

    public void ClearGenerateUIElements()
    {
        Transform tr = transform.Find("Generate UI Elements");
        for (int i = 0; i < tr.childCount; i++)
        {
            Destroy(tr.GetChild(i).gameObject);
        }
    }
}
