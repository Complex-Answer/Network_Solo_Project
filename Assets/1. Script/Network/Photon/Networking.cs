using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


public class Networking : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform _spawn;
    void Start()
    {
        Debug.Log("서버에 접속 중...");
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 완료! 로비에 접속 중...");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 접속 완료!");
        PhotonNetwork.JoinOrCreateRoom("TestRoom", new RoomOptions { MaxPlayers = 4}, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("방 접속 완료! 플레이어 생성 중...");
        PhotonNetwork.Instantiate("Player", _spawn.position, Quaternion.identity);
    }


    void Update()
    {
        
    }
}
