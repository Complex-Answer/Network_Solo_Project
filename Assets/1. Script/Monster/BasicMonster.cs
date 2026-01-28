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

    private IMState _mobState;
    private Animator _animator;
    public IMState MobState => _mobState;
    public Animator Animator => _animator;
    public Transform Target {  get { return _target; } set { _target = value; } }
    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();

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
        _target = tempTarget;
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
    public void OnDamage(int damage)
    {
        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damage);
    }
    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        _currentHP -= damage;
        if (_currentHP <= 0 && PhotonNetwork.IsMasterClient)
        {
            // 방장이 확인하고 몹 제거 보고
            BattleManager._instance.RemoveMonster(this);
            PhotonNetwork.Destroy(gameObject);
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
}
