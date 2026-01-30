using Photon.Pun;
using UnityEngine;

public class MDieState : IMState
{
    private BasicMonster _mob;
    private float _dieTimer;
    private float _dissolveDelay = 2.0f;
    public MDieState(BasicMonster mob)
    {
        _mob  = mob;
    }
    public void Enter()
    {
        _dieTimer = Time.time;

        if (_mob.Agent != null)
        {
            _mob.Agent.isStopped = true;
            _mob.Agent.ResetPath();
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
        if (PhotonNetwork.IsMasterClient)
        {
            if (Time.time >= _dieTimer + _dissolveDelay)
            {
                PhotonNetwork.Destroy(_mob.gameObject);
            }
        }
    }
}
