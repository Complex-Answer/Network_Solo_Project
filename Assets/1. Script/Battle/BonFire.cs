using Photon.Pun;
using UnityEngine;

public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private string _portalPrefabName = "Portal";
    [SerializeField] Transform _portalVector;
    [SerializeField] Transform _playerSpawnPoint;
    bool _isUsed = false;
    private bool _isPlayerSpawned = false;

    private PhotonView _pv;
    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        SpawnMyPlayer();
        _isUsed = false;
    }
    public void OnInteract(PlayerManager player)
    {
        if (_isUsed) return;

        _pv.RPC(nameof(RPC_GlobalHeal), RpcTarget.All, 30f);
    }
    [PunRPC]
    private void RPC_GlobalHeal(float percent)
    {
        _isUsed = true;
        if (PlayerManager._instance != null)
        {
            float healAmount = PlayerManager._instance.MaxHp * (percent / 100f);
            PlayerManager._instance.RestoreHp(healAmount);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(_portalPrefabName, _portalVector.position, Quaternion.identity);

            Debug.Log("모닥불 사용 완료: 포탈이 소환되었습니다.");
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
            _isPlayerSpawned = true;
            Debug.Log("<color=cyan>[배틀] 플레이어 본인 소환 완료. 이제부터 게임 종료 판정이 가능합니다.</color>");
        }
    }
}
