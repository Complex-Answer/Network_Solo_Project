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

    //이 밑에 있는 값들은 나중에 게임 매니저에서 가져올 것들
    [Header("플레이어 스탯")]
    private float _maxHp;
    private float _moveSpeed;
    private float _attack;
    private float _attackSpeed;
    private float _hp;
    private int _gold;

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
    public float MaxHp => _maxHp;
    public float MoveSpeed => _moveSpeed;
    public float Attack => _attack;
    public float AttackSpeed => _attackSpeed;
    public int Gold => _gold;
    public PlayerMove PlayerMove => _playerMove;
    public PlayerAttack PlayerAttack => _playerAttack;
    public PlayerDash PlayerDash => _playerDash;
    public Rigidbody Rb => _rb;
    public Animator Animator => _animator;

    public event Action<float, float> OnHpChanged;
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
            if (GameManager._instance != null)
            {
                _hp = GameManager._instance.Hp;
                _maxHp = GameManager._instance.MaxHp;
                _moveSpeed = GameManager._instance.MoveSpeed;
                _attack = GameManager._instance.Attack;
                _attackSpeed = GameManager._instance.AttackSpeed;
                _gold = GameManager._instance.Gold;
            }

        }
        else
        {

        }

        MoveState(new MoveState(this));
    }
    public void OnHealthChanged() //바뀔때 마다 알려주는 이벤트 함수
    {
        if (GameManager._instance != null)
        {
            GameManager._instance._currentHp = _hp;
        }
    }
    public void TakeDamage(float damaged)
    {
        if (photonView.IsMine)
        {
            _hp -= damaged;
            OnHealthChanged();

            if (_hp <= 0)
            {
                _hp = 0;
                //죽는 처리
                MoveState(new DieState(this));
            }
        }
    }

    void Update()
    {
        if (photonView != null && !photonView.IsMine) return;

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

}
