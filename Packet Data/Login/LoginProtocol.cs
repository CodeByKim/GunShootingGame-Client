using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginProtocol : MonoBehaviour {
	
	void Start ()
    {
        
    }

    public void InitializeProtocol()
    {
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.REGISTER_USER, RESPONSE_REGISTER_USER);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.LOGIN, RESPONSE_LOGIN);
    }

    public void RESPONSE_REGISTER_USER(TcpPacket packet)
    {
        if(packet.header.extra == (int)EXTRA.SUCCESS)
        {
            Debug.Log("SUCCESS REGISTER USER");
            LoginSceneGUI.Instance.ChangeRegisterToLogin();
        }
        else if(packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("FAIL REGISTER USER");

            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LoginSceneGUI.Instance.loginGUI);
            messageBox.SetTitle("중복된 ID입니다. 다시입력하세요.");

            messageBox.AddConfirmEventFunc(RegisterGUI.Instance.Release);
        }
    }

    public void RESPONSE_LOGIN(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_LoginPacketData recvData = new SERVERtoCLIENT_LoginPacketData();
            recvData.Deserialize(packet);

            GameFramework.Instance.MyPlayer.Login(recvData.playerID, recvData.playerKey);
            GameFramework.Instance.MyPlayer.State = PLAYER_STATE.LOBBY;
            SceneManager.LoadScene("Lobby");
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("INVALID ID & PASSWORD");

            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LoginSceneGUI.Instance.loginGUI);
            messageBox.SetTitle("잘못된 ID, 또는 Password 입니다.");

            messageBox.AddConfirmEventFunc(LoginGUI.Instance.Release);
        }
        else if (packet.header.extra == (int)EXTRA.OVERLAPED_LOGIN)
        {
            Debug.Log("OVERLAPED LOGIN");

            MessageBox messageBox = UIFactory.Instance.Create(UI_ELEMENT.MESSAGE_BOX).GetComponent<MessageBox>();
            messageBox.Initialize();
            messageBox.AttachUIElement(new Vector2(0, 50), LoginSceneGUI.Instance.loginGUI);
            messageBox.SetTitle("이미 접속되어 있습니다.");

            messageBox.AddConfirmEventFunc(LoginGUI.Instance.Release);
        }
    }
}
