using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterGUI : MonoBehaviour 
{
    public static RegisterGUI Instance;

    public InputField idInput;
    public InputField passwordInput;

    private void Awake()
    {
        Instance = this;
    }

    public void OnRequestRegister()
    {
        SoundManager.Instance.PlayUIButtonClick();
        CLIENTtoSERVER_RegisterPacketData data = new CLIENTtoSERVER_RegisterPacketData();
        data.id = this.idInput.text;
        data.password = this.passwordInput.text;
        NetworkManager.Instance.SendCommand((int)PROTOCOL.REGISTER_USER, (int)EXTRA.REQUEST, data);        
    }

    public void OnCancle()
    {
        SoundManager.Instance.PlayUIButtonClick();
        LoginSceneGUI.Instance.ChangeRegisterToLogin();
    }

    public void Release()
    {
        this.idInput.text = "";
        this.passwordInput.text = "";
    }
}
