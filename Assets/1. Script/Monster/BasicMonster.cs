using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class BasicMonster : MonoBehaviourPunCallbacks, IPunObservable, IMobDamaged
{
    [SerializeField] private MonsterSO _mobData;
    private float _currentHP;
    private NavMeshAgent _agent;
    private Transform _target;
    private PlayerManager _targetHealth;
    private Collider _collider;
    private Rigidbody _rb;
    private int _syncAttackCount = 0; 
    private int _lastAttackCount = 0;
    private bool _isDead = false;

    private IMState _mobState;
    private Animator _animator;
    public IMState MobState => _mobState;
    public Animator Animator => _animator;
    public Transform Target { get { return _target; } set { _target = value; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public Collider MobCollider => _collider;
    public Rigidbody Rigidbody => _rb;
    public float AttackSpeed => _mobData._attackSpeed;
    public float AttackDamage => _mobData._attackDamage;
        
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();
        _rb = GetComponent<Rigidbody>();

        if (_mobData != null)
        {
            if (_mobData._enemyModelPrefab != null)
            {
                GameObject model = Instantiate(_mobData._enemyModelPrefab, transform);
                model.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                _animator = model.GetComponent<Animator>();
            }

            //초기 스탯
            _currentHP = _mobData._maxHp;
            _agent.speed = _mobData._moveSpeed;
            _agent.stoppingDistance = _mobData._attackRange;
        }
    }
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            BattleManager._instance.RegisterMonster(this);
            ChangeState(new MChaseState(this));
        }

    }

    //가까운 플레이어를 찾는 메서드
    public void FindNearestPlayer()
    {
        var players = BattleManager._instance.PlayerList;

        if (players == null || players.Count == 0)
        {
            _target = null;
            return;
        }
        float minDistance = Mathf.Infinity; //거리 초기화(초기 설정은 무한대)
        Transform tempTarget = null;
        Vector3 myPos = transform.position; //몹 위치

        foreach (PlayerManager p in players)
        {
            if (p == null || p.IsDead) continue;

            float distance = (p.transform.position - myPos).sqrMagnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                tempTarget = p.transform;
            }
        }
        if (_target != tempTarget)
        {
            _target = tempTarget;

            if (_target != null)
            {
                // 공격할 때 매번 찾지 않도록 여기서 미리 컴포넌트를 캐싱해둠
                _targetHealth = _target.GetComponent<PlayerManager>();
            }
            else
            {
                _targetHealth = null;
            }
        }
    }

    void Update()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _mobState?.Update();
        }
    }

    public void ChangeState(IMState state)
    {
        _mobState?.Exit();
        _mobState = state;
        _mobState.Enter();

    }
    public void OnMobDamaged(float damage)
    {
        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.MasterClient, damage);
    }
    [PunRPC]
    private void RPC_TakeDamage(float damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _currentHP -= damage;
            if (_currentHP <= 0 && PhotonNetwork.IsMasterClient)
            {
                ChangeState(new MDieState(this));
            }
        }
    }
    public void TriggerAttack()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            _syncAttackCount++; // 숫자를 올림 (방장)
            if (_animator != null) _animator.SetTrigger("Attack");
        }
    }
    public void TriggerDie()
    {
        if (PhotonNetwork.IsMasterClient && !_isDead)
        {
            // RPC를 통해 모든 사람의 화면에서 'Die' 트리거 실행
            photonView.RPC(nameof(RPC_MonsterDie), RpcTarget.All);
        }
    }
    [PunRPC]
    private void RPC_MonsterDie()
    {
        if (_isDead) return; // 중복 실행 방지
        _isDead = true;

        if (_animator != null) _animator.SetTrigger("Die");

        if (_agent != null && _agent.isOnNavMesh) // 길 위에 있을 때만 실행
        {
            _agent.isStopped = true;
            _agent.ResetPath();
        }
            _agent.enabled = false;

        // 사망 시 물리 및 AI 정지
        _collider.enabled = false;
        _rb.isKinematic = true;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHP); // 방장이 체력 보냄
            stream.SendNext(_isDead);
            stream.SendNext(_syncAttackCount);
        }
        else
        {
            _currentHP = (float)stream.ReceiveNext(); // 남들이 체력 받음
            _isDead = (bool)stream.ReceiveNext();
            int receivedCount = (int)stream.ReceiveNext();

            if (receivedCount > _lastAttackCount)
            {
                if (_animator != null) _animator.SetTrigger("Attack");
                _lastAttackCount = receivedCount; //숫자 올라감
            }
        }
    }

    public void OnAttackHit() // 애니메이션 이벤트가 이 이름을 찾아서 호출합니다.
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (Target != null)
        {
            // 근거리 체크 후 직접 데미지
            float distance = (_target.position - transform.position).sqrMagnitude;
            float range = (_agent.stoppingDistance + 0.5f) * (_agent.stoppingDistance + 0.5f);
            if (distance <= range)
            {
                _targetHealth?.TakeDamage(AttackDamage);
            }
        }
    }
    
}
