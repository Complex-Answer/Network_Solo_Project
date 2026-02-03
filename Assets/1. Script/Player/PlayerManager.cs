using Photon.Pun;
using System;
using System.Collections;
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
    public bool IsDead { get; set; } = false;
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
            BattleManager._instance.RegisterPlayer(this);
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
    public void DestroySelfDelayed(float delay)
    {
        if (photonView.IsMine)
        {
            StartCoroutine(CoDestroySelf(delay));
        }
    }

    private IEnumerator CoDestroySelf(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (PhotonNetwork.IsConnected && photonView != null)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
    public void ReportDeath()
    {
        // 내 캐릭터가 죽었다고 모두에게 알림
        photonView.RPC(nameof(RPC_ReportGlobalDeath), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
    }

    [PunRPC]
    private void RPC_ReportGlobalDeath(int actorNr)
    {
        // 모든 클라이언트는 본인의 GameManager 장부에 이 사람이 죽었음을 기록함
        if (GameManager._instance != null)
        {
            GameManager._instance.UpdateSurvivalStatus(actorNr, false);
        }
    }
    private void OnDestroy()
    {
        if (BattleManager._instance != null)
        {
            BattleManager._instance.RemovePlayer(this);
        }
        if (photonView.IsMine && _instance == this)
        {
            _instance = null;
        }
    }
}
