using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomButton : MonoBehaviour
{
    private string roomName;
    private string roomKey;
    private int joinedPlayerCount;
    private bool isStart;

    private Text roomNameUIText;
    private Text waitingUIText;
    private Text joinedPlayerCountUIText;

    public string RoomKey
    {
        get { return this.roomKey; }
        set { this.roomKey = value; }
    }

    public int PlayerCount
    {
        get { return this.joinedPlayerCount; }
        set { this.joinedPlayerCount = value; }
    }

    public bool IsStart
    {
        get { return this.isStart; }
        set { this.isStart = value; }
    }

    public void Initialize(string roomKey, string roomName, bool isStart, int joinedPlayerCount)
    {
        this.roomNameUIText = transform.Find("Room Name").GetComponent<Text>();
        this.waitingUIText = transform.Find("Waiting Text").GetComponent<Text>();
        this.joinedPlayerCountUIText = transform.Find("Joined Count").GetComponent<Text>();

        this.roomKey = roomKey;
        this.roomName = roomName;
        this.joinedPlayerCount = joinedPlayerCount;

        this.roomNameUIText.text = this.roomName;
        this.waitingUIText.text = isStart ? "게임 중 ..." : "대기 중 ...";
        this.joinedPlayerCountUIText.text = this.joinedPlayerCount.ToString() + "/4";
    }

    public void OnEnterRoom()
    {
        // if(room.isStart) 이런 식으로 해야 할것 같은데...
        
        if(waitingUIText.text == "게임 중 ...")    //굉장히... 안좋아보임
        {
            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LobbySceneGUI.Instance.lobbyGUI);
            messageBox.SetTitle("이미 게임중인 방입니다.");

            return;
        }

        CLIENTtoSERVER_JoinRoomPacketData data = new CLIENTtoSERVER_JoinRoomPacketData();
        data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
        data.roomKey = roomKey;
        NetworkManager.Instance.SendCommand((int)PROTOCOL.JOIN_ROOM, (int)EXTRA.REQUEST, data);
    }

    public void IncreaseRoomJoinCount()
    {
        this.joinedPlayerCount += 1;
        this.joinedPlayerCountUIText.text = this.joinedPlayerCount.ToString() + "/4";
    }

    public void DecreaseRoomJoinCount()
    {
        this.joinedPlayerCount -= 1;
        this.joinedPlayerCountUIText.text = this.joinedPlayerCount.ToString() + "/4";
    }

    public void StartGame()
    {
        this.isStart = true;
        this.waitingUIText.text = "게임 중 ...";
    }

    public void EndGame()
    {
        this.isStart = false;
        this.waitingUIText.text = "대기 중 ...";
    }
}
