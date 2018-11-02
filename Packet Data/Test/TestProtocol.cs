using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestProtocol : MonoBehaviour 
{
    public void InitializeProtocol()
    {
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.KOREAN_STRING_TEST, RESPONSE_KOREAN_STRING);
    }

    public void RESPONSE_KOREAN_STRING(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_KoreanStringPacketData recvData = new SERVERtoCLIENT_KoreanStringPacketData();
            recvData.Deserialize(packet);
            Debug.Log(recvData.testString1);
            Debug.Log(recvData.testString2);
        }
    }
}
