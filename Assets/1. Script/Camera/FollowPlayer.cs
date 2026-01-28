using Photon.Pun;
using Unity.Cinemachine;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    void Start()
    {
        PhotonView pv = GetComponent<PhotonView>();

        if (pv.IsMine)
        {
            //CinemachineCamera cinema = 
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
