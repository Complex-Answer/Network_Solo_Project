using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;


public class Networking : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform[] _spawn;
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

        //들어온 유저의 번호를 저장하는 리스트
        List<int> usedNumbers = new();
        
        foreach (var p in PhotonNetwork.PlayerListOthers)
        {
            //CustomProperties는 딕셔너리로 PlayerNum을 저장, val로 반환
            if (p.CustomProperties.TryGetValue("PlayerNum", out var val))
            {
                //object타입이라 박싱이 일어나지만 들어올때만 실행되는거라 상관은 없을듯? - 이거말고 다른게 생각도 안남 ㅋㅋ;
                usedNumbers.Add((int)val);
            }
        }
        int myNumber = -1;

        //빈 번호 찾기
        for (int i = 1; i <= 4; i++)
        {
            if (!usedNumbers.Contains(i))
            {
                myNumber = i;
                break;
            }
        }
        if (myNumber == -1)
        {
            myNumber = 1;
        }
        //위에서 번호 찾은걸 PlayerNum에다가 저장 처음은 무조건 1이겠지?
        ExitGames.Client.Photon.Hashtable props = new()
        {
            { "PlayerNum", myNumber }
        };
        //위에서 쓴걸 이제 저장시키는거지
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);

        string prefabName = $"Player {myNumber:D2}";
        int spawnIndex = myNumber - 1;

        if(spawnIndex < _spawn.Length)
        {
            PhotonNetwork.Instantiate(prefabName, _spawn[spawnIndex].position, _spawn[spawnIndex].rotation);
        }
        else
        {
            PhotonNetwork.Instantiate(prefabName, Vector3.zero, Quaternion.identity);
        }
    }//까먹지 말라고 주석 다쳐놨다...
}
