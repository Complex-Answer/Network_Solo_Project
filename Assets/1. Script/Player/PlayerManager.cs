using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private float _maxHp = 100;
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private int _attack = 5;
    [SerializeField] private int _attackSpeed = 4;
    private float _hp;

    Rigidbody _rb;
    PlayerMove _playerMove;
    PlayerAttack _playerAttack;
    PlayerDash _playerDash;

    public PlayerMove PlayerMove => _playerMove;
    public PlayerAttack PlayerAttack => _playerAttack;
    public PlayerDash PlayerDash => _playerDash;
    public Rigidbody Rb => _rb;
    public float Hp { get { return _hp; } set { _hp = value; } }
    public float MaxHp => _maxHp;
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    public int Attack => _attack;
    public int AttackSpeed => _attackSpeed;
    void Start()
    {
        _playerMove = GetComponent<PlayerMove>();
        _playerAttack = GetComponent<PlayerAttack>();
        _playerDash = GetComponent<PlayerDash>();
        _rb = GetComponent<Rigidbody>();
    }

    void Update()
    {

    }
}
