using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class Portal : MonoBehaviour
{
    [SerializeField] string _mapSceneName = "Floor";
    [SerializeField] GameObject _moveUI;

    private PlayerInput _activeInput;

    private void OnTriggerEnter(Collider other)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent(out PhotonView pv) && pv.IsMine)
            {
                Debug.Log("포탈 진입");

                //입력 차단
                if (other.TryGetComponent(out _activeInput))
                {
                    _activeInput.DeactivateInput();
                }

                // 리지드바디 초기화
                if (other.TryGetComponent(out Rigidbody rb))
                {
                    rb.linearVelocity = Vector3.zero;
                }

                // 애니메이터 초기화
                if (other.TryGetComponent(out Animator anim))
                {
                    anim.SetFloat("Speed", 0f);
                }

                //UI 확인용
                if (_moveUI != null)
                {
                    _moveUI.SetActive(true);

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }

        }
    }

    public void OnClickConfirm()
    {
        if (_activeInput != null)
        {
            _activeInput.ActivateInput();
            _activeInput = null;
        }

        if (MapManager._instance != null)
        {
            MapManager._instance.CanMove = true;
        }
        PhotonNetwork.LoadLevel(_mapSceneName);
    }

    public void OnClickCancel()
    {
        if (_activeInput != null)
        {
            _activeInput.ActivateInput();
            _activeInput = null; // 참조 해제
        }

        if (_moveUI != null)
        {
            _moveUI.SetActive(false);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

}
