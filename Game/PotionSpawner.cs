using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionSpawner : MonoBehaviour 
{
    private List<PotionSpawnPoint> _spawnPoint;
    private List<Potion> _spawnedPotions;

    public static PotionSpawner Instance;
    public GameObject potionPrefab;

    public List<Potion> SpawnedPotions
    {
        get { return _spawnedPotions; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _spawnedPotions = new List<Potion>();

        if (GameManager.Instance.IsHost)
        {
            _spawnPoint = new List<PotionSpawnPoint>();

            for (int i = 0; i < transform.childCount; i++)
                _spawnPoint.Add(transform.GetChild(i).GetComponent<PotionSpawnPoint>());

            StartCoroutine(CreatePotion());
        }
    }

    public void RemoteSpawnPotion(string potionID, Vector3 position)
    {
        Potion spawnedPotion = Instantiate(potionPrefab, position, Quaternion.identity).GetComponent<Potion>();
        spawnedPotion.PotionID = potionID;
        spawnedPotion.ReleaseEvent = ReleasePotion;
        SpawnedPotions.Add(spawnedPotion);
    }

    private IEnumerator CreatePotion()
    {
        yield return new WaitForSeconds(5);

        while (GameManager.Instance.IsGameStart)
        {
            int positionIndex = Random.Range(0, 4);
            Vector3 spawnPosition = _spawnPoint[positionIndex].transform.position;

            Potion spawnedPotion = Instantiate(potionPrefab, spawnPosition, Quaternion.identity).GetComponent<Potion>();
            spawnedPotion.ReleaseEvent = ReleasePotion;
            spawnedPotion.PotionID = System.Guid.NewGuid().ToString();
            SpawnedPotions.Add(spawnedPotion);

            if (GameManager.Instance.IsGameStart)
            {
                CLIENTtoSERVER_PotionSpawnPacketData data = new CLIENTtoSERVER_PotionSpawnPacketData();
                data.gameKey = GameManager.Instance.GameKey;
                data.playerKey = GameFramework.Instance.MyPlayer.PlayerKey;
                data.potionID = spawnedPotion.PotionID;
                data.positionX = spawnPosition.x;
                data.positionY = spawnPosition.y;
                data.positionZ = spawnPosition.z;

                NetworkManager.Instance.SendCommand((int)PROTOCOL.POTION_SPAWN, (int)EXTRA.HOST_TO_SERVER, data);
            }

            int nextTime = Random.Range(15, 30);
            yield return new WaitForSeconds(nextTime);
        }
    }

    public void ReleasePotion(Potion potion)
    {
        SpawnedPotions.Remove(potion);
        Destroy(potion.gameObject);
    }
}
