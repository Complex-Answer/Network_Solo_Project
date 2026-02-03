using Photon.Pun;
using UnityEngine;

public class BonFire : MonoBehaviour, IInteractable
{
    [SerializeField] private string _portalPrefabName = "Portal";
    [SerializeField] Transform _portalVector;
    bool _isUsed = false;

    private PhotonView _pv;
    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }
    private void Start()
    {
        _isUsed = false;
    }
    public void OnInteract(PlayerManager player)
    {
        if (_isUsed || !PhotonNetwork.IsMasterClient) return;

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
}
