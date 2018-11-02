using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPlayerUIElement : MonoBehaviour
{
    public string playerID;
    public Text displayedPlayerIdText;
    public GameObject readStateText;

    public void Initialize(string playerID)
    {
        this.playerID = playerID;
        this.displayedPlayerIdText.text = this.playerID;
        this.displayedPlayerIdText.color = Color.black;
    }

    public void Release()
    {
        this.playerID = "";
        this.displayedPlayerIdText.text = "";
        readStateText.SetActive(false);
        CancelLeader();
    }

    public void SetReady()
    {
        this.readStateText.SetActive(true);
    }

    public void CancelReady()
    {
        this.readStateText.SetActive(false);
    }

    public void SetLeader()
    {
        this.displayedPlayerIdText.text = this.playerID + " (L)";
        this.displayedPlayerIdText.color = Color.red;
    }

    public void CancelLeader()
    {
        this.displayedPlayerIdText.text = this.playerID;
        this.displayedPlayerIdText.color = Color.black;
    }
}