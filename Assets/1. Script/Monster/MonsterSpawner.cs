using Photon.Pun;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public string[] monsterNames = { "Slime", "Bee", "Spider" };

    public int spawnCount = 3;

    public void SpawnRandom()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        for(int i =0; i < spawnCount; i++)
        {
            string randomName = monsterNames[Random.Range(0, monsterNames.Length)];

            Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));

            PhotonNetwork.Instantiate(randomName, spawnPos, Quaternion.identity);
        }
    }
}
