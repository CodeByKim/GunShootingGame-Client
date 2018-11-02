using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginGUI : MonoBehaviour 
{
    public static LoginGUI Instance;

    public InputField idInput;
    public InputField passwordInput;

    private void Awake()
    {
        Instance = this;
    }

    public void OnShowRegisterWindow()
    {
        //GameObject panel = Resources.Load("Prefabs/Register Panel") as GameObject;
        //GameObject createdPanel = Instantiate(panel);
        //createdPanel.GetComponent<RectTransform>().SetParent(transform);
        //createdPanel.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        SoundManager.Instance.PlayUIButtonClick();
        LoginSceneGUI.Instance.ChangeLoginToRegister();
    }

    public void OnLogin()
    {
        SoundManager.Instance.PlayUIButtonClick();
        CLIENTtoSERVER_LoginPacketData data = new CLIENTtoSERVER_LoginPacketData();
        data.id = this.idInput.text;
        data.password = this.passwordInput.text;
        NetworkManager.Instance.SendCommand((int)PROTOCOL.LOGIN, (int)EXTRA.REQUEST, data);
    }

    public void Release()
    {
        this.idInput.text = "";
        this.passwordInput.text = "";
    }
}
