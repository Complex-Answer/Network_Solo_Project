using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager _instance;

    [SerializeField] string portalPrefabName = "Portal";
    //포탈 위치
    [SerializeField] Transform portalSpawnPoint;

    private List<Transform> _playerList = new();
    private List<BasicMonster> _aliveMonsters = new();

    public bool IsBattle { get; private set; } = false;
    public bool IsSpawning { get; set; } = false;
    public List<Transform> PlayerList { get { return _playerList; } }
    private void Awake()
    {
        _instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        if(MapManager._instance != null)
        {
            MapManager._instance.CanMove = false;
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
        
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return;
        //}
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
        
        SpawnPortal();

        // 이기고 전투 보상 획득
    }
    private void SpawnPortal()
    {
        Vector3 spawnPos = (portalSpawnPoint != null) ? portalSpawnPoint.position : Vector3.zero;
        // 포탈은 모든 사람에게 보여야 하므로 네트워크 소환
        PhotonNetwork.Instantiate(portalPrefabName, spawnPos, Quaternion.identity);
    }

}
