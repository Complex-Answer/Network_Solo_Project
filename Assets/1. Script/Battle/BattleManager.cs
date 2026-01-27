using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviourPunCallbacks
{
    public static BattleManager _instance;


    public bool IsBattle { get; private set; } = false;

    private List<BasicMonster> _aliveMonsters = new();
    private void Awake()
    {
        _instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    private void Start()
    {
        if(MapManager._instance != null)
        {
            MapManager._instance.CanMove = false;
        }
    }
    public void RegisterMonster(BasicMonster monster)
    {
        if (PhotonNetwork.IsMasterClient) _aliveMonsters.Add(monster);
    }

    // ¸÷ÀÌ Á×À» ¶§ È£Ãâ
    public void RemoveMonster(BasicMonster monster)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        _aliveMonsters.Remove(monster);

        // ¸÷ÀÌ ´Ù Á×¾úÀ¸¸é ½Â¸® Ã³¸®
        if (_aliveMonsters.Count <= 0)
        {
            WinBattle();
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
        Debug.Log("ÀÌ±è");
        //photonView.RPC(nameof(RPC_BattleEnd), RpcTarget.All);
        RPC_BattleEnd();
    }

    //[PunRPC]
    private void RPC_BattleEnd()
    {
        IsBattle = true;
        Debug.Log("ÀüÅõ ÀÌ±è");

        if (MapManager._instance != null)
        {
            MapManager._instance.CanMove = true;
        }

        SceneManager.LoadScene("Floor");
        //PhotonNetwork.LoadLevel("Floor");
        // ÀÌ±â°í ÀüÅõ º¸»ó È¹µæ
    }

}
