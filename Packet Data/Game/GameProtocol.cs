using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameProtocol : MonoBehaviour
{
    public void InitializeProtocol()
    {
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.HEART_BEAT, RESPONSE_HEART_BEAT);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.GAME_START, RESPONSE_GAME_START);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.GAME_END, RESPONSE_GAME_END);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.OTHER_GAME_START, RESPONSE_OTHER_GAME_START);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.OTHER_GAME_END, RESPONSE_OTHER_GAME_END);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.GAME_PLAYER_MOVE, RESPONSE_GAME_PLAYER_MOVE);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.OTHER_GAME_PLAYER_MOVE, RESPONSE_OTHER_GAME_PLAYER_MOVE);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.GAME_PLAYER_ATTACK, RESPONSE_GAME_PLAYER_ATTACK);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.OTHER_GAME_PLAYER_ATTACK, RESPONSE_OTHER_GAME_PLAYER_ATTACK);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENEMY_SPAWN, RESPONSE_ENEMY_SPAWN);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENEMY_MOVE, RESPONSE_ENEMY_MOVE);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENEMY_DAMAGED, RESPONSE_ENEMY_DAMAGED);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENEMY_ATTACK, RESPONSE_ENEMY_ATTACK);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.ENEMY_DIE, RESPONSE_ENEMY_DIE);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.GAME_PLAYER_DIE, RESPONSE_GAME_PLAYER_DIE);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.GAME_PLAYER_DAMAGED, RESPONSE_GAME_PLAYER_DAMAGED);

        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.POTION_SPAWN, RESPONSE_POTION_SPAWN);
        ProtocolManager.Instance.RegisterResponseProtocolFunc(PROTOCOL.POTION_PICKUP, RESPONSE_POTION_PICKUP);
    }

    public void RESPONSE_HEART_BEAT(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_HeartBeatPacketData recvData = new SERVERtoCLIENT_HeartBeatPacketData();
            recvData.Deserialize(packet);

            Debug.Log("Heart Beat Packet !");

            SERVERtoCLIENT_HeartBeatPacketData sendData = new SERVERtoCLIENT_HeartBeatPacketData();
            NetworkManager.Instance.SendCommand((int)PROTOCOL.HEART_BEAT, (int)EXTRA.REQUEST, sendData);
        }
    }

    public void RESPONSE_GAME_START(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            Debug.Log("Start Game!!");
            GameManager.Instance.Initialize();
        }
        else if (packet.header.extra == (int)EXTRA.FAIL)
        {
            Debug.Log("Fail Game!!");
        }
    }

    public void RESPONSE_GAME_END(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_GameEndPacketData recvData = new SERVERtoCLIENT_GameEndPacketData();
            recvData.Deserialize(packet);

            /*GameFramework.Instance.MyPlayer.State = PLAYER_STATE.IN_ROOM;
            GameFramework.Instance.GameInfo.gameKey = "";
            GameFramework.Instance.GameInfo.isHost = false;
            GameFramework.Instance.GameInfo.gamePlayers.Clear();
            SceneManager.LoadScene("Lobby");*/
            Debug.Log("Before Leave Game Scene");
            GameManager.Instance.IsGameStart = false;
            GameFramework.Instance.LeaveGameScene();
        }
    }

    public void RESPONSE_OTHER_GAME_START(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_OtherGameStartPacketData recvData = new SERVERtoCLIENT_OtherGameStartPacketData();
            recvData.Deserialize(packet);

            RoomButton roomButton = RoomManager.Instance.GetRoomButton(recvData.roomKey);
            roomButton.StartGame();
        }
    }

    public void RESPONSE_OTHER_GAME_END(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_OtherGameEndPacketData recvData = new SERVERtoCLIENT_OtherGameEndPacketData();
            recvData.Deserialize(packet);

            RoomButton roomButton = RoomManager.Instance.GetRoomButton(recvData.roomKey);
            roomButton.EndGame();
        }
    }

    public void RESPONSE_GAME_PLAYER_MOVE(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoHOST_GamePlayerMovePacketData recvData = new SERVERtoHOST_GamePlayerMovePacketData();
            recvData.Deserialize(packet);

            CLIENTtoSERVER_GamePlayerMovePacketData sendData = new CLIENTtoSERVER_GamePlayerMovePacketData();
            sendData.playerKey = recvData.playerKey;
            sendData.gameKey = GameManager.Instance.GameKey;

            sendData.posX = recvData.posX;
            sendData.posY = recvData.posY;
            sendData.posZ = recvData.posZ;

            sendData.angularX = recvData.angularX;
            sendData.angularY = recvData.angularY;
            sendData.angularZ = recvData.angularZ;            

            NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_MOVE, (int)EXTRA.HOST_TO_SERVER, sendData);
        }
    }

    public void RESPONSE_OTHER_GAME_PLAYER_MOVE(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_OtherGamePlayerMovePacketData recvData = new SERVERtoCLIENT_OtherGamePlayerMovePacketData();
            recvData.Deserialize(packet);

            Vector3 newPos = new Vector3(recvData.posX, recvData.posY, recvData.posZ);
            Vector3 newRotation = new Vector3(recvData.angularX, recvData.angularY, recvData.angularZ);
            
            GamePlayer findPlayer = GameManager.Instance.GamePlayers.Find((player) => recvData.playerID == player.Player.PlayerID);

            if(findPlayer != null)
            {
                findPlayer.transform.position = newPos;
                findPlayer.transform.eulerAngles = newRotation;
            }
        }
    }

    public void RESPONSE_GAME_PLAYER_ATTACK(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoHOST_GamePlayerAttackPacketData recvData = new SERVERtoHOST_GamePlayerAttackPacketData();
            recvData.Deserialize(packet);

            CLIENTtoSERVER_GamePlayerAttackPacketData sendData = new CLIENTtoSERVER_GamePlayerAttackPacketData();
            sendData.playerKey = recvData.playerKey;
            sendData.gameKey = GameManager.Instance.GameKey;

            NetworkManager.Instance.SendCommand((int)PROTOCOL.GAME_PLAYER_ATTACK, (int)EXTRA.HOST_TO_SERVER, sendData);
        }
    }

    public void RESPONSE_OTHER_GAME_PLAYER_ATTACK(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_OtherGamePlayerAttackPacketData recvData = new SERVERtoCLIENT_OtherGamePlayerAttackPacketData();
            recvData.Deserialize(packet);

            GamePlayer findPlayer = GameManager.Instance.GamePlayers.Find((player) => recvData.playerID == player.Player.PlayerID);

            if(findPlayer != null)
                findPlayer.RemotePlayerShoot();
        }
    }

    public void RESPONSE_ENEMY_SPAWN(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_EnemySpawnPacketData recvData = new SERVERtoCLIENT_EnemySpawnPacketData();
            recvData.Deserialize(packet);
            EnemySpawner.Instance.RemoteSpawnEnemy(recvData.enemyID, new Vector3(recvData.posX, recvData.posY, recvData.posZ));
        }
    }

    public void RESPONSE_ENEMY_MOVE(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_EnemyMovePacketData recvData = new SERVERtoCLIENT_EnemyMovePacketData();
            recvData.Deserialize(packet);

            Vector3 newPos = new Vector3(recvData.posX, recvData.posY, recvData.posZ);
            Vector3 newRotation = new Vector3(recvData.angularX, recvData.angularY, recvData.angularZ);

            Enemy findEnemy = EnemySpawner.Instance.SpawnedEnemies.Find((enemy) => recvData.enemyID == enemy.EnemyID);
            findEnemy.transform.position = newPos;
            findEnemy.transform.eulerAngles = newRotation;
        }
    }

    public void RESPONSE_ENEMY_DAMAGED(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_EnemyDamagedPacketData recvData = new SERVERtoCLIENT_EnemyDamagedPacketData();
            recvData.Deserialize(packet);

            //나중에 체력바를 추가해 GUI적으로 보여주던지.. 피격 애니메이션을 재생시키던지
            Enemy findEnemy = EnemySpawner.Instance.SpawnedEnemies.Find((enemy) => recvData.enemyID == enemy.EnemyID);
            if(findEnemy != null)
                findEnemy.DecreaseHealth();
        }
    }

    public void RESPONSE_ENEMY_ATTACK(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_EnemyAttackPacketData recvData = new SERVERtoCLIENT_EnemyAttackPacketData();
            recvData.Deserialize(packet);

            Enemy findEnemy = EnemySpawner.Instance.SpawnedEnemies.Find((enemy) => recvData.enemyID == enemy.EnemyID);
            findEnemy.transform.GetChild(0).GetComponent<Animation>().CrossFade("Enemy Attack");
        }
    }

    public void RESPONSE_ENEMY_DIE(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_EnemyDiePacketData recvData = new SERVERtoCLIENT_EnemyDiePacketData();
            recvData.Deserialize(packet);

            Enemy findEnemy = EnemySpawner.Instance.SpawnedEnemies.Find((enemy) => recvData.enemyID == enemy.EnemyID);
            Destroy(findEnemy.gameObject);
        }
    }

    public void RESPONSE_GAME_PLAYER_DIE(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_GamePlayerDiePacketData recvData = new SERVERtoCLIENT_GamePlayerDiePacketData();
            recvData.Deserialize(packet);

            GamePlayer findPlayer = GameManager.Instance.GamePlayers.Find((player) => recvData.playerID == player.Player.PlayerID);
            GameManager.Instance.GamePlayers.Remove(findPlayer);

            if(findPlayer.Player.IsMine())
                GameGUI.Instance.ShowGameOver();

            Destroy(findPlayer.gameObject);
        }
    }

    public void RESPONSE_GAME_PLAYER_DAMAGED(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_GamePlayerDamagedPacketData recvData = new SERVERtoCLIENT_GamePlayerDamagedPacketData();
            recvData.Deserialize(packet);

            GamePlayer findPlayer = GameManager.Instance.GamePlayers.Find((player) => recvData.playerID == player.Player.PlayerID);
            if (findPlayer != null)
                findPlayer.SetHealth(recvData.hp);
        }
    }

    public void RESPONSE_POTION_SPAWN(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_PotionSpawnPacketData recvData = new SERVERtoCLIENT_PotionSpawnPacketData();
            recvData.Deserialize(packet);
            PotionSpawner.Instance.RemoteSpawnPotion(recvData.potionID, new Vector3(recvData.positionX, recvData.positionY, recvData.positionZ));
        }
    }

    public void RESPONSE_POTION_PICKUP(TcpPacket packet)
    {
        if (packet.header.extra == (int)EXTRA.SUCCESS)
        {
            SERVERtoCLIENT_PotionPickupPacketData recvData = new SERVERtoCLIENT_PotionPickupPacketData();
            recvData.Deserialize(packet);
            GamePlayer findPlayer = GameManager.Instance.GamePlayers.Find((player) => recvData.playerID == player.Player.PlayerID);

            if (findPlayer != null)
                findPlayer.SetHealth(recvData.hp);

            Potion findPotion = PotionSpawner.Instance.SpawnedPotions.Find((potion) => recvData.potionID == potion.PotionID);
            Destroy(findPotion.gameObject);
        }
    }
}
