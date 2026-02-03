using Photon.Pun;
using UnityEngine;

public class MDieState : IMState
{
    private BasicMonster _mob;
    private float _dieTimer;
    private float _dissolveDelay = 2.0f;
    private bool _isDestroyed = false;
    public MDieState(BasicMonster mob)
    {
        _mob  = mob;
    }
    public void Enter()
    {
        if(_mob == null)
        {
            return;
        }
        _dieTimer = Time.time;
        if (PhotonNetwork.LocalPlayer.TagObject != null)
        {
            var myPlayer = PhotonNetwork.LocalPlayer.TagObject as PlayerManager;
            //if (myPlayer != null && myPlayer.photonView.IsMine)
            //{
            //    myPlayer.Add
            //
            //    (_mob.Gold);
            //}
        }

        if (_mob.Agent != null)
        {
            _mob.Agent.isStopped = true;
            _mob.Agent.ResetPath();
            _mob.Agent.enabled = false;

        }

        if (_mob.Animator != null)
        {
            _mob.TriggerDie();
        }

        
        if (_mob.MobCollider != null) _mob.MobCollider.enabled = false;

        if (PhotonNetwork.IsMasterClient)
        {
            BattleManager._instance.RemoveMonster(_mob);
        }
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (PhotonNetwork.IsMasterClient && !_isDestroyed)
        {
            if (Time.time >= _dieTimer + _dissolveDelay)
            {
                _isDestroyed = true; // 지웠다고 표시
                PhotonNetwork.Destroy(_mob.gameObject);
            }
        }
    }
}
