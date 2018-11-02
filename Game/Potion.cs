using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour
{
    private string _potionID;
    public Action<Potion> ReleaseEvent;

    public string PotionID
    {
        get { return _potionID; }
        set { _potionID = value; }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && GameManager.Instance.IsHost)
        {
            Debug.Log("[Get Potion] " + other.GetComponent<GamePlayer>().Player.PlayerID);

            other.GetComponent<GamePlayer>().IncreaseHealth();

            CLIENTtoSERVER_PotionPickupPacketData data = new CLIENTtoSERVER_PotionPickupPacketData();
            data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
            data.gameKey = GameManager.Instance.GameKey;
            data.playerID = other.GetComponent<GamePlayer>().Player.PlayerID;
            data.potionID = PotionID;
            data.hp = other.GetComponent<GamePlayer>().Life;

            NetworkManager.Instance.SendCommand((int)PROTOCOL.POTION_PICKUP, (int)EXTRA.HOST_TO_SERVER, data);
            ReleaseEvent(this);
        }
    }
}
