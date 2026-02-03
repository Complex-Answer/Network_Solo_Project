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
        Vector2 randomCircle = Random.insideUnitCircle * 3f; //직선으로 나오는게 아닌 원형으로 하나씩 튀어나옴

        //위의 원을 바탕으로 랜덤으로 소환
        Vector3 finalSpawnPos = spawnPoint.position + new Vector3(randomCircle.x, 0, randomCircle.y);

        PhotonNetwork.Instantiate(prefab.name, finalSpawnPos, Quaternion.identity);
    }
    IEnumerator SpawnRoutine()
    {
        _totalSpawnCount = Random.Range(10, 21);
        //_totalSpawnCount = 1;
        Debug.Log($"{_monsterPool.name} 꾸러미가 선택되었습니다!");

        for (int i = 0; i < _totalSpawnCount; i++)
        {
            int randomIndex = Random.Range(0, _monsterPool.monsterPrefabs.Count);
            GameObject selectedPrefab = _monsterPool.monsterPrefabs[randomIndex];

            SpawnMonster(selectedPrefab);

            yield return new WaitForSeconds(0.5f);
        }
    }
}
