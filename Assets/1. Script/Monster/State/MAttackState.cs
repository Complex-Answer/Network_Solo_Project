using UnityEngine;

public class MAttackState : IMState
{

    private BasicMonster _mob;
    private float _attackCooldown = 2.0f; // 공격 간격
    private float _lastAttackTime;

    public MAttackState(BasicMonster mob)
    {
        _mob = mob;
    }
    public void Enter()
    {
        LookAtTarget();
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        
    }

    private void LookAtTarget() //플레이어 트래킹처럼 얘도 위아래 못보게끔 고정시켜줘야지
    {
        Vector3 dir = (_mob.Target.position - _mob.transform.position).normalized;
        dir.y = 0; 
        if (dir != Vector3.zero)
        {
            _mob.transform.rotation = Quaternion.Slerp(_mob.transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10f);
        }
    }
}
