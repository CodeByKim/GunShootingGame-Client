using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour 
{
    private List<EnemySpawnPoint> _spawnPoint;
    private List<Enemy> _spawnedEnemy;
    private int _liveEnemys;

    public static EnemySpawner Instance;
    public GameObject enemyPrefab;
    public int enemyCount;

    public List<Enemy> SpawnedEnemies
    {
        get { return _spawnedEnemy; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _spawnedEnemy = new List<Enemy>();

        if(GameManager.Instance.IsHost)
        {
            _spawnPoint = new List<EnemySpawnPoint>();

            for (int i = 0; i < transform.childCount; i++)
                _spawnPoint.Add(transform.GetChild(i).GetComponent<EnemySpawnPoint>());

            StartCoroutine(SpawnEnemy());
        }
    }

    private IEnumerator SpawnEnemy()
    {
        yield return new WaitForSeconds(1);
        //여기서 Game Start! UI 띄워주는게 좋을듯..
        int spawnedCount = 0;

        while(spawnedCount < enemyCount)
        {
            int positionIndex = Random.Range(0, 4);
            Vector3 spawnPosition = _spawnPoint[positionIndex].transform.position;

            Enemy spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity).GetComponent<Enemy>();
            SpawnedEnemies.Add(spawnedEnemy);
            spawnedEnemy.Initialize();
            spawnedEnemy.EnemyID = System.Guid.NewGuid().ToString();
            spawnedEnemy.DieEvent = EnemyDie;
            spawnedCount += 1;
            _liveEnemys += 1;

            if(GameManager.Instance.IsGameStart)
            {
                //Enemy Spawn 패킷을 서버에 전송해서 다른 클라이언트들도 Enemy Spawn 되도록 해야 함
                CLIENTtoSERVER_EnemySpawnPacketData data = new CLIENTtoSERVER_EnemySpawnPacketData();
                data.gameKey = GameManager.Instance.GameKey;
                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                data.enemyID = spawnedEnemy.EnemyID;
                data.posX = spawnedEnemy.transform.position.x;
                data.posY = spawnedEnemy.transform.position.y;
                data.posZ = spawnedEnemy.transform.position.z;

                NetworkManager.Instance.SendCommand((int)PROTOCOL.ENEMY_SPAWN, (int)EXTRA.HOST_TO_SERVER, data);
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    public void RemoteSpawnEnemy(string enemyID, Vector3 position)
    {
        Enemy spawnedEnemy = Instantiate(enemyPrefab, position, Quaternion.identity).GetComponent<Enemy>();
        spawnedEnemy.EnemyID = enemyID;
        SpawnedEnemies.Add(spawnedEnemy);
    }

    public void EnemyDie(Enemy diedEnemy)
    {
        _liveEnemys -= 1;
        SpawnedEnemies.Remove(diedEnemy);

        if (_liveEnemys <= 0)
        {
            enemyCount *= 2;
            _liveEnemys = 0;
            SpawnedEnemies.Clear();
            StartCoroutine(SpawnEnemy());
        }
    }
}
