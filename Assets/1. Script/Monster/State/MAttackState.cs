using UnityEngine;

public class MAttackState : IMState
{

    private BasicMonster _mob;
    private float _lastAttackTime;
    float _attackCooldown;

    public MAttackState(BasicMonster mob)
    {
        _mob = mob;
    }
    public void Enter()
    {
        _lastAttackTime = Time.time;
        if (_mob.Animator != null)
        {
            _mob.Animator.SetFloat("AttackSpeed", _mob.AttackSpeed);
        }
        _attackCooldown = 1f / _mob.AttackSpeed;

        LookAtTarget();
    }

    public void Exit()
    {
        
    }

    public void Update()
    {
        if (_mob.Target == null) //타겟이 없으면 바로 추적 상태로
        {
            _mob.ChangeState(new MChaseState(_mob));
            return;
        }

        float distance = Vector3.Distance(_mob.transform.position, _mob.Target.position);

        if (distance > _mob.Agent.stoppingDistance + 0.5f)
        {
            _mob.ChangeState(new MChaseState(_mob));
            return;
        }
       
        if (Time.time >= _lastAttackTime + _attackCooldown)
        {
            _lastAttackTime = Time.time;

            if (_mob.Animator != null)
            {
                _mob.Animator.SetTrigger("Attack");
            }
        }

        LookAtTarget();
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
