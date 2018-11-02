using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Player
{
    protected string playerID;

    public string PlayerID
    {
        get { return this.playerID; }
        set { this.playerID = value; }
    }

    public abstract void Initialize();
    public abstract bool IsMine();
}

public class MyPlayer : Player
{
    private string playerKey;
    private PLAYER_STATE currentState;

    public string PlayerKey
    {
        get { return this.playerKey; }
        set { this.playerKey = value; }
    }

    public PLAYER_STATE State
    {
        get { return this.currentState; }
        set { this.currentState = value; }
    }

    public override void Initialize()
    {
        this.playerID = "";
        this.playerKey = "";
        this.currentState = PLAYER_STATE.NONE;
    }

    public void Login(string playerID, string playerKey)
    {
        this.playerID = playerID;
        this.playerKey = playerKey;
    }

    public override bool IsMine()
    {
        return true;
    }
}

public class OtherPlayer : Player
{
    public override void Initialize()
    {
        this.playerID = "";
    }

    public override bool IsMine()
    {
        return false;
    }
}