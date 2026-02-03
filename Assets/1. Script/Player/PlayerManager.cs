using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
/// <summary>
/// 플레이어 매니저입니다
/// 플레이어의 상태와 컴포넌트 및 인풋을 관리합니다
/// </summary>
public class PlayerManager : MonoBehaviourPun
{
    static public PlayerManager _instance;
    //이 밑에 있는 값들은 나중에 게임 매니저에서 가져올 것들
    [Header("플레이어 스탯")]
    private float _hp;
    //private int _gold;

    [Header("컴포넌트")]
    Rigidbody _rb;
    PlayerMove _playerMove;
    PlayerAttack _playerAttack;
    PlayerDash _playerDash;
    PlayerInteract _playerInteract;
    Animator _animator;


    [Header("인풋 액션")]
    InputAction _moveAction;
    InputAction _interactAction;
    InputAction _dashAction;
    InputAction _attackAction;

    [Header("프로퍼티")]
    private IState _state;
    public float Hp => _hp;
    public float MaxHp { get; private set; }
    public float MoveSpeed { get; private set; }
    public float Attack { get; private set; }
    public float AttackSpeed { get; private set; }
    //public int Gold => _gold;
    public PlayerMove PlayerMove => _playerMove;
    public PlayerAttack PlayerAttack => _playerAttack;
    public PlayerDash PlayerDash => _playerDash;
    public Rigidbody Rb => _rb;
    public Animator Animator => _animator;

    public event Action<float, float> OnHpChanged;
    //public event Action<int> OnGoldChanged;

    private void Awake()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerDash = GetComponent<PlayerDash>();
        _playerInteract = GetComponent<PlayerInteract>();
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();

        _moveAction = InputSystem.actions["Move"];
        _interactAction = InputSystem.actions["Interact"];
        _dashAction = InputSystem.actions["Dash"];
        _attackAction = InputSystem.actions["Attack"];
    }
    void Start()
    {
       
        if (photonView.IsMine)
        {
            _instance = this;
            LoadStatsFromManager();
        }
        else
        {
            if (GameManager._instance != null)
            {
                MaxHp = GameManager._instance.MaxHp;
                _hp = MaxHp; // 일단 풀피로 설정 (이후 동기화)
                OnHpChanged?.Invoke(_hp, MaxHp);
            }
        }

        if (BattleManager._instance != null)
        {
            BattleManager._instance.RegisterPlayer(this.transform);
        }

        MoveState(new MoveState(this));
    }
    private void LoadStatsFromManager()
    {
        var gm = GameManager._instance;
        if (gm == null)
        {
            return;
        }
        _hp = gm.Hp;
        MaxHp = gm.MaxHp;
        MoveSpeed = gm.MoveSpeed;
        Attack = gm.Attack;
        AttackSpeed = gm.AttackSpeed;

        OnHpChanged?.Invoke(_hp, MaxHp);

    }
    public void SaveToManager()
    {
        if (!photonView.IsMine || GameManager._instance == null)
        {
            return;
        }
        GameManager._instance.SavePlayerStats(_hp);
    }
    public void TakeDamage(float damaged)
    {
        photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.All, damaged);
    }
    [PunRPC]
    private void RPC_TakeDamage(float damaged)
    {
        ProcessDamage(damaged);
    }

    private void ProcessDamage(float damaged)
    {
        _hp -= damaged;
        _hp = Mathf.Clamp(_hp, 0, MaxHp);

        OnHpChanged?.Invoke(_hp, MaxHp);

        if (_hp <= 0 && photonView.IsMine)
        {
            _hp = 0;
            if (_state is not DieState)
            {
                //죽는 처리
                MoveState(new DieState(this));
            }
        }
    }
    //public void Add
    //
    //
    //(int amount)
    //{
    //    if (!photonView.IsMine) return; 

    //    _
    //
    //    += amount;
    //    On
    //
    //
    //    Changed?.Invoke(_gold);
    //}
    public void RestoreHp(float amount)
    {
        _hp += amount;
        _hp = Mathf.Clamp(_hp, 0, MaxHp);

        OnHpChanged?.Invoke(_hp, MaxHp);
    }
    void Update()
    {
        if (photonView != null && !photonView.IsMine) return;

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Lobby")
        {
            _state?.Update();
            return;
        }

        Vector2 moveDir = _moveAction.ReadValue<Vector2>();
        _playerMove.SetMoveInput(moveDir);

        _playerInteract.Interact();

        if (_dashAction.WasPressedThisFrame())
        {
            _playerDash.OnDash();
        }
        if (_attackAction.IsPressed())
        {
            _playerAttack.OnAttack();
        }
        if (_interactAction.WasPressedThisFrame())
        {
            _playerInteract.DoInteract();
        }

        _state?.Update();
    }
    public void MoveState(IState state)
    {
        _state?.Exit();
        _state = state;
        _state.Enter();
    }
    private void OnDestroy()
    {
        if (BattleManager._instance != null)
        {
            BattleManager._instance.RemovePlayer(this.transform);
        }
        if (photonView.IsMine && _instance == this)
        {
            _instance = null;
        }
    }
}
