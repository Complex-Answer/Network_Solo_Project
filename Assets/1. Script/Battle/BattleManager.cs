using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager _instance;


    public bool IsBattle { get; private set; } = false;


    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {
        if(MapManager._instance != null)
        {
            MapManager._instance.CanMove = false;
        }
    }

    public void WinBattle()
    {
        
        //if (!PhotonNetwork.IsMasterClient)
        //{
        //    return;
        //}
        if (IsBattle)
        {
            return;
        }
        IsBattle = true;
        Debug.Log("¿Ã±Ë");
        //photonView.RPC(nameof(RPC_BattleEnd), RpcTarget.All);
        RPC_BattleEnd();
    }

    //[PunRPC]
    private void RPC_BattleEnd()
    {
        IsBattle = true;
        Debug.Log("¿¸≈ı ¿Ã±Ë");

        if (MapManager._instance != null)
        {
            MapManager._instance.CanMove = true;
            MapManager._instance.RefreshMapUI();
        }

        SceneManager.LoadScene("Floor");
        // ¿Ã±‚∞Ì ¿¸≈ı ∫∏ªÛ »πµÊ
    }

}
