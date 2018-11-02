using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGUI : MonoBehaviour
{
    public static LobbyGUI Instance;

    public InputField chatInputField;
    public RectTransform roomPanel;

    public int pageNumber;

    public const int ROOM_X_OFFSET = -180;
    public const int ROOM_Y_OFFSET = 150;

    public const int ROOM_WIDTH = 2;
    public const int ROOM_HEIGHT = 3;

    private void Awake()
    {
        Instance = this;
    }

    public void Initialize()
    {
        this.pageNumber = 0;
    }

    public void OnCreateRoom()
    {
        SoundManager.Instance.PlayUIButtonClick();
        PromptWindow window = UIFactory.Instance.Create(UI_ELEMENT.PROMPT_WINDOW).GetComponent<PromptWindow>();
        window.Initialize();
        window.AttachUIElement(new Vector2(0, 50), LobbySceneGUI.Instance.lobbyGUI);
        window.SetTitle("생성 할 방의 이름을 입력하시오.");

        window.AddConfirmEventFunc(() => {
            CLIENTtoSERVER_CreateRoomPacketData data = new CLIENTtoSERVER_CreateRoomPacketData();
            data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
            data.roomName = window.GetInputText();
            NetworkManager.Instance.SendCommand((int)PROTOCOL.CREATE_ROOM, (int)EXTRA.REQUEST, data);
        });
    }

    public void OnGoToLobby()
    {
        SoundManager.Instance.PlayUIButtonClick();
        CLIENTtoSERVER_LeaveLobbyPacketData data = new CLIENTtoSERVER_LeaveLobbyPacketData();
        data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
        NetworkManager.Instance.SendCommand((int)PROTOCOL.LEAVE_LOBBY, (int)EXTRA.REQUEST, data);
    }

    public void OnSendChatting()
    {
        CLIENTtoSERVER_LobbyChattingPacketData data = new CLIENTtoSERVER_LobbyChattingPacketData();
        data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
        data.chatString = this.chatInputField.text;
        NetworkManager.Instance.SendCommand((int)PROTOCOL.LOBBY_CHATTING, (int)EXTRA.REQUEST, data);

        this.chatInputField.text = "";
        Debug.Log("Send Chat...");
    }

    public void MoveToRoomPanel(RoomButton roomButton)
    {
        roomButton.GetComponent<RectTransform>().SetParent(roomPanel);
    }

    public void ShowChatMessage(string message, Color color)
    {
        GameObject chatText = Resources.Load("Prefabs/Chatting Text") as GameObject;
        Text createdChatText = Instantiate(chatText).GetComponent<Text>();
        createdChatText.text = message;
        createdChatText.color = color;
        RectTransform chatPanel = GameObject.Find("Content").GetComponent<RectTransform>();

        createdChatText.GetComponent<RectTransform>().SetParent(chatPanel);
        chatPanel.sizeDelta = new Vector2(0, chatPanel.sizeDelta.y + 30);
        chatPanel.anchoredPosition = new Vector2(0, chatPanel.anchoredPosition.y + 30);
    }

    public void RemoveRoomButton(RoomButton roomButton)
    {
        Destroy(roomButton.gameObject);
    }

    public void OnNextRoomPage()
    {
        SoundManager.Instance.PlayUIButtonClick();
        NextPage(RoomManager.Instance.RoomButtons);
    }

    public void OnPrevRoomPage()
    {
        SoundManager.Instance.PlayUIButtonClick();
        PrevPage(RoomManager.Instance.RoomButtons);
    }

    public void NextPage(List<RoomButton> roomButtons)
    {
        if (roomButtons.Count / ROOM_WIDTH * ROOM_HEIGHT <= this.pageNumber)
        {
            this.pageNumber = roomButtons.Count / ROOM_WIDTH * ROOM_HEIGHT;
            return;
        }

        for (int y = 0; y < ROOM_HEIGHT; y++)
        {
            for (int x = 0; x < ROOM_WIDTH; x++)
            {
                if (roomButtons.Count > y * ROOM_WIDTH + x)
                {
                    roomButtons[y * ROOM_WIDTH + x + (pageNumber * ROOM_WIDTH * ROOM_HEIGHT)].gameObject.SetActive(false);
                }
            }
        }

        this.pageNumber += 1;
        LayoutPositionOfRoom(roomButtons);
    }

    public void PrevPage(List<RoomButton> roomButtons)
    {
        if (this.pageNumber <= 0)
        {
            this.pageNumber = 0;
            return;
        }

        for (int y = 0; y < ROOM_HEIGHT; y++)
        {
            for (int x = 0; x < ROOM_WIDTH; x++)
            {
                if (roomButtons.Count > y * ROOM_WIDTH + x + (pageNumber * ROOM_WIDTH * ROOM_HEIGHT))
                {
                    roomButtons[y * ROOM_WIDTH + x + (pageNumber * ROOM_WIDTH * ROOM_HEIGHT)].gameObject.SetActive(false);
                }
            }
        }

        this.pageNumber -= 1;
        LayoutPositionOfRoom(roomButtons);
    }

    public void LayoutPositionOfRoom(List<RoomButton> roomButtons)
    {
        for (int y = 0; y < ROOM_HEIGHT; y++)
        {
            for (int x = 0; x < ROOM_WIDTH; x++)
            {
                if (roomButtons.Count > y * ROOM_WIDTH + x + (pageNumber * ROOM_WIDTH * ROOM_HEIGHT))
                {
                    roomButtons[y * ROOM_WIDTH + x + (pageNumber * ROOM_WIDTH * ROOM_HEIGHT)].gameObject.SetActive(true);
                    roomButtons[y * ROOM_WIDTH + x + (pageNumber * ROOM_WIDTH * ROOM_HEIGHT)].GetComponent<RectTransform>().anchoredPosition = new Vector2(x * 360 + ROOM_X_OFFSET, y * -120 + ROOM_Y_OFFSET);
                }
            }
        }
    }
}