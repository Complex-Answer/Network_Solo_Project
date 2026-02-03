using Photon.Pun;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    public void OnClick_BackToLobby()
    {
        if (GameManager._instance != null)
        {
            GameManager._instance.BackToLobby();
        }
        else
        {
            Debug.LogError("GameManager 인스턴스를 찾을 수 없습니다! 로비에서 시작했는지 확인하세요.");
        }
    }
    public void OnClick_ExitGame()
    {
        Debug.Log("게임을 종료합니다.");

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
        Application.Quit();
    }
}
