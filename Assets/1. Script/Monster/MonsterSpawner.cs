using Photon.Pun;
using System.Collections;
using UnityEngine;

public class MonsterSpawner : MonoBehaviourPunCallbacks
{
    [Header("데이터 설정")]
    [SerializeField] private MonsterGroupSO _groupData;
    private MonsterPoolSO _monsterPool; 

    [Header("스폰 관련")]
    [SerializeField] private int _totalSpawnCount = 30; // 몬스터 소환 수
    [SerializeField] private Transform[] _spawnPoints; //스폰 위치
    [SerializeField] private GameObject _exitPortal; //포탈 소환할거

    [Header("클리어 보상")]

    private int _aliveMonsters = 0; // 현재 맵에 살아있는 몹 숫자
    private bool _isStarted = false;

    public void ActivateSpawner()
    {
        if (_isStarted) return;
        _isStarted = true;

        if (PhotonNetwork.IsMasterClient)
        {
            // 3. [로그라이크 핵심] 마스터 데이터에 들어있는 여러 꾸러미 중 하나를 랜덤으로 선택
            // 예: [0:숲, 1:사막, 2:동굴] 중 랜덤으로 하나가 결정됨
            int poolIndex = Random.Range(0, _groupData._monsterPool.Count);
            _monsterPool = _groupData._monsterPool[poolIndex];

            // 4. 결정된 꾸러미를 들고 실제 소환 루틴(코루틴) 시작
            StartCoroutine(SpawnRoutine());
        }
    }
    private void SpawnMonster(GameObject prefab)
    {
        if (prefab == null) return;

        Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Length)];

        PhotonNetwork.Instantiate(prefab.name, spawnPoint.position, Quaternion.identity);

        _aliveMonsters++;
    }
    IEnumerator SpawnRoutine()
    {
        Debug.Log($"로그라이크 선택 결과: {_monsterPool.name} 꾸러미가 선택되었습니다!");

        // 설정된 총 마릿수(30마리)만큼 반복문 실행
        for (int i = 0; i < _totalSpawnCount; i++)
        {
            // 5. 선택된 꾸러미(예: 숲) 안에 들어있는 몹 리스트 중 하나를 또 랜덤으로 선택
            // 예: 숲 꾸러미 안의 [슬라임, 벌, 거미] 중 하나를 뽑음
            int randomIndex = Random.Range(0, _monsterPool.monsterPrefabs.Count);
            GameObject selectedPrefab = _monsterPool.monsterPrefabs[randomIndex];

            // 6. 실제 소환 함수 실행 (위치 선정 및 Instantiate)
            SpawnMonster(selectedPrefab);

            yield return new WaitForSeconds(0.2f);
        }

        StartCoroutine(CheckClearRoutine());
    }

    IEnumerator CheckClearRoutine()
    {
        while (_aliveMonsters > 0)
        {
            yield return new WaitForSeconds(0.5f);
        }

        // [RPC] 방장이 포탈을 열기로 결정했으니 모든 플레이어에게 알림
        photonView.RPC(nameof(RPC_OpenPortal), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_OpenPortal()
    {
        if (_exitPortal != null) _exitPortal.SetActive(true);
    }

    public void OnMonsterDestroyed()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _aliveMonsters--;
            if (_aliveMonsters < 0) _aliveMonsters = 0;
        }
    }
}
