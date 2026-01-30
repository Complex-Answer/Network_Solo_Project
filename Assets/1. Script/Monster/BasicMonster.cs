using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;


[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonTransformView))]
public class BasicMonster : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private MonsterSO _mobData;
    private int _currentHP;
    private NavMeshAgent _agent;
    private Transform _target;
    private PlayerManager _targetHealth;
    private Collider _collider;

    private IMState _mobState;
    private Animator _animator;
    public IMState MobState => _mobState;
    public Animator Animator => _animator;
    public Transform Target { get { return _target; } set { _target = value; } }
    public NavMeshAgent Agent { get { return _agent; } }
    public Collider MobCollider => _collider;

    public float AttackSpeed => _mobData._attackSpeed;
    public int AttackDamage => _mobData._attackDamage;
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _collider = GetComponent<Collider>();

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

        foreach (Transform p in players)
        {
            if (p == null) continue;

            float distance = (p.position - myPos).sqrMagnitude;
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
    public void OnMobDamage(int damage)
    {
        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }
    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0 && PhotonNetwork.IsMasterClient)
        {
            ChangeState(new MDieState(this));
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(_currentHP); // 방장이 체력 보냄
        }
        else
        {
            _currentHP = (int)stream.ReceiveNext(); // 남들이 체력 받음
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
