using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayerUIElement : MonoBehaviour
{
    public string playerID;
    public Text displayedPlayerIdText;

    public void Initialize(string playerID)
    {
        this.playerID = playerID;
        this.displayedPlayerIdText.text = playerID;
    }
}