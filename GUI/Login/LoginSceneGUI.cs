using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneGUI : MonoBehaviour
{
    public static LoginSceneGUI Instance;

    public GameObject loginGUI;
    public GameObject registerGUI;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangeLoginToRegister()
    {
        //LobbyGUI.Instance.ClearRoomButtons();
        //LobbyGUI.Instance.ClearLobbyPlayerTexts();
        //ClearGenerateUIElements();
        LoginGUI.Instance.Release();

        this.loginGUI.SetActive(false);
        this.registerGUI.SetActive(true);
    }

    public void ChangeRegisterToLogin()
    {
        this.registerGUI.SetActive(false);
        this.loginGUI.SetActive(true);

        RegisterGUI.Instance.Release();
        //LobbyGUI.Instance.ClearRoomButtons();
        //LobbyGUI.Instance.ClearLobbyPlayerTexts();
    }
}
