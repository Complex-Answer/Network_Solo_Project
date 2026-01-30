using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPunCallbacks
{
    [Header("데이터 설정")]
    [SerializeField] private MonsterGroupSO _groupData;
    private MonsterPoolSO _monsterPool;

    [Header("스폰 관련")]
    [SerializeField] private Transform[] _spawnPoints; //스폰 위치
    private int _totalSpawnCount;// 몬스터 소환 수

    private bool _isStarted = false;

    public void ActivateSpawner()
    {
        if (_isStarted) return;
        _isStarted = true;

        if (PhotonNetwork.IsMasterClient)
        {
            int poolIndex = Random.Range(0, _groupData._monsterPool.Count);
            _monsterPool = _groupData._monsterPool[poolIndex];

            StartCoroutine(SpawnRoutine());
        }
    }
    private void SpawnMonster(GameObject prefab)
    {
        if (prefab == null) return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, Quaternion.identity);
    }
    IEnumerator SpawnRoutine()
    {
        _totalSpawnCount = Random.Range(20, 41);
        Debug.Log($"{_monsterPool.name} 꾸러미가 선택되었습니다!");

        for (int i = 0; i < _totalSpawnCount; i++)
        {
            int randomIndex = Random.Range(0, _monsterPool.monsterPrefabs.Count);
            GameObject selectedPrefab = _monsterPool.monsterPrefabs[randomIndex];

            SpawnMonster(selectedPrefab);

            yield return new WaitForSeconds(0.2f);
        }
    }
}
