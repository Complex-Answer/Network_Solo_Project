using UnityEngine;
using UnityEngine.AI;

public class MChaseState : IMState
{
    BasicMonster _mob;
    NavMeshAgent _agent;

    private float _searchTimer = 0f;
    private float _searchInterval = 0.2f;
    public MChaseState(BasicMonster mob)
    {
        _mob = mob;
        _agent = mob.GetComponent<NavMeshAgent>();
    }
    public void Enter()
    {
        if (_mob.Animator != null)
        {
            _mob.Animator.SetBool("IsMoving", true);
        }
        _mob.FindNearestPlayer();
    }

    public void Exit()
    {
        //몹 멈추게 하기
        if (_agent.hasPath)
        {
            _agent.ResetPath();
        }
        _mob.Animator.SetBool("IsMoving", false);
    }


    public void Update()
    {
        _searchTimer += Time.deltaTime;
        if (_searchTimer >= _searchInterval)
        {
            _searchTimer = 0f;
            _mob.FindNearestPlayer();
        }

        if (_mob.Target != null)
        {
            //내브메쉬를 이용해 타겟으로 이동
            _agent.SetDestination(_mob.Target.position);

            // 거리 체크
            float distance = Vector3.Distance(_mob.transform.position, _mob.Target.position);

            //공격 사거리보다 앞이라면 이제 플레이어 공격
            if (distance <= _agent.stoppingDistance)
            {
                // 공격 상태로 전환
                 _mob.ChangeState(new MAttackState(_mob));
            }
        }
    }
}
