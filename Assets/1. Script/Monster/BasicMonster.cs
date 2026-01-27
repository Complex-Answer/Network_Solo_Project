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
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();

        if (_mobData != null)
        {
            _currentHP = _mobData._maxHp;
            _agent.speed = _mobData._moveSpeed;
        }
    }
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) _target = player.transform;

        if (PhotonNetwork.IsMasterClient)
        {
            BattleManager._instance.RegisterMonster(this);
            ChangeState(new MChaseState(this));
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
