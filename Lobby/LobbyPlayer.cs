using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayer
{
    public LobbyPlayerUIElement uiElement;
    public Player player;

    public LobbyPlayer(Player player)
    {
        this.player = player;
    }
}
