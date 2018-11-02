using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomGUI : MonoBehaviour
{
    public static RoomGUI Instance;

    public Text titleText;              
    public RectTransform chatPanel;     
    public InputField chatInputField;
    public Button readyButton;

    public RoomPlayerUIElement[] roomPlayerUI;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize(Room room)
    {
        titleText.text = room.RoomName;
        //readyButton.onClick.AddListener(OnReady);

        for (int i = 0; i < room.RoomPlayer.Count; i++)
        {
            roomPlayerUI[i].gameObject.SetActive(true);
            roomPlayerUI[i].Initialize(room.RoomPlayer[i].player.PlayerID);

            if (room.RoomPlayer[i].readyState)
                roomPlayerUI[i].SetReady();
            else
                roomPlayerUI[i].CancelReady();

            if (room.RoomPlayer[i].isLeader)
            {
                roomPlayerUI[i].SetLeader();

                RoomPlayerUIElement element = roomPlayerUI[i];

                for(int j = i; j > 0; j--)
                    roomPlayerUI[j] = roomPlayerUI[j - 1];

                roomPlayerUI[0] = element;
            }
        }
    }

    public void Release()
    {
        //ChangeStartToReadyButton();
        readyButton.transform.Find("Text").GetComponent<Text>().text = "Ready";
        readyButton.onClick.RemoveAllListeners();
        for (int i = 0; i < 4; i++)
        {
            roomPlayerUI[i].CancelReady();
            roomPlayerUI[i].gameObject.SetActive(false);
        }
    }

    public void UpdateRoomPlayerUIElement(Room room)
    {
        for (int i = 0; i < 4; i++)
        {
            roomPlayerUI[i].Release();
            roomPlayerUI[i].gameObject.SetActive(false);
        }
           
        for (int i = 0; i < room.RoomPlayer.Count; i++)
        {
            roomPlayerUI[i].gameObject.SetActive(true);
            roomPlayerUI[i].Initialize(room.RoomPlayer[i].player.PlayerID);

            if (room.RoomPlayer[i].isLeader)
                roomPlayerUI[i].SetLeader();

            if (room.RoomPlayer[i].readyState)
                roomPlayerUI[i].SetReady();
            else
                roomPlayerUI[i].CancelReady();
        }
    }

    public void ReadyPlayer(int index)
    {
        roomPlayerUI[index].SetReady();
    }

    public void CancleReadyPlayer(int index)
    {
        roomPlayerUI[index].CancelReady();
    }

    public void ChangeReadyToStartButton()
    {
        readyButton.transform.Find("Text").GetComponent<Text>().text = "Start !";
        //readyButton.onClick.RemoveAllListeners();
        //readyButton.onClick.AddListener(OnStart);
    }

    public void ChangeStartToReadyButton()
    {
        readyButton.transform.Find("Text").GetComponent<Text>().text = "Ready !";
        //readyButton.onClick.RemoveAllListeners();
        //readyButton.onClick.AddListener(OnReady);
    }

    /*public void ChangeLeader(string playerID)
    {
        for (int i = 0; i < roomPlayerUI.Length; i++)
        {
            if (roomPlayerUI[i].playerID == playerID)
                roomPlayerUI[i].SetLeader();
        }
    }

    public void CancelLeader(string playerID)
    {
        for (int i = 0; i < roomPlayerUI.Length; i++)
        {
            if (roomPlayerUI[i].playerID == playerID)
                roomPlayerUI[i].CancelLeader();
        }
    }*/

    public void ShowChatMessage(string chatMessage, Color color)
    {
        GameObject chatText = Resources.Load("Prefabs/Chatting Text") as GameObject;
        Text createdChatText = Instantiate(chatText).GetComponent<Text>();
        createdChatText.text = chatMessage;
        createdChatText.color = color;

        createdChatText.GetComponent<RectTransform>().SetParent(chatPanel);
        chatPanel.sizeDelta = new Vector2(0, chatPanel.sizeDelta.y + 30);
        chatPanel.anchoredPosition = new Vector2(0, chatPanel.anchoredPosition.y + 30);
    }

    #region UI Action
    public void OnSendChatting()
    {
        CLIENTtoSERVER_RoomChattingPacketData data = new CLIENTtoSERVER_RoomChattingPacketData
        {
            playerKey = GameFramework.Instance.MyPlayer.PlayerKey,
            roomKey = RoomManager.Instance.EnteredRoom.RoomKey,
            chatString = this.chatInputField.text
        };
   
        NetworkManager.Instance.SendCommand((int)PROTOCOL.ROOM_CHATTING, (int)EXTRA.REQUEST, data);

        this.chatInputField.text = "";
        Debug.Log("Send Chat...");

    }

    public void OnReadyAndStart()
    {
        SoundManager.Instance.PlayUIButtonClick();

        if (!RoomManager.Instance.MyPlayer.isLeader)
        {
            CLIENTtoSERVER_ReadyPacketData data = new CLIENTtoSERVER_ReadyPacketData
            {
                playerKey = GameFramework.Instance.MyPlayer.PlayerKey,
                roomKey = RoomManager.Instance.EnteredRoom.RoomKey
            };

            NetworkManager.Instance.SendCommand((int)PROTOCOL.READY, (int)EXTRA.REQUEST, data);
            Debug.Log("Ready");
        }
        else
        {
            CLIENTtoSERVER_RoomStartPacketData data = new CLIENTtoSERVER_RoomStartPacketData
            {
                playerKey = GameFramework.Instance.MyPlayer.PlayerKey,
                roomKey = RoomManager.Instance.EnteredRoom.RoomKey
            };

            NetworkManager.Instance.SendCommand((int)PROTOCOL.ROOM_START, (int)EXTRA.REQUEST, data);
            Debug.Log("OnStart...");
        }
    }

    public void OnLeaveRoom()
    {
        SoundManager.Instance.PlayUIButtonClick();
        if (!RoomManager.Instance.MyPlayer.readyState)
        {
            CLIENTtoSERVER_LeaveRoomPacketData data = new CLIENTtoSERVER_LeaveRoomPacketData
            {
                playerKey = GameFramework.Instance.MyPlayer.PlayerKey,
                roomKey = RoomManager.Instance.EnteredRoom.RoomKey
            };

            NetworkManager.Instance.SendCommand((int)PROTOCOL.LEAVE_ROOM, (int)EXTRA.REQUEST, data);
        }
        else
        {
            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LobbySceneGUI.Instance.roomGUI);
            messageBox.SetTitle("Ready를 해제하고 나가십시오.");
        }
    }
    #endregion
}
