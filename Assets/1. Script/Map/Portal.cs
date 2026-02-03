using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviourPun, IInteractable
{
    [SerializeField] string _mapSceneName = "Floor";
    //[SerializeField] GameObject _moveUI;

    private PlayerInput _activeInput;
    private bool _isActivated = false;

    public void OnInteract(PlayerManager player)
    {
        if (_isActivated) return;

        if (player.photonView.IsMine)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                _isActivated = true;
                photonView.RPC(nameof(PRC_Portal), RpcTarget.All);
            }
        }
    }

    [PunRPC]
    private void PRC_Portal()
    {
        _isActivated = true;

        if (PlayerManager._instance != null)
        {
            // 내 로컬 캐릭터의 현재 실시간 데이터를 GameManager 변수에 복사
            PlayerManager._instance.SaveToManager();
            Debug.Log($"[데이터 보존] HP {PlayerManager._instance.Hp}를 가지고 다음 층으로 이동합니다.");
            Debug.Log($"<color=yellow>[[저장]] 포탈 타는 순간 내 HP: {PlayerManager._instance.Hp}</color>");
        }

        GameObject myChar = GetLocalPlayer(); //개인 캐릭터 확인
        if (myChar == null) return;

        //입력 차단
        if (myChar.TryGetComponent(out _activeInput))
        {
            _activeInput.DeactivateInput();
        }
        // 리지드바디 초기화
        if (myChar.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
        }
        // 애니메이터 초기화
        if (myChar.TryGetComponent(out Animator anim))
        {
            anim.SetFloat("Speed", 0f);
        }
        //UI 확인용
        //if (_moveUI != null)
        //{
        //    _moveUI.SetActive(true);
        //    Cursor.lockState = CursorLockMode.None;
        //    Cursor.visible = true;
        //}
        if (PhotonNetwork.IsMasterClient)
        {
            if (MapManager._instance != null)
            {
                MapManager._instance.CanMove = true;
            }

            // 모든 플레이어를 "Floor" 씬으로 함께 이동시킴
            PhotonNetwork.LoadLevel(_mapSceneName);
        }

    }

    private GameObject GetLocalPlayer()
    {
        PhotonView[] pvs = FindObjectsByType<PhotonView>(FindObjectsSortMode.None);
        foreach (var pv in pvs)
        {
            if (pv.IsMine && pv.CompareTag("Player")) return pv.gameObject;
        }
        return null;
    }

    //    public void OnClickConfirm()
    //    {
    //        if (_activeInput != null)
    //        {
    //            _activeInput.ActivateInput();
    //            _activeInput = null;
    //        }
    //        if (_moveUI != null)
    //        {
    //            _moveUI.SetActive(false);
    //        }
    //        if (MapManager._instance != null)
    //        {
    //            MapManager._instance.CanMove = true;
    //        }
    //        PhotonNetwork.LoadLevel(_mapSceneName);
    //    }

    //    public void OnClickCancel()
    //    {
    //        photonView.RPC(nameof(RPC_CancelPortal), RpcTarget.All);
    //    }
    //    [PunRPC]
    //    private void RPC_CancelPortal()
    //    {
    //        if (_activeInput != null)
    //        {
    //            _activeInput.ActivateInput();
    //            _activeInput = null;
    //        }

    //        if (_moveUI != null)
    //        {
    //            _moveUI.SetActive(false);
    //            Cursor.lockState = CursorLockMode.Locked;
    //            Cursor.visible = false;
    //        }

    //        _isActivated = false;
    //        Debug.Log("포탈 이동이 취소되어 모든 플레이어의 UI를 닫습니다.");
    //    }
}
