using Photon.Pun;
using UnityEngine;

public class LobbyUI : MonoBehaviourPun
{
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    public void OnStartButtonClick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("씬 전환 중");

            PhotonNetwork.LoadLevel("Floor");
        }
        else
        {
            Debug.LogWarning("방장이 아닌 플레이어는 시작할 수 없습니다");
        }
    }
}
