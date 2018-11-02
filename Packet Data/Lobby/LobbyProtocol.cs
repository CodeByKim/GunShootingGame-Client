using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyProtocol : MonoBehaviour
{
	void Start ()
    {
		
	}

    public void InitializeProtocol()
    {
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENTER_LOBBY, RESPONSE_ENTER_LOBBY);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.LEAVE_LOBBY, RESPONSE_LEAVE_LOBBY);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENTER_LOBBY_OTHER_PLAYER, RESPONSE_ENTER_LOBBY_OTHER_PLAYER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.LEAVE_LOBBY_OTHER_PLAYER, RESPONSE_LEAVE_LOBBY_OTHER_PLAYER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.LOBBY_CHATTING, RESPONSE_LOBBY_CHATTING);
    }

    public void RESPONSE_ENTER_LOBBY(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_EnterLobbyPacketData recvData = new SERVERtoCLIENT_EnterLobbyPacketData();
            recvData.Deserialize(packet);

            for (int i = 0; i < recvData.roomCount; i++)
            {
                RoomButton roomButton = RoomManager.Instance.CreateRoomButton(recvData.roomKeys[i], recvData.roomNames[i], recvData.isStart[i], recvData.joinedRoomPlayerCount[i]);
                LobbyManager.Instance.AttachRoomButton(roomButton);
            }

            for (int i = 0; i < recvData.playerCount; i++)
                LobbyManager.Instance.CreateLobbyPlayer(recvData.playerIds[i]);            
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("FAIL RESPONSE_LOBBY_INFO");
        }
    }

    public void RESPONSE_LEAVE_LOBBY(TcpPacket packet)
    {
        SERVERtoCLIENT_LeaveLobbyPacketData recvData = new SERVERtoCLIENT_LeaveLobbyPacketData();
        recvData.Deserialize(packet);

        Debug.Log("RESPONSE_LEAVE_LOBBY");
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            LobbyManager.Instance.Release();
            GameFramework.Instance.MyPlayer.State = PLAYER_STATE.LOGIN;
            SceneManager.LoadScene("Login");
        }
    }

    public void RESPONSE_ENTER_LOBBY_OTHER_PLAYER(TcpPacket packet)
    {
        if(GameFramework.Instance.MyPlayer.State == PLAYER_STATE.LOBBY)
        {
            SERVERtoCLIENT_EnterLobbyOtherPlayerPacketData recvData = new SERVERtoCLIENT_EnterLobbyOtherPlayerPacketData();
            recvData.Deserialize(packet);

            if (packet.header.extra == (int)EXTRA.FOR_LOGIN)
            {
                string chatMessage = "[Server] : " + recvData.playerName + "님이 접속하였습니다.";
                LobbyGUI.Instance.ShowChatMessage(chatMessage, Color.red);
            }

            LobbyManager.Instance.CreateLobbyPlayer(recvData.playerName);
        }
    }

    public void RESPONSE_LEAVE_LOBBY_OTHER_PLAYER(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.FOR_LOGOUT)
        {
            SERVERtoCLIENT_LeaveLobbyOtherPlayerPacketData recvData = new SERVERtoCLIENT_LeaveLobbyOtherPlayerPacketData();
            recvData.Deserialize(packet);
            
            GameObject chatText = Resources.Load("Prefabs/Chatting Text") as GameObject;
            Text createdChatText = Instantiate(chatText).GetComponent<Text>();
            createdChatText.text = "[Server] : " + recvData.playerName + "님이 로그아웃하였습니다.";
            createdChatText.color = Color.blue;
            RectTransform chatPanel = GameObject.Find("Content").GetComponent<RectTransform>();

            createdChatText.GetComponent<RectTransform>().SetParent(chatPanel);
            chatPanel.sizeDelta = new Vector2(0, chatPanel.sizeDelta.y + 30);
            chatPanel.anchoredPosition = new Vector2(0, chatPanel.anchoredPosition.y + 30);

            LobbyManager.Instance.RemoveLobbyPlayer(recvData.playerName);
        }
        else if (packet.header.extra == (int)EXTRA.FOR_JOIN_ROOM)
        {
            SERVERtoCLIENT_JoinRoomOtherPlayerPacketData recvData = new SERVERtoCLIENT_JoinRoomOtherPlayerPacketData();
            recvData.Deserialize(packet);

            LobbyManager.Instance.RemoveLobbyPlayer(recvData.playerID);
        }
    }

    public void RESPONSE_LOBBY_CHATTING(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_LobbyChattingPacketData recvData = new SERVERtoCLIENT_LobbyChattingPacketData();
            recvData.Deserialize(packet);

            string chatMessage = recvData.playerName + " : " + recvData.chatString;
            LobbyGUI.Instance.ShowChatMessage(chatMessage, Color.black);
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("FAIL LOBBY CHATTING...");
        }
    }
}
