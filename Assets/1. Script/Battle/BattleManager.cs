using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager _instance;

    [SerializeField] string _portalPrefabName = "Portal";
    //포탈 위치
    [SerializeField] Transform _portalSpawnPoint;
    [SerializeField] Transform _playerSpawnPoint;
    [SerializeField] SetStat _statUI;

    private List<Transform> _playerList = new();
    private List<BasicMonster> _aliveMonsters = new();

    public bool IsBattle { get; private set; } = false;
    public bool IsSpawning { get; set; } = false;
    public List<Transform> PlayerList { get { return _playerList; } }
    private void Awake()
    {
        _instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;

        PhotonView pv = GetComponent<PhotonView>();
        if (pv != null)
        {
            // 만약 ID가 0이라면 강제로 1 할당 (씬에 미리 배치된 경우만 해당)
            if (pv.ViewID == 0)
            {
                pv.ViewID = 1;
                Debug.Log("<color=yellow>BattleManager: ID가 0이라 강제로 1을 할당했습니다.</color>");
            }
            else
            {
                Debug.Log($"<color=green>BattleManager: 정상적으로 ID {pv.ViewID}를 사용 중입니다.</color>");
            }
        }
    }
    private void Start()
    {
        SpawnMyPlayer();
        if (MapManager._instance != null)
        {
            MapManager._instance.CanMove = false;
        }
    }
    private void SpawnMyPlayer()
    {
        if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue("PlayerNum", out var val))
        {
            int myNumber = (int)val;
            string prefabName = $"Player {myNumber:D2}";

            int offsetIndex = myNumber - 1;
            Vector3 spawnPos = _playerSpawnPoint.position + (_playerSpawnPoint.right * offsetIndex * 2f);

            // 계산된 위치(spawnPos)로 소환
            PhotonNetwork.Instantiate(prefabName, spawnPos, _playerSpawnPoint.rotation);

            Debug.Log($"[배틀] {myNumber}번 플레이어가 기준점으로부터 {offsetIndex * 2f}m 떨어진 곳에 소환됨.");
        }
    }
    public void RegisterMonster(BasicMonster monster)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _aliveMonsters.Add(monster);
        }
    }

    // 몹이 죽을 때 호출
    public void RemoveMonster(BasicMonster monster)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _aliveMonsters.Remove(monster);
        Debug.Log($"몬스터 현재 남은 마리 수: {_aliveMonsters.Count}");
        // 몹이 다 죽었으면 승리 처리
        if (!IsSpawning && _aliveMonsters.Count <= 0)
        {
            WinBattle();
        }
    }

    public void RegisterPlayer(Transform player)
    {
        if (!PlayerList.Contains(player))
        {
            PlayerList.Add(player);
            Debug.Log("플레이어 등록 완료");
        }
    }

    public void RemovePlayer(Transform player)
    {
        if (PlayerList.Contains(player))
        {
            PlayerList.Remove(player);
        }

        _playerList.RemoveAll(p => p == null);
    }

    public void WinBattle()
    {

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
        if (IsBattle)
        {
            return;
        }
        IsBattle = true;
        Debug.Log("이김");
        photonView.RPC(nameof(RPC_BattleEnd), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_BattleEnd()
    {
        IsBattle = true;
        Debug.Log("전투 이김, 포탈 등장");

        ShowStatUpUI();
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPortal();
        }

        // 이기고 전투 보상 획득
    }
    private void SpawnPortal()
    {
        Vector3 spawnPos = (_portalSpawnPoint != null) ? _portalSpawnPoint.position : Vector3.zero;
        // 포탈은 모든 사람에게 보여야 하므로 네트워크 소환
        PhotonNetwork.Instantiate(_portalPrefabName, spawnPos, Quaternion.identity);
    }
    private void ShowStatUpUI()
    {
        _statUI.Open();
    }

}
