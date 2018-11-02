using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RoomProtocol : MonoBehaviour
{
    void Start()
    {

    }

    public void InitializeProtocol()
    {
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.CREATE_ROOM, RESPONSE_CREATE_ROOM);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.LEAVE_ROOM, RESPONSE_LEAVE_ROOM);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.JOIN_ROOM, RESPONSE_JOIN_ROOM);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.READY, RESPONSE_READY);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.REMOVE_ROOM, RESPONSE_REMOVE_ROOM);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.CHANGE_ROOM_LEADER, RESPONSE_CHANGER_ROOM_LEADER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.CREATE_ROOM_OTHER_PLAYER, RESPONSE_CREATE_ROOM_OTHER_PLAYER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.LEAVE_ROOM_OTHER_PLAYER, RESPONSE_LEAVE_ROOM_OTHER_PLAYER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.JOIN_ROOM_OTHER_PLAYER, RESPONSE_JOIN_ROOM_OTHER_PLAYER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.READY_OTHER_PLAYER, RESPONSE_READY_OTHER_PLAYER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ROOM_CHATTING, RESPONSE_ROOM_CHATTING);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ROOM_START, RESPONSE_ROOM_START);
    }

    public void RESPONSE_CREATE_ROOM(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_CreateRoomPacketData recvData = new SERVERtoCLIENT_CreateRoomPacketData();
            recvData.Deserialize(packet);

            //LobbyManager.Instance.Release();
            LobbySceneGUI.Instance.ChangeLobbyToRoom();

            RoomPlayer roomPlayer = RoomManager.Instance.CreateRoomPlayer(GameFramework.Instance.MyPlayer);
            roomPlayer.isLeader = true;

            Room room = RoomManager.Instance.CreateRoom(recvData.roomName, recvData.roomKey, 1);
            room.PushRoomPlayer(new RoomPlayer[] { roomPlayer });
            room.Enter();

            RoomGUI.Instance.Initialize(room);
            RoomGUI.Instance.ChangeReadyToStartButton();
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("OVERLAPED ROOM NAME...");

            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LobbySceneGUI.Instance.lobbyGUI);
            messageBox.SetTitle("중복된 방 이름입니다.");
        }
    }

    public void RESPONSE_LEAVE_ROOM(TcpPacket packet)
    {
        SERVERtoCLIENT_LeaveRoomPacketData recvData = new SERVERtoCLIENT_LeaveRoomPacketData();
        recvData.Deserialize(packet);

        RoomManager.Instance.EnteredRoom.Leave();
        RoomManager.Instance.EnteredRoom = null;

        LobbySceneGUI.Instance.ChangeRoomToLobby();

        for (int i = 0; i < recvData.roomCount; i++)
        {           
            RoomButton roomButton = RoomManager.Instance.CreateRoomButton(recvData.roomKeys[i], recvData.roomNames[i], recvData.isStart[i], recvData.joinedRoomPlayerCount[i]);
            LobbyManager.Instance.AttachRoomButton(roomButton);
        }

        for (int i = 0; i < recvData.playerCount; i++)
            LobbyManager.Instance.CreateLobbyPlayer(recvData.playerNames[i]);

        GameFramework.Instance.MyPlayer.State = PLAYER_STATE.LOBBY;
    }

    public void RESPONSE_JOIN_ROOM(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_JoinRoomPacketData recvData = new SERVERtoCLIENT_JoinRoomPacketData();
            recvData.Deserialize(packet);

            LobbySceneGUI.Instance.ChangeLobbyToRoom();
            RoomPlayer[] roomPlayers = new RoomPlayer[recvData.joinedPlayerCount];

            for (int i = 0; i < recvData.joinedPlayerCount; i++)
            {
                Player player;

                if (recvData.joinedPlayers[i] != GameFramework.Instance.MyPlayer.PlayerID)
                {
                    player = new OtherPlayer();
                    player.Initialize();
                    player.PlayerID = recvData.joinedPlayers[i];
                }
                else
                    player = GameFramework.Instance.MyPlayer;

                RoomPlayer roomPlayer = RoomManager.Instance.CreateRoomPlayer(player);
                roomPlayer.readyState = recvData.joinedPlayerReadyState[i];
                roomPlayer.isLeader = recvData.isLeaders[i];
                roomPlayers[i] = roomPlayer;
            }

            Room room = RoomManager.Instance.CreateRoom(recvData.roomName, recvData.roomKey, recvData.joinedPlayerCount);
            room.PushRoomPlayer(roomPlayers);
            room.Enter();

            RoomGUI.Instance.Initialize(room);            
        }
    }

    public void RESPONSE_CHANGER_ROOM_LEADER(TcpPacket packet)
    {
        SERVERtoCLIENT_ChangeRoomLeaderPacketData recvData = new SERVERtoCLIENT_ChangeRoomLeaderPacketData();
        recvData.Deserialize(packet);

        RoomManager.Instance.EnteredRoom.ChangeLeader(recvData.playerID);
    }

    public void RESPONSE_READY(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_ReadyPacketData recvData = new SERVERtoCLIENT_ReadyPacketData();
            recvData.Deserialize(packet);

            RoomManager.Instance.EnteredRoom.SetReady(GameFramework.Instance.MyPlayer.PlayerID, recvData.ready);

            //if (RoomManager.Instance.MyPlayer.readyState)
                //RoomManager.Instance.EnteredRoom.SetReady(GameFramework.Instance.MyPlayer.PlayerID, false);
            //else
                //RoomManager.Instance.EnteredRoom.SetReady(GameFramework.Instance.MyPlayer.PlayerID, true);
        }
    }

    public void RESPONSE_ROOM_START(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_RoomStartPacketData recvData = new SERVERtoCLIENT_RoomStartPacketData();
            recvData.Deserialize(packet);
            
            RoomManager.Instance.EnteredRoom.IsStartGame = true;
            GameFramework.Instance.EnterGameScene(recvData.gameKey);
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LobbySceneGUI.Instance.roomGUI);
            messageBox.SetTitle("모든 인원이 준비하지 않았습니다.");
        }
    }

    public void RESPONSE_REMOVE_ROOM(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_RemoveRoomPacketData recvData = new SERVERtoCLIENT_RemoveRoomPacketData();
            recvData.Deserialize(packet);

            RoomManager.Instance.RemoveRoomButton(recvData.roomKey);            
        }
    }

    public void RESPONSE_CREATE_ROOM_OTHER_PLAYER(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.FOR_JOIN_ROOM)
        {
            SERVERtoCLIENT_CreateRoomOtherPlayerPacketData recvData = new SERVERtoCLIENT_CreateRoomOtherPlayerPacketData();
            recvData.Deserialize(packet);

            RoomButton roomButton = RoomManager.Instance.CreateRoomButton(recvData.roomKey, recvData.roomName, false, 1);
            LobbyManager.Instance.AttachRoomButton(roomButton);

            LobbyManager.Instance.RemoveLobbyPlayer(recvData.playerName);

            Debug.Log("Create Room By Other Player...");
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("FAIL Create Room By Other Player");
        }
    }

    public void RESPONSE_LEAVE_ROOM_OTHER_PLAYER(TcpPacket packet)
    {
        SERVERtoCLIENT_LeaveRoomOtherPlayerPacketData recvData = new SERVERtoCLIENT_LeaveRoomOtherPlayerPacketData();
        recvData.Deserialize(packet);

        if (GameFramework.Instance.MyPlayer.State == PLAYER_STATE.LOBBY)
        {
            LobbyManager.Instance.CreateLobbyPlayer(recvData.playerID);

            RoomButton roomButton = RoomManager.Instance.GetRoomButton(recvData.roomKey);
            if(roomButton != null)
                roomButton.DecreaseRoomJoinCount();
        }
        else if (GameFramework.Instance.MyPlayer.State == PLAYER_STATE.IN_ROOM)
        {
            RoomManager.Instance.EnteredRoom.LeaveOtherPlayer(recvData.playerID);
        }
    }

    public void RESPONSE_JOIN_ROOM_OTHER_PLAYER(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_JoinRoomOtherPlayerPacketData recvData = new SERVERtoCLIENT_JoinRoomOtherPlayerPacketData();
            recvData.Deserialize(packet);

            if (GameFramework.Instance.MyPlayer.State == PLAYER_STATE.LOBBY)
            {
                //내가 Lobby에 있을때 다른사람이 Room으로 들어갔으므로 오른쪽 유저 리스트에 해당 유저를 지워야 함
                LobbyManager.Instance.RemoveLobbyPlayer(recvData.playerID);
                //해당 방에 사람이 들어갔으므로 방 참여자수 1 증가
                RoomButton roomButton = RoomManager.Instance.GetRoomButton(recvData.roomKey);
                if (roomButton != null)
                    roomButton.IncreaseRoomJoinCount();
            }
            else if (GameFramework.Instance.MyPlayer.State == PLAYER_STATE.IN_ROOM)
            {
                //내가 Room에 있을때 다른사람이 Room에서 들어왔으므로 방에 해당 유저를 추가해야 함

                string chatMessage = "[Server] : " + recvData.playerID + "님이 방에 입장하였습니다.";
                RoomGUI.Instance.ShowChatMessage(chatMessage, Color.red);

                Player player = new OtherPlayer();
                player.Initialize();
                player.PlayerID = recvData.playerID;

                RoomPlayer roomPlayer = RoomManager.Instance.CreateRoomPlayer(player);
                roomPlayer.readyState = false;
                roomPlayer.isLeader = false;

                RoomManager.Instance.EnteredRoom.PushRoomPlayer(roomPlayer);
                RoomGUI.Instance.UpdateRoomPlayerUIElement(RoomManager.Instance.EnteredRoom);
            }
        }
    }

    public void RESPONSE_READY_OTHER_PLAYER(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_ReadyOtherPlayerPacketData recvData = new SERVERtoCLIENT_ReadyOtherPlayerPacketData();
            recvData.Deserialize(packet);

            RoomManager.Instance.EnteredRoom.SetReady(recvData.playerID, recvData.ready);
        }
    }

    public void RESPONSE_ROOM_CHATTING(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_RoomChattingPacketData recvData = new SERVERtoCLIENT_RoomChattingPacketData();
            recvData.Deserialize(packet);

            string chatMessage = recvData.playerName + " : " + recvData.chatString;
            RoomGUI.Instance.ShowChatMessage(chatMessage, Color.black);
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("FAIL LOBBY CHATTING...");
        }
    }
}
