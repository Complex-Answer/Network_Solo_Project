using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class FollowPlayer : MonoBehaviourPun
{
    void Start()
    {
        if (photonView.IsMine)
        {
            var vcam = FindFirstObjectByType<CinemachineCamera>();

            if (vcam != null)
            {
                vcam.Follow = transform;
                vcam.LookAt = transform;

                Debug.Log($"{gameObject.name}: 카메라 연결 완료!");
            }
            else
            {
                Debug.LogWarning("씬에 CinemachineCamera가 없습니다!");
            }
        }
    }
    
}
